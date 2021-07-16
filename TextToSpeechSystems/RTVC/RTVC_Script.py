import sys, pathlib

RTVC_Path = str(pathlib.Path(__file__).parent.resolve().joinpath('RealTimeVoiceCloning'))
if not RTVC_Path in sys.path:
    sys.path.append(RTVC_Path)

from synthesizer.inference import Synthesizer
from encoder import inference as encoder
from vocoder import inference as vocoder

import numpy as np
import soundfile as sf
import os
import torch
from time import time

Audio_Source_Path = pathlib.Path(sys.argv[1])   #.wav
Input_Path = sys.argv[2]                        #.txt
enc_model_fpath = pathlib.Path(sys.argv[3])     #encoder.pt
syn_model_fpath = pathlib.Path(sys.argv[4])     #synthesizer.pt
voc_model_fpath = pathlib.Path(sys.argv[5])     #vocoder.pt
Audio_Target_Path = sys.argv[6]                 #.wav 

tic_all = time()

if os.path.exists(Audio_Target_Path):
    print("Deleting previous ...")
    os.remove(Audio_Target_Path)

print("Loading text ...")
tic = time()
f = open(Input_Path,"r", encoding="utf-8")
Input_Text = f.read()
f.close()
toc = time()
print("Loading text took: %fs" % (toc - tic))

if torch.cuda.is_available():
    device_id = torch.cuda.current_device()
    gpu_properties = torch.cuda.get_device_properties(device_id)

print("Loading synthesizer weights ...")
tic = time()
encoder.load_model(enc_model_fpath)
synthesizer = Synthesizer(syn_model_fpath)
toc = time()
print("Loading synthesizer weights took: %fs" % (toc - tic))

print("Preprocessing voice ...")
tic = time()
preprocessed_wav = encoder.preprocess_wav(Audio_Source_Path)
embed = encoder.embed_utterance(preprocessed_wav)
toc = time()
print("Preprocessing voice took: %fs" % (toc - tic))

print("Generate features ...")
tic = time()
texts = Input_Text.split(".")
embeds = [embed] * len(texts)

specs = synthesizer.synthesize_spectrograms(texts, embeds)

all_specs = None
for spec in specs:
    print(np.shape(spec))
    if all_specs is None:
        all_specs = spec
    else:
        all_specs = np.concatenate((all_specs, spec), axis=1)
        
toc = time()
print("Feature genaraton took: %fs" % (toc - tic))

print("Free memory for vocoding ...")
sample_rate = synthesizer.sample_rate
synthesizer = None
torch.cuda.empty_cache()

print("Loading vocoder weights ...")
tic = time()
vocoder.load_model(voc_model_fpath)
toc = time()
print("Loading vocoder weights took: %fs" % (toc - tic))

print("Vocoding ...")
tic = time()
generated_wav = vocoder.infer_waveform(all_specs)
toc = time()
print("\nVocoding took: %fs" % (toc - tic))

## Post-generation
# There's a bug with sounddevice that makes the audio cut one second earlier, so we
# pad it.
#generated_wav = np.pad(generated_wav, (0, synthesizer.sample_rate), mode="constant")

# Trim excess silences to compensate for gaps in spectrograms (issue #53)
#generated_wav = encoder.preprocess_wav(generated_wav)

print("Saving audio ...")
tic = time()
sf.write(Audio_Target_Path, generated_wav.astype(np.float32), sample_rate)
toc = time()
print("Saving took: %fs" % (toc - tic))


toc_all = time()
print("All took: %f" % (toc_all - tic_all))

input()#"Enter to end"