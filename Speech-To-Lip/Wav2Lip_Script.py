import numpy as np
import cv2, os
from tqdm import tqdm
import torch
import ffmpeg
import sys, pathlib, tempfile
import math

Wav2Lip_Path = str(pathlib.Path(__file__).parent.resolve().joinpath('Wav2Lip'))
if not Wav2Lip_Path in sys.path:
    sys.path.append(Wav2Lip_Path)

import audio
import face_detection
from models import Wav2Lip
from time import time

# Checkpoint_Path: Name of saved checkpoint to load weights from
Wav2Lip_Weights_Path = sys.argv[1]  #wav2lip.pth
# Face: Filepath of video/image that contains faces to use
Input_Video_Path = sys.argv[2]      #.mp4
# Audio: Filepath of video/audio file to use as raw audio source
Input_Audio_Path = sys.argv[3]      #.wav
# Outfile: Video path to save result. See default for an e.g.
Output_Video_Path = sys.argv[4]     #.mp4

tic_all = time()

# Pads: default=[0, 10, 0, 0], Padding (top, bottom, left, right). Please adjust to include chin at least
Pads = [0, 10, 0, 0]

# Face_det_batch_size: Batch size for face detection, default = 16
Face_Detection_Batch_Size = 64 # bottleneck of the implementation

# Wav2lip_batch_size: Batch size for Wav2Lip model(s), default=128
Wav2Lip_Batch_Size = 1024

# Nosmooth: default=False, Prevent smoothing face detections over a short temporal window
Nosmooth = True

Img_size = 96

Temp_Path = pathlib.Path(tempfile.gettempdir())
Temp_Audio_Path = str(Temp_Path.joinpath('temp.wav'))
Temp_Video_Path = str(Temp_Path.joinpath('temp.mp4'))

Video_Path = pathlib.Path(Input_Video_Path)
face_det_path = Video_Path.parent.joinpath(Video_Path.stem + ".npy")

device = 'cuda' if torch.cuda.is_available() else 'cpu'
print('Using {} for inference.'.format(device))

print('Reading video frames...')
tic = time()

video_stream = cv2.VideoCapture(Input_Video_Path)
fps = video_stream.get(cv2.CAP_PROP_FPS)

full_frames = []

while video_stream.isOpened():
    reading, frame = video_stream.read()
    if reading: full_frames.append(frame)
    else: break
    
video_stream.release()
toc = time()
print("Reading video frames took: %fs" % (toc - tic))
print ("Number of frames available: %d" % len(full_frames))

print("Loading wav ...")
tic = time()
wav = audio.load_wav(Input_Audio_Path, 16000)
mel = audio.melspectrogram(wav)

mel_chunks = []
mel_idx_multiplier = 80./fps 
mel_step_size = 16
i = 0
while 1:
    start_idx = int(i * mel_idx_multiplier)
    if start_idx + mel_step_size > len(mel[0]):
        mel_chunks.append(mel[:, len(mel[0]) - mel_step_size:])
        break
    mel_chunks.append(mel[:, start_idx : start_idx + mel_step_size])
    i += 1
toc = time()
print("Loading wav took: %fs" % (toc - tic))
print("Number of mel chunks available: %d" % len(mel_chunks))

if os.path.isfile(face_det_path):
    print("Loading detected faces...")
    face_det_results = np.load(face_det_path, allow_pickle=True)
