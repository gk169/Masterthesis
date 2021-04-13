# -*- coding: utf-8 -*-

import pyttsx3
import sys, os
from time import time

Input_Path = sys.argv[1]          #"text"
VoiceID = sys.argv[2]       #VoiceID e.g. "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\TTS_MS_EN-US_ZIRA_11.0"
Output_Path = sys.argv[3]   #.wav / .mp3

tic_all = time()

if os.path.exists(Output_Path):
    print("Deleting previous ...")
    os.remove(Output_Path)

f = open(Input_Path,"r", encoding="utf-8")
Input_Text = f.read()
f.close()

engine = pyttsx3.init()

engine.setProperty('voice',VoiceID)
engine.setProperty('rate',130)
engine.save_to_file(Input_Text, Output_Path)

engine.runAndWait()

toc_all = time()
print("All took: %f" % (toc_all - tic_all))

input()#"Enter to end"