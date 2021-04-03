#!/usr/bin/env python3

import speech_recognition as sr
import pathlib

Common_File_Path = pathlib.Path(__file__).parent.parent.parent.parent
# Checkpoint_Path: Name of saved checkpoint to load weights from
Audio_Path = str(Common_File_Path
                 .joinpath('data')
                 .joinpath('original')
                 .joinpath('audios')
                 .joinpath('Merkel_short.wav'))

LANGUAGE = 'de-DE'#'en-US'

# use the audio file as the audio source
r = sr.Recognizer()
with sr.AudioFile(Audio_Path) as source:
    audio = r.record(source)  # read the entire audio file

# recognize speech using Sphinx
try:
    print("Sphinx:\n" + r.recognize_sphinx(audio, language=LANGUAGE) + '\n')
except sr.UnknownValueError:
    print("Sphinx could not understand audio")
except sr.RequestError as e:
    print("Sphinx error; {0}".format(e))

# recognize speech using Google Speech Recognition
try:
    print("Google:\n" + r.recognize_google(audio, language=LANGUAGE) + '\n')
except sr.UnknownValueError:
    print("Google Speech Recognition could not understand audio")
except sr.RequestError as e:
    print("Could not request results from Google Speech Recognition service; {0}".format(e))