# -*- coding: utf-8 -*-

import sys
import tqdm
import transformers
from time import time

Model_Path = sys.argv[1]    #MarianMT/German-English
Input_Path = sys.argv[2]    #.txt
Output_Path = sys.argv[3]   #.txt

tic_all = time()

f = open(Input_Path,"r", encoding="utf-8")
Input_Text = f.read()
f.close()

tokenizer = transformers.MarianTokenizer.from_pretrained(Model_Path)
model = transformers.MarianMTModel.from_pretrained(Model_Path)
#model.to('cuda')

#start = time.time()

src_text = Input_Text.split(". ")

model.to('cuda')
batch = tokenizer(src_text, return_tensors="pt", padding=True).to('cuda')
gen = model.generate(**batch).to('cuda')
tgt_text = tokenizer.batch_decode(gen, skip_special_tokens=True)

#translated = model.generate(**tokenizer.prepare_seq2seq_batch(src_text, return_tensors="pt"))
#translated = model.generate(**tokenizer(src_text, return_tensors="pt", padding=True))
#end = time.time()
#print('Translate took: {:5.3f}s'.format(end-start))

#tgt_text = [tokenizer.decode(t, skip_special_tokens=True) for t in translated]

tgt_text_wo_empty = []

for string in tgt_text:
    print(string)
    if (string != ""):
        string_wo_fullstop = string.replace(".", "")
        tgt_text_wo_empty.append(string_wo_fullstop)

separator = ". "

output_text = separator.join(tgt_text_wo_empty)

f = open(Output_Path,"w", encoding="utf-8")
f.write(output_text)
f.close()

toc_all = time()
print("All took: %f" % (toc_all - tic_all))

input()#"Enter to end")