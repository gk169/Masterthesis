import sys, pathlib
RTVC_Path = str(pathlib.Path(__file__).parent.resolve().joinpath('RealTimeVoiceCloning'))
if not RTVC_Path in sys.path:
    sys.path.append(RTVC_Path)

#from encoder.params_model import model_embedding_size as speaker_embedding_size
#from utils.argutils import print_args
#from utils.modelutils import check_model_paths
from synthesizer.inference import Synthesizer
from encoder import inference as encoder
from vocoder import inference as vocoder

import numpy as np
import soundfile as sf
#import librosa
from datetime import datetime
#import argparse
import torch
#import ffmpeg
#import os
#from audioread.exceptions import NoBackendError

Common_File_Path = pathlib.Path(__file__).parent.parent.parent.joinpath('weights').joinpath('RealTimeVoiceCloning')

#Audio_Source_Path = pathlib.Path(__file__).parent.parent.parent.joinpath('data').joinpath('original').joinpath('audios').joinpath('Thorsten_sample.wav')#'Biden_short.mp3')
Audio_Source_Path = pathlib.Path(__file__).parent.joinpath('RealTimeVoiceCloning').joinpath('samples').joinpath('p240_00000.wav')
Audio_Target_Path = pathlib.Path(__file__).parent.parent.parent.joinpath('data').joinpath('generated').joinpath('audios')

'''if not Audio_Source_Path.suffix == '.wav':
    print('Extracting raw audio...')
    Audio_Path = str(Audio_Source_Path)
    print('MP3Source:\n'+ Audio_Path)
    print(Audio_Source_Path.exists())
    
    New_Audio_Source_Path = Audio_Source_Path.parent.joinpath(Audio_Source_Path.stem + '.wav')
    New_Audio_Path = str(New_Audio_Source_Path)
    print('Wav-Source:\n' + New_Audio_Path)
    
    #Input_Audio_Stream
    Stream = ffmpeg.input('RealTimeVoiceCloning/samples/p240_00000.mp3')#Audio_Path)
    #Output_Audio_Stream
    Stream = ffmpeg.output(Stream, 'RealTimeVoiceCloning/samples/p240_00000.wav')#New_Audio_Path)
    #Output_Audio_Stream.run(overwrite_output=True)
    #ffmpeg.run(Stream)
    #Audio_Source_Path = New_Audio_Source_Path'''

Text = 'The coronavirus is currently changing life in our country dramatically.\nOur idea of normality,\nof public life and social interaction:\nAll of this is being put to the test like never before.\nMillions of them cannot go to work,\ntheir children cannot go to school or to daycare.\nTheaters,\ncinemas and shops are closed\nand what is perhaps the hardest thing:\nWe all miss the encounter that we usually do.\n'
#Text = 'Das Coronavirus verändert zur Zeit das Leben in unserem Land dramatisch.\nUnsere Vorstellung von Normalität,\nvon öffentlichen Leben,\nvon sozialem Miteinander:\nAll das wird auf die Probe gestellt wie nie zuvor.\nMillionen von ihnen können nicht zur Arbeit.\nIhre Kinder können nicht zur Schule oder in die Kita.\nTheater und Kinos und Geschäfte sind geschlossen,\nund was vielleicht das Schwerste ist:\nUns allen fehlen die Begegnung die sonst selbstverständlich sind.'
print(Text)
enc_model_fpath = Common_File_Path.joinpath('encoder.pt')
syn_model_fpath = Common_File_Path.joinpath('synthesizer.pt')
voc_model_fpath = Common_File_Path.joinpath('vocoder.pt')

    
print("Running a test of your configuration...\n")
    
if torch.cuda.is_available():
    device_id = torch.cuda.current_device()
    gpu_properties = torch.cuda.get_device_properties(device_id)
    ## Print some environment information (for debugging purposes)
    print("Found %d GPUs available. Using GPU %d (%s) of compute capability %d.%d with "
        "%.1fGb total memory.\n" % 
        (torch.cuda.device_count(),
        device_id,
        gpu_properties.name,
        gpu_properties.major,
        gpu_properties.minor,
        gpu_properties.total_memory / 1e9))

## Remind the user to download pretrained models if needed
#check_model_paths(encoder_path=enc_model_fpath,
#                  synthesizer_path=syn_model_fpath,
#                  vocoder_path=voc_model_fpath)

## Load the models one by one.
print("Preparing the encoder, the synthesizer and the vocoder...")
encoder.load_model(enc_model_fpath)
synthesizer = Synthesizer(syn_model_fpath)
vocoder.load_model(voc_model_fpath)

## Interactive speech generation
## Computing the embedding
# First, we load the wav using the function that the speaker encoder provides. This is 
# important: there is preprocessing that must be applied.

# The following two methods are equivalent:
# - Directly load from the filepath:
preprocessed_wav = encoder.preprocess_wav(Audio_Source_Path)
print("Loaded file succesfully")

# Then we derive the embedding. There are many functions and parameters that the 
# speaker encoder interfaces. These are mostly for in-depth research. You will typically
# only use this function (with its default parameters):
embed = encoder.embed_utterance(preprocessed_wav)
print("Created the embedding")

# If seed is specified, reset torch seed and force synthesizer reload
#if args.seed is not None:
torch.manual_seed(5)#args.seed)
synthesizer = Synthesizer(syn_model_fpath)

# The synthesizer works in batch, so you need to put your data in a list or numpy array
texts = [Text]
embeds = [embed]

# If you know what the attention layer alignments are, you can retrieve them here by
# passing return_alignments=True
specs = synthesizer.synthesize_spectrograms(texts, embeds)
spec = specs[0]
print("Created the mel spectrogram")


## Generating the waveform
print("Synthesizing the waveform:")

# If seed is specified, reset torch seed and reload vocoder
#if args.seed is not None:
torch.manual_seed(5)#args.seed)
vocoder.load_model(voc_model_fpath)

# Synthesizing the waveform is fairly straightforward. Remember that the longer the
# spectrogram, the more time-efficient the vocoder.
generated_wav = vocoder.infer_waveform(spec)


## Post-generation
# There's a bug with sounddevice that makes the audio cut one second earlier, so we
# pad it.
generated_wav = np.pad(generated_wav, (0, synthesizer.sample_rate), mode="constant")

# Trim excess silences to compensate for gaps in spectrograms (issue #53)
generated_wav = encoder.preprocess_wav(generated_wav)

# Save it on the disk
now = datetime.now()
dt_string = now.strftime("%Y-%m-%d_%H-%M-%S-%f")

filename = str(Audio_Target_Path.joinpath('RTVC_' + dt_string + '.wav'))

print(generated_wav.dtype)
sf.write(filename, generated_wav.astype(np.float32), synthesizer.sample_rate)

print("\nSaved output as %s\n\n" % filename)