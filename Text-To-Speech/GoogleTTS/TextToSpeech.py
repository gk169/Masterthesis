# -*- coding: utf-8 -*-
"""
Created on Tue Feb 23 12:44:54 2021

@author: AI-Student
"""

import gtts
import pyttsx3
import pathlib
from datetime import datetime
import time

#Text = 'Hello World'
Text = 'Hallo Welt'
#Language = 'en'
Language = 'de'


# Google

start = time.time()

tts = gtts.gTTS(text=Text, lang=Language)

end = time.time()

print('Gooogle sysnthesis took: {:5.3f}s'.format(end-start))

now = datetime.now()
dt_string = now.strftime("%Y-%m-%d_%H-%M-%S-%f")

Output_Audio_Path = str(pathlib.Path(__file__).parent.parent.parent
                        .joinpath('data')
                        .joinpath('generated')
                        .joinpath('audios')
                        .joinpath('GoogleTTS_' + dt_string + '.mp3'))

tts.save(Output_Audio_Path)

#print(gtts.lang.tts_langs())

# pyttsx3

Output_Audio_Path = str(pathlib.Path(__file__).parent.parent.parent
                        .joinpath('data')
                        .joinpath('generated')
                        .joinpath('audios')
                        .joinpath('Pyttsx3_' + dt_string + '.mp3'))


start = time.time()

engine = pyttsx3.init()

# engine.setProperty('voice',voices[1].id)

engine.save_to_file('Test', Output_Audio_Path)
engine.runAndWait()

end = time.time()

print('Pyttsx3 sysnthesis took: {:5.3f}s'.format(end-start))
