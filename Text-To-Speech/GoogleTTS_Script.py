# -*- coding: utf-8 -*-

import gtts
import sys

Text = sys.argv[1]
Language = sys.argv[2]
#Voice = sys.argv[2]
OutputPath = sys.argv[3]

tts = gtts.gTTS(text=Text, lang=Language)

tts.save(OutputPath)