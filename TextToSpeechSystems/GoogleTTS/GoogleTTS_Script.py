# -*- coding: utf-8 -*-

import gtts
import sys
from time import time

Input_Path = sys.argv[1]          #.txt
Language = sys.argv[2]      #"de"
#Voice = sys.argv[2]
Output_Path = sys.argv[3]    #".wav"

print("Start transcribing ...")
tic_all = time()
f = open(Input_Path,"r", encoding="utf-8")
Input_Text = f.read()
f.close()

tts = gtts.gTTS(text=Input_Text, lang=Language)

tts.save(Output_Path)

toc_all = time()
print("All took: %f" % (toc_all - tic_all))

input()#"Enter to end"