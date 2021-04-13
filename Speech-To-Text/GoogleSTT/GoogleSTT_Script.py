import speech_recognition as sr
import sys
from time import time

Audio_File_Path = sys.argv[1]   #.wav
Audio_Language = sys.argv[2]    #de-DE
Output_Path = sys.argv[3]       #.txt

tic_all = time()
r = sr.Recognizer()
with sr.AudioFile(Audio_File_Path) as source:
    audio = r.record(source)  # read the entire audio file
    
text = r.recognize_google(audio, language=Audio_Language)

f = open(Output_Path,"w", encoding="utf-8")
f.write(text)
f.close()

toc_all = time()
print("All took: %f" % (toc_all - tic_all))

input()#"Enter to end")