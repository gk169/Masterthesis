# -*- coding: utf-8 -*-

import pyttsx3
import sys
import json

Output_Path = sys.argv[1]

engine = pyttsx3.init()

voices = engine.getProperty('voices')


f = open(Output_Path,"w", encoding="utf-8")

for voice in voices:
    id = str(voice.id)
    nameParts = str(voice.name).split('-')
    name = nameParts[0].strip()
    language = nameParts[1].strip()
    f.write(name+' \t'+id+' \t'+language+'\n')
    
f.close()