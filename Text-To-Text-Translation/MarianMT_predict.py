# -*- coding: utf-8 -*-

import pathlib
from transformers import MarianMTModel, MarianTokenizer
import time

Common_File_Path = pathlib.Path(__file__).parent.parent.parent
# Checkpoint_Path: Name of saved checkpoint to load weights from
Model_Path = str(Common_File_Path
                   .joinpath('weights')
                   .joinpath('MarianMT')
                   .joinpath('de-en'))

src_text = ['>>en<< Coronavirus verändert zur Zeit das Leben in unserem Land dramatisch unsere Vorstellung von Normalität von öffentlichen Leben von sozialem Miteinander all das wird auf die Probe gestellt wie nie zuvor Millionen von ihnen können nicht zur Arbeit ihre Kinder können nicht zur Schule oder in die Kita Theater und Kinos und Geschäfte sind geschlossen und was vielleicht das Schwerste ist uns allen fehlen die Begegnung die sonst selbst']

start = time.time()
tokenizer = MarianTokenizer.from_pretrained(Model_Path)
end = time.time()
print('Tokenizer load took: {:5.3f}s'.format(end-start))


start = time.time()
model = MarianMTModel.from_pretrained(Model_Path)
end = time.time()
print('Model load took: {:5.3f}s'.format(end-start))

start = time.time()
translated = model.generate(**tokenizer.prepare_seq2seq_batch(src_text, return_tensors="pt"))
end = time.time()
print('Translate took: {:5.3f}s'.format(end-start))


start = time.time()
tgt_text = [tokenizer.decode(t, skip_special_tokens=True) for t in translated]
end = time.time()
print('Decoding took: {:5.3f}s'.format(end-start))

print('MarianMT:')
print(tgt_text[0])