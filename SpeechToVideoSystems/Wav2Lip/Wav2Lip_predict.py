import numpy as np
import cv2, os
from tqdm import tqdm
import torch
import ffmpeg
from datetime import datetime

import sys, pathlib
Wav2Lip_Path = str(pathlib.Path(__file__).parent.resolve().joinpath('Wav2Lip'))
if not Wav2Lip_Path in sys.path:
    sys.path.append(Wav2Lip_Path)

import audio
import face_detection
from models import Wav2Lip


Common_File_Path = pathlib.Path(__file__).parent.parent.parent
# Checkpoint_Path: Name of saved checkpoint to load weights from
Wav2Lip_Weights_Path = str(Common_File_Path
                           .joinpath('weights')
                           .joinpath('Wav2Lip')
                           .joinpath('wav2lip.pth'))

# Face: Filepath of video/image that contains faces to use
Input_Video_Path = str(Common_File_Path
                       .joinpath('data')
                       .joinpath('original')
                       .joinpath('videos')
                       .joinpath('Biden_short.mp4'))

# Audio: Filepath of video/audio file to use as raw audio source
Input_Audio_Path = str(Common_File_Path
                       .joinpath('data')
                       .joinpath('original')
                       .joinpath('audios')
                       .joinpath('Merkel_short.mp3'))

# Outfile: Video path to save result. See default for an e.g.
now = datetime.now()
dt_string = now.strftime("%Y-%m-%d_%H-%M-%S")

Output_Video_Path = str(Common_File_Path
                       .joinpath('data')
                       .joinpath('generated')
                       .joinpath('videos')
                       .joinpath('Wav2Lip_' + dt_string + '.mp4'))

# Pads: default=[0, 10, 0, 0], Padding (top, bottom, left, right). Please adjust to include chin at least
Pads = [0, 10, 0, 0]

# Face_det_batch_size: Batch size for face detection, default = 16
Face_Detection_Batch_Size = 20 # bottleneck of the implementation

# Wav2lip_batch_size: Batch size for Wav2Lip model(s), default=128
Wav2Lip_Batch_Size = 128

# Resize_factor: default=1, Reduce the resolution by this factor. Sometimes, best results are obtained at 480p or 720p
Resize_factor = 1

# Crop: default=[0, -1, 0, -1], Crop video to a smaller region (top, bottom, left, right).
# Applied after resize_factor and rotate arg. Useful if multiple face present.
# -1 implies the value will be auto-inferred based on height, width
Crop = [0, -1, 0, -1]

# Nosmooth: default=False, Prevent smoothing face detections over a short temporal window
Nosmooth = False

Img_size = 96

Temp_Path = pathlib.Path(__file__).parent.resolve().joinpath('temp')
if not Temp_Path.exists():
    Temp_Path.mkdir()
    

Temp_Audio_Path = str(Temp_Path.joinpath('temp.wav'))
Temp_Video_Path = str(Temp_Path.joinpath('temp.avi'))

def get_smoothened_boxes(boxes, T):
    for i in range(len(boxes)):
        if i + T > len(boxes):
            window = boxes[len(boxes) - T:]
        else:
            window = boxes[i : i + T]
        boxes[i] = np.mean(window, axis=0)
    return boxes

def face_detect(images):
    detector = face_detection.FaceAlignment(face_detection.LandmarksType._2D, flip_input=False, device=device)

    batch_size = Face_Detection_Batch_Size
    
    while 1:
        predictions = []
        try:
            for i in tqdm(range(0, len(images), batch_size)):
                predictions.extend(detector.get_detections_for_batch(np.array(images[i:i + batch_size])))
        except RuntimeError:
            if batch_size == 1: 
                raise RuntimeError('Image too big to run face detection on GPU. Please use the --resize_factor argument')
            batch_size //= 2
            print('Recovering from OOM error; New batch size: {}'.format(batch_size))
            continue
        break

    results = []
    pady1, pady2, padx1, padx2 = Pads
    for rect, image in zip(predictions, images):
        if rect is None:
            cv2.imwrite(Wav2Lip_Path + '/temp/faulty_frame.jpg', image) # check this frame where the face was not detected.
            raise ValueError('Face not detected! Ensure the video contains a face in all the frames.')

        y1 = max(0, rect[1] - pady1)
        y2 = min(image.shape[0], rect[3] + pady2)
        x1 = max(0, rect[0] - padx1)
        x2 = min(image.shape[1], rect[2] + padx2)
        
        results.append([x1, y1, x2, y2])

    boxes = np.array(results)
    if not Nosmooth: boxes = get_smoothened_boxes(boxes, T=5)
    results = [[image[y1: y2, x1:x2], (y1, y2, x1, x2)] for image, (x1, y1, x2, y2) in zip(images, boxes)]

    del detector
    return results 

