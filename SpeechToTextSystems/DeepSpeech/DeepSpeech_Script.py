import numpy as np
import sys
import json

import deepspeech
import librosa
#from timeit import default_timer as timer
from time import time


Model_Path = sys.argv[1]    #'output_graph_de.pbmm'))
Scorer_Path = sys.argv[2]   #'kenlm_de.scorer'))
Audio_Path = sys.argv[3]    #.wav
Output_Path = sys.argv[4]   #.txt

tic_all = time()
tic = time()
ds = deepspeech.Model(Model_Path)
toc = time()
print("Model loading took: %f" % (toc - tic))

tic = time()
ds.enableExternalScorer(Scorer_Path)
toc = time()
print("Scorer loading took: %f" % (toc - tic))

# Load audio as np.float32, range [-1 ... 1]
audio, _ = librosa.core.load(Audio_Path)
# Extend range to int16 [-32767 ... 32767]
audio = audio * 32767
# Convert np.float32 to np.int16
audio = audio.astype(np.int16)

tic = time()
text = ds.stt(audio)
toc = time()
print('Inference took %0.3fs.' % (toc-tic))

f = open(Output_Path,"w", encoding="utf-8")
f.write(text)
f.close()

toc_all = time()

print("All took: %f" % (toc_all - tic_all))

input()#"Enter to end")