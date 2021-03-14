# -*- coding: utf-8 -*-

import requests
import urllib
import sys

sourceLangCode = sys.argv[1]
targetLangCode = sys.argv[2]

input_text = sys.argv[3]
output_path = sys.argv[4]

text = urllib.parse.quote(input_text)

url = 'https://translate.googleapis.com/translate_a/single?client=gtx&sl=' + sourceLangCode + "&tl=" + targetLangCode + "&dt=t&q=" + text

request = requests.get(url)

json = request.json()

data = json[0]

translated_text = ""

for line in data:
    translated_text = translated_text + line[0]

translated_text = translated_text.replace(u"\u200B","")

f = open(output_path,"w", encoding="utf-8")
f.write(translated_text)
f.close()