def datagen(frames, mels):
    img_batch, mel_batch, frame_batch, coords_batch = [], [], [], []
    face_det_results = face_detect(frames)

    for i, m in enumerate(mels):
        idx =  i%len(frames)
        frame_to_save = frames[idx].copy()
        face, coords = face_det_results[idx].copy()

        face = cv2.resize(face, (Img_size, Img_size))
            
        img_batch.append(face)
        mel_batch.append(m)
        frame_batch.append(frame_to_save)
        coords_batch.append(coords)

        if len(img_batch) >= Wav2Lip_Batch_Size:
            img_batch, mel_batch = np.asarray(img_batch), np.asarray(mel_batch)

            img_masked = img_batch.copy()
            img_masked[:, Img_size//2:] = 0

            img_batch = np.concatenate((img_masked, img_batch), axis=3) / 255.
            mel_batch = np.reshape(mel_batch, [len(mel_batch), mel_batch.shape[1], mel_batch.shape[2], 1])

            yield img_batch, mel_batch, frame_batch, coords_batch
            img_batch, mel_batch, frame_batch, coords_batch = [], [], [], []

    if len(img_batch) > 0:
        img_batch, mel_batch = np.asarray(img_batch), np.asarray(mel_batch)

        img_masked = img_batch.copy()
        img_masked[:, Img_size//2:] = 0

        img_batch = np.concatenate((img_masked, img_batch), axis=3) / 255.
        mel_batch = np.reshape(mel_batch, [len(mel_batch), mel_batch.shape[1], mel_batch.shape[2], 1])

        yield img_batch, mel_batch, frame_batch, coords_batch

mel_step_size = 16
device = 'cuda' if torch.cuda.is_available() else 'cpu'
print('Using {} for inference.'.format(device))

def _load(checkpoint_path):
    if device == 'cuda':
        checkpoint = torch.load(checkpoint_path)
    else:
        checkpoint = torch.load(checkpoint_path,
                                map_location=lambda storage, loc: storage)
    return checkpoint

def load_model(path):
    model = Wav2Lip()
    print("Load checkpoint from: {}".format(path))
    checkpoint = _load(path)
    s = checkpoint["state_dict"]
    new_s = {}
    for k, v in s.items():
        new_s[k.replace('module.', '')] = v
    model.load_state_dict(new_s)

    model = model.to(device)
    return model.eval()

#def main():
if not os.path.isfile(Input_Video_Path):
    raise ValueError('--face argument must be a valid path to video/image file')

else:
    video_stream = cv2.VideoCapture(Input_Video_Path)
    fps = video_stream.get(cv2.CAP_PROP_FPS)

    print('Reading video frames...')

    full_frames = []
    while 1:
        still_reading, frame = video_stream.read()
        if not still_reading:
            video_stream.release()
            break
        if Resize_factor > 1:
            frame = cv2.resize(frame, (frame.shape[1]//Resize_factor, frame.shape[0]//Resize_factor))

        y1, y2, x1, x2 = Crop
        if x2 == -1: x2 = frame.shape[1]
        if y2 == -1: y2 = frame.shape[0]

        frame = frame[y1:y2, x1:x2]

        full_frames.append(frame)

print ("Number of frames available for inference: "+str(len(full_frames)))

if not Input_Audio_Path.endswith('.wav'):
    print('Extracting raw audio...')
    
    Input_Audio_Stream = ffmpeg.input(Input_Audio_Path)
    Output_Audio_Stream = ffmpeg.output(Input_Audio_Stream, Temp_Audio_Path)
    Output_Audio_Stream.run(overwrite_output=True)#Overwrite)

    Input_Audio_Path = Temp_Audio_Path

wav = audio.load_wav(Input_Audio_Path, 16000)
mel = audio.melspectrogram(wav)
#print(mel.shape)

if np.isnan(mel.reshape(-1)).sum() > 0:
    raise ValueError('Mel contains nan! Using a TTS voice? Add a small epsilon noise to the wav file and try again')

mel_chunks = []
mel_idx_multiplier = 80./fps 
i = 0
while 1:
    start_idx = int(i * mel_idx_multiplier)
    if start_idx + mel_step_size > len(mel[0]):
        mel_chunks.append(mel[:, len(mel[0]) - mel_step_size:])
        break
    mel_chunks.append(mel[:, start_idx : start_idx + mel_step_size])
    i += 1

print("\n Length of mel chunks: {}".format(len(mel_chunks)))

full_frames = full_frames[:len(mel_chunks)]

batch_size = Wav2Lip_Batch_Size
gen = datagen(full_frames.copy(), mel_chunks)

for i, (img_batch, mel_batch, frames, coords) in enumerate(tqdm(gen, total=int(np.ceil(float(len(mel_chunks))/batch_size)))):
    if i == 0:
        model = load_model(Wav2Lip_Weights_Path)
        print ("Model loaded")
        frame_h, frame_w = full_frames[0].shape[:-1]
        out = cv2.VideoWriter(Temp_Video_Path, cv2.VideoWriter_fourcc(*'DIVX'), fps, (frame_w, frame_h))
    img_batch = torch.FloatTensor(np.transpose(img_batch, (0, 3, 1, 2))).to(device)
    mel_batch = torch.FloatTensor(np.transpose(mel_batch, (0, 3, 1, 2))).to(device)
    with torch.no_grad():
        pred = model(mel_batch, img_batch)
    pred = pred.cpu().numpy().transpose(0, 2, 3, 1) * 255.
    for p, f, c in zip(pred, frames, coords):
        y1, y2, x1, x2 = c
        p = cv2.resize(p.astype(np.uint8), (x2 - x1, y2 - y1))
        f[y1:y2, x1:x2] = p
        out.write(f)

out.release()

Input_Audio_Stream = ffmpeg.input(Input_Audio_Path)
Input_Video_Stream = ffmpeg.input(Temp_Video_Path)
Output_Video_Stream = ffmpeg.output(Input_Audio_Stream, Input_Video_Stream, Output_Video_Path)
Output_Video_Stream.run(overwrite_output=True)