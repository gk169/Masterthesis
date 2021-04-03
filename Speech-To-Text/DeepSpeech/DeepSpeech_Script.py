import numpy as np
import sys
import json

import deepspeech

from timeit import default_timer as timer


Model_Path = sys.argv[1]    #'output_graph_de.pbmm'))
Scorer_Path = sys.argv[2]   #'kenlm_de.scorer'))
Audio_Path = sys.argv[3]    #.wav
Output_Path = sys.argv[4]   #.txt


ds = deepspeech.Model(Model_Path)

ds.enableExternalScorer(Scorer_Path)

import librosa
# Load audio as np.float32, range [-1 ... 1]
audio, _ = librosa.core.load(Audio_Path)
# Extend range to int16 [-32767 ... 32767]
audio = audio * 32767
# Convert np.float32 to np.int16
audio = audio.astype(np.int16)

inference_start = timer()
text = ds.stt(audio)
inference_end = timer() - inference_start
print('Inference took %0.3fs.' % (inference_end))

f = open(Output_Path,"w", encoding="utf-8")
f.write(text)
f.close()