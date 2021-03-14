# -*- coding: utf-8 -*-

import requests
import urllib
#import sys

sourceLangCode = 'de'
targetLangCode = 'en'

input_text = "Das Coronavirus verändert zur Zeit das Leben in unserem Land dramatisch.\n Unsere Vorstellung von Normalität, von öffentlichen Leben, von sozialem Miteinander: All das wird auf die Probe gestellt wie nie zuvor. Millionen von ihnen können nicht zur Arbeit. Ihre Kinder können nicht zur Schule oder in die Kita. Theater, Kinos und Geschäfte sind geschlossen. Und was vielleicht das Schwerste ist: Uns allen fehlen die Begegnung die sonst selbstverständlich sind."#'Coronavirus verändert zur Zeit das Leben in unserem Land dramatisch unsere Vorstellung von Normalität von öffentlichen Leben von sozialem Miteinander all das wird auf die Probe gestellt wie nie zuvor Millionen von ihnen können nicht zur Arbeit ihre Kinder können nicht zur Schule oder in die Kita Theater und Kinos und Geschäfte sind geschlossen und was vielleicht das Schwerste ist uns allen fehlen die Begegnung die sonst selbst'
output_path = r"C:\Users\Kevin\AppData\Local\Temp\TranslatedText.txt"#sys.argv[4]

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

print(translated_text)