else:
    print("Detecting faces ...")
    tic = time()    
    detector = face_detection.FaceAlignment(face_detection.LandmarksType._2D, flip_input=False, device=device)
    face_positions = []
    for i in tqdm(range(0, len(full_frames)-1, Face_Detection_Batch_Size),file=sys.stdout):
        if i + Face_Detection_Batch_Size < len(full_frames):
            face_positions.extend(detector.get_detections_for_batch(np.array(full_frames[i : i + Face_Detection_Batch_Size])))
        else:
            face_positions.extend(detector.get_detections_for_batch(np.array(full_frames[i : len(full_frames)])))

    del detector

    mouths = []
    pady1, pady2, padx1, padx2 = Pads
    for rect, image in zip(face_positions, full_frames):
        if rect is None:
            cv2.imwrite(Wav2Lip_Path + '/temp/faulty_frame.jpg', image) # check this frame where the face was not detected.
            raise ValueError('Face not detected! Ensure the video contains a face in all the frames.')

        y1 = max(0, rect[1] - pady1)
        y2 = min(image.shape[0], rect[3] + pady2)
        x1 = max(0, rect[0] - padx1)
        x2 = min(image.shape[1], rect[2] + padx2)
        
        mouths.append([x1, y1, x2, y2])

    mouth_boxes = np.array(mouths)

    if not Nosmooth:
        T = 5
        for i in range(len(mouth_boxes)):
            if i + T > len(mouth_boxes):
                window = mouth_boxes[len(mouth_boxes) - T:]
            else:
                window = mouth_boxes[i : i + T]
            mouth_boxes[i] = np.mean(window, axis=0)

    face_det_results = [[image[y1:y2, x1:x2], (y1, y2, x1, x2)] for image, (x1, y1, x2, y2) in zip(full_frames, mouth_boxes)]
    np.save(face_det_path, face_det_results)
    toc = time()
    print("Detecting faces took: %fs" % (toc - tic))

print("Number of detected faces available: %d" % len(face_det_results))

mel_chunks = np.array(mel_chunks)
full_frames = np.array(full_frames)

#print(mel_chunks.shape)
#print(full_frames.shape)
#print(face_det_results.shape)

if len(mel_chunks) > len(full_frames):
    repeats = math.ceil(len(mel_chunks) / len(full_frames))
    #print("repeats: %d" % repeats)
    full_frames = np.tile(full_frames, (repeats, 1, 1, 1))
    face_det_results = np.tile(face_det_results, (repeats,1))

#print(full_frames.shape)
#print(face_det_results.shape)
#input()

full_frames = full_frames[:len(mel_chunks)]
face_det_results = face_det_results[:len(mel_chunks)]

def datagen(frames, face_det_results, mels):
    img_batch, mel_batch, frame_batch, coords_batch = [], [], [], []
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

gen = datagen(full_frames.copy(), face_det_results, mel_chunks)

print("Loading weights ...")
tic = time()

model = Wav2Lip()
print("Load checkpoint from: {}".format(Wav2Lip_Weights_Path))
if device == 'cuda':
    checkpoint = torch.load(Wav2Lip_Weights_Path)
else:
    checkpoint = torch.load(Wav2Lip_Weights_Path, map_location=lambda storage, loc: storage)
s = checkpoint["state_dict"]
new_s = {}
for k, v in s.items():
    new_s[k.replace('module.', '')] = v
model.load_state_dict(new_s)
model = model.to(device)
model = model.eval()

toc = time()
print("Loading weights took: %fs" % (toc - tic))

print("Generating faces ...")
tic = time()
frame_h, frame_w = full_frames[0].shape[:-1]

out = cv2.VideoWriter(Temp_Video_Path, cv2.VideoWriter_fourcc(*'X264'), fps, (frame_w, frame_h)) #DIVX
 
for i, (img_batch, mel_batch, frames, coords) in enumerate(tqdm(gen, total=int(np.ceil(float(len(mel_chunks))/Wav2Lip_Batch_Size)), file=sys.stdout)):
    
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
toc = time()
print("Generating faces took: %fs" % (toc - tic))

print("Generating video ...")
tic = time()
Input_Audio_Stream = ffmpeg.input(Input_Audio_Path)
Input_Video_Stream = ffmpeg.input(Temp_Video_Path)
Output_Video_Stream = ffmpeg.output(Input_Audio_Stream, Input_Video_Stream, Output_Video_Path, **{'qscale:v': 3})
Output_Video_Stream.run(overwrite_output=True)
toc = time()
print("Generating video took: %fs" % (toc - tic))

toc_all = time()
print("All took: %f" % (toc_all - tic_all))

input()#"Enter to end"