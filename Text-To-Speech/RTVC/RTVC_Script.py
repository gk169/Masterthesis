import sys, pathlib

RTVC_Path = str(pathlib.Path(__file__).parent.resolve().joinpath('RealTimeVoiceCloning'))
if not RTVC_Path in sys.path:
    sys.path.append(RTVC_Path)

#if not 'RealTimeVoiceCloning' in sys.path:
#    sys.path.append('RealTimeVoiceCloning')

from synthesizer.inference import Synthesizer
from encoder import inference as encoder
from vocoder import inference as vocoder

import numpy as np
import soundfile as sf
#import pathlib
#from datetime import datetime
import torch

Audio_Source_Path = pathlib.Path(sys.argv[1])   #.wav
Text = sys.argv[2]                              #"text"
enc_model_fpath = pathlib.Path(sys.argv[3])     #encoder.pt
syn_model_fpath = pathlib.Path(sys.argv[4])     #synthesizer.pt
voc_model_fpath = pathlib.Path(sys.argv[5])     #vocoder.pt
Audio_Target_Path = sys.argv[6]                 #.wav 

    
if torch.cuda.is_available():
    device_id = torch.cuda.current_device()
    gpu_properties = torch.cuda.get_device_properties(device_id)

## Load the models
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

# Then we derive the embedding. There are many functions and parameters that the 
# speaker encoder interfaces. These are mostly for in-depth research. You will typically
# only use this function (with its default parameters):
embed = encoder.embed_utterance(preprocessed_wav)

# If seed is specified, reset torch seed and force synthesizer reload
torch.manual_seed(5)
synthesizer = Synthesizer(syn_model_fpath)

# The synthesizer works in batch, so you need to put your data in a list or numpy array
texts = [Text]
embeds = [embed]

# If you know what the attention layer alignments are, you can retrieve them here by
specs = synthesizer.synthesize_spectrograms(texts, embeds)
spec = specs[0]

## Generating the waveform
# If seed is specified, reset torch seed and reload vocoder
torch.manual_seed(5)
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

sf.write(Audio_Target_Path, generated_wav.astype(np.float32), synthesizer.sample_rate)