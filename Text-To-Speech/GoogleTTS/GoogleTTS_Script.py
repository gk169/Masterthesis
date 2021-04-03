# -*- coding: utf-8 -*-

import gtts
import sys

Text = sys.argv[1]          #"text"
Language = sys.argv[2]      #"de"
#Voice = sys.argv[2]
OutputPath = sys.argv[3]    #".wav"

tts = gtts.gTTS(text=Text, lang=Language)

tts.save(OutputPath)