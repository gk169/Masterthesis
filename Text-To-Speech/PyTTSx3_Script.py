# -*- coding: utf-8 -*-

import pyttsx3
import sys

Text = sys.argv[1]
Voice = sys.argv[2]
Output_Path = sys.argv[3]


engine = pyttsx3.init()

#engine.setProperty('voice',voices[1].id)

engine.save_to_file(Text, Output_Path)

engine.runAndWait()