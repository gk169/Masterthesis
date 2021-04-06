#!/usr/bin/env python
# coding: utf-8

import tensorflow as tf
import soundfile as sf
import sys

from tensorflow_tts.inference import TFAutoModel
from tensorflow_tts.inference import AutoConfig
from tensorflow_tts.inference import AutoProcessor

# Inputs
input_text = sys.argv[1]                #"text"

Feature_Generation_Name = sys.argv[2]   #'Tacotron2'
Feature_Generation = sys.argv[3]        #tacotron2-120k.h5
Feature_Config = sys.argv[4]            #tacotron2.v1.yaml

Vocoder_Name = sys.argv[5]              #'MB_MelGAN'
Vocoder = sys.argv[6]                   #mb.melgan-940k.h5
Vocoder_Config = sys.argv[7]            #multiband_melgan.v1.yaml

Processor = sys.argv[8]                 #ljspeech_mapper.json

Output_Audio_Path = sys.argv[9]         #.wav

Feature_Config = AutoConfig.from_pretrained(Feature_Config)
Vocoder_Config = AutoConfig.from_pretrained(Vocoder_Config)
Processor = AutoProcessor.from_pretrained(Processor)

# Process

Feature_Generator = TFAutoModel.from_pretrained(config=Feature_Config, pretrained_path=Feature_Generation, name=Feature_Generation_Name)
Vocoder = TFAutoModel.from_pretrained(config=Vocoder_Config, pretrained_path=Vocoder, name=Vocoder_Name)


def do_synthesis(input_text, Text2MelModel, VocoderModel):
    input_ids = Processor.text_to_sequence(input_text)
    
    # text2mel part
    if Text2MelModel.name == "Tacotron2":
        _, mel_outputs, _, _ = Text2MelModel.inference(
            tf.expand_dims(tf.convert_to_tensor(input_ids, dtype=tf.int32), 0),
            tf.convert_to_tensor([len(input_ids)], tf.int32),
            tf.convert_to_tensor([0], dtype=tf.int32)
        )
    elif Text2MelModel.name == "Fastspeech1":
        _, mel_outputs, _ = Text2MelModel.inference(
            input_ids=tf.expand_dims(tf.convert_to_tensor(input_ids, dtype=tf.int32), 0),
            speaker_ids=tf.convert_to_tensor([0], dtype=tf.int32),
            speed_ratios=tf.convert_to_tensor([1.0], dtype=tf.float32),
        )
    elif Text2MelModel.name == "Fastspeech2":
        _, mel_outputs, _, _, _ = Text2MelModel.inference(
            tf.expand_dims(tf.convert_to_tensor(input_ids, dtype=tf.int32), 0),
            speaker_ids=tf.convert_to_tensor([0], dtype=tf.int32),
            speed_ratios=tf.convert_to_tensor([1.0], dtype=tf.float32),
            f0_ratios=tf.convert_to_tensor([1.0], dtype=tf.float32),
            energy_ratios=tf.convert_to_tensor([1.0], dtype=tf.float32),
        )
    
    audio = VocoderModel.inference(mel_outputs)[0, :, 0]
    
    return mel_outputs.numpy(), audio.numpy()

#start = time.time()
_, audio = do_synthesis(input_text, Feature_Generator, Vocoder)
#end = time.time()
#print('Sysnthesis took: {:5.3f}s'.format(end-start))

# Outputs
sf.write(Output_Audio_Path, audio, 22050)