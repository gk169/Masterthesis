# -*- coding: utf-8 -*-

import requests
import urllib
import sys
from time import time

sourceLangCode = sys.argv[1]    #de
targetLangCode = sys.argv[2]    #en

input_path = sys.argv[3]        #.txt
output_path = sys.argv[4]       #.txt

tic_all = time()
f = open(input_path,"r", encoding="utf-8")
input_text = f.read()
f.close()

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

toc_all = time()

print("All took: %f" % (toc_all - tic_all))

input()#"Enter to end")