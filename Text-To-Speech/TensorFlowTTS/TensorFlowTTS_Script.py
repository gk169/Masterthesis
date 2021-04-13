#!/usr/bin/env python
# coding: utf-8

import os
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3'

import tensorflow as tf
import soundfile as sf
import sys
from time import time
import numpy as np

from tensorflow_tts.inference import TFAutoModel
from tensorflow_tts.inference import AutoConfig
from tensorflow_tts.inference import AutoProcessor

# Inputs
Input_Path = sys.argv[1]                #"text"

Feature_Generation_Name = sys.argv[2]   #'Tacotron2'
Feature_Generation = sys.argv[3]        #tacotron2-120k.h5
Feature_Config = sys.argv[4]            #tacotron2.v1.yaml

Vocoder_Name = sys.argv[5]              #'MB_MelGAN'
Vocoder = sys.argv[6]                   #mb.melgan-940k.h5
Vocoder_Config = sys.argv[7]            #multiband_melgan.v1.yaml

Processor = sys.argv[8]                 #ljspeech_mapper.json

Output_Audio_Path = sys.argv[9]         #.wav
tic_all = time()

print("Loading text...")
tic = time()
f = open(Input_Path,"r", encoding="utf-8")
Input_Text = f.read()
f.close()
toc = time()
print("Loading text took: %fs" % (toc - tic))

print("Loading weights ...")
tic = time()
Feature_Config = AutoConfig.from_pretrained(Feature_Config)
Vocoder_Config = AutoConfig.from_pretrained(Vocoder_Config)
Processor = AutoProcessor.from_pretrained(Processor)
Feature_Generator = TFAutoModel.from_pretrained(config=Feature_Config, pretrained_path=Feature_Generation, name=Feature_Generation_Name)
Vocoder = TFAutoModel.from_pretrained(config=Vocoder_Config, pretrained_path=Vocoder, name=Vocoder_Name)
toc = time()
print("Loading weights took: %fs" % (toc - tic))

print("Preprocess text ...")
tic = time()
input_ids = Processor.text_to_sequence(Input_Text)
toc = time()
print("Preprocessing text took: %fs" % (toc - tic))

print("Generate features ...")
tic = time()
n = 500
split_ids = [input_ids[i * n:(i + 1) * n] for i in range((len(input_ids) + n - 1) // n )]

mel_outputs_all = None
for ids in split_ids:
    if Feature_Generator.name == "Tacotron2":
        _, mel_outputs, _, _ = Feature_Generator.inference(
            tf.expand_dims(tf.convert_to_tensor(ids, dtype=tf.int32), 0),
            tf.convert_to_tensor([len(ids)], tf.int32),
            tf.convert_to_tensor([0], dtype=tf.int32)
        )
    elif Feature_Generator.name == "Fastspeech1":
        _, mel_outputs, _ = Feature_Generator.inference(
            input_ids=tf.expand_dims(tf.convert_to_tensor(ids, dtype=tf.int32), 0),
            speaker_ids=tf.convert_to_tensor([0], dtype=tf.int32),
            speed_ratios=tf.convert_to_tensor([1.0], dtype=tf.float32),
        )
    elif Feature_Generator.name == "Fastspeech2":
        _, mel_outputs, _, _, _ = Feature_Generator.inference(
            tf.expand_dims(tf.convert_to_tensor(ids, dtype=tf.int32), 0),
            speaker_ids=tf.convert_to_tensor([0], dtype=tf.int32),
            speed_ratios=tf.convert_to_tensor([1.0], dtype=tf.float32),
            f0_ratios=tf.convert_to_tensor([1.0], dtype=tf.float32),
            energy_ratios=tf.convert_to_tensor([1.0], dtype=tf.float32),
        )
    print(mel_outputs.shape)
    if mel_outputs_all is None:
        mel_outputs_all = mel_outputs
    else:
        mel_outputs_all = np.concatenate((mel_outputs_all, mel_outputs), axis=1)

#print(mel_outputs_all.shape)
toc = time()
print("Feature generation took: %fs" % (toc - tic))

print("Synthesize ...")
tic = time()
audio_all = Vocoder.inference(mel_outputs_all)[0, :, 0].numpy()
toc = time()
print("Synthesization took: %fs" % (toc - tic))

#print(audio.shape)
#audio_all = np.hstack((audio_all, audio))#, axis=0)

#print(audio_all.shape)
#toc = time()
#print("Processing took: %fs" % (toc - tic))
#return audio_all#.numpy()

# Outputs
print("Save audio ...")
tic = time()
sf.write(Output_Audio_Path, audio_all, 22050)
toc = time()
print("Saving took: %fs" % (toc - tic))

toc_all = time()
print("All took: %f" % (toc_all - tic_all))

input()#"Enter to end"