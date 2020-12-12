#!/usr/bin/env python3

import speech_recognition as sr

AUDIO_FILE = 'data/TTS_output.wav'#'Biden_short.mp3'

LANGUAGE = 'en-US'#'de-DE'

# use the audio file as the audio source
r = sr.Recognizer()
with sr.AudioFile(AUDIO_FILE) as source:
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