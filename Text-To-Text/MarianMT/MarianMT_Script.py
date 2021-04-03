# -*- coding: utf-8 -*-

import sys
import tqdm
import transformers

Model_Path = sys.argv[1]    #MarianMT/German-English
src_text = sys.argv[2]      #"text"
Output_Path = sys.argv[3]   #.txt

tokenizer = transformers.MarianTokenizer.from_pretrained(Model_Path)
model = transformers.MarianMTModel.from_pretrained(Model_Path)

#start = time.time()
translated = model.generate(**tokenizer.prepare_seq2seq_batch(src_text, return_tensors="pt"))
#end = time.time()
#print('Translate took: {:5.3f}s'.format(end-start))

tgt_text = [tokenizer.decode(t, skip_special_tokens=True) for t in translated]

f = open(Output_Path,"w", encoding="utf-8")
f.write(tgt_text[0])
f.close()