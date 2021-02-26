# -*- coding: utf-8 -*-

import requests
import urllib

sourceLang = 'de'
targetLang = 'en'

text = 'Coronavirus verändert zur Zeit das Leben in unserem Land dramatisch unsere Vorstellung von Normalität von öffentlichen Leben von sozialem Miteinander all das wird auf die Probe gestellt wie nie zuvor Millionen von ihnen können nicht zur Arbeit ihre Kinder können nicht zur Schule oder in die Kita Theater und Kinos und Geschäfte sind geschlossen und was vielleicht das Schwerste ist uns allen fehlen die Begegnung die sonst selbst'

text = urllib.parse.quote(text)

url = 'https://translate.googleapis.com/translate_a/single?client=gtx&sl=' + sourceLang + "&tl=" + targetLang + "&dt=t&q=" + text

x = requests.get(url)

reponse_text = x.text

splitted = reponse_text.split('["')

tobeused_splitted = splitted[1:len(splitted):2]

translation_parts = list()

for part in tobeused_splitted:
    splitted_parts = part.split('","')
    translation_parts.append(splitted_parts[0])

separator = ''
text_translated = separator.join(translation_parts)

text_translated = text_translated.replace("\\u200b",'')

print('Google Translator API:')
print(text_translated)