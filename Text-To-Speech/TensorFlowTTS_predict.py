#!/usr/bin/env python
# coding: utf-8

import tensorflow as tf
from datetime import datetime
import soundfile as sf

from tensorflow_tts.inference import TFAutoModel
from tensorflow_tts.inference import AutoConfig
from tensorflow_tts.inference import AutoProcessor

# for degubbing
import time

class network_info:
    def __init__(self, name, config_path, weight_path):
        self.name = name
        self.config_path = config_path
        self.weight_path = weight_path
        self.config = AutoConfig.from_pretrained(self.config_path)
        self.model = TFAutoModel.from_pretrained(config=self.config, pretrained_path=self.weight_path, name = self.name)

# Text2Mel (feature generation) options
#Text2Mel = network_info('tacotron2', 'configs/tacotron2.v1.yaml', 'weights/feature_generation/tacotron2-120k.h5')
#Text2Mel = network_info('fastspeech1', 'configs/fastspeech1.v1.yaml', 'weights/feature_generation/fastspeech1-150k.h5')
Text2Mel = network_info('fastspeech2', 'configs/fastspeech2.v1.yaml', 'weights/feature_generation/fastspeech2-150k.h5')

# Vocoder options
Vocoder = network_info('melgan', 'configs/melgan.v1.yaml', 'weights/vocoder/melgan-1M6.h5')
#Vocoder = network_info('melgan_stft', 'configs/melgan_stft.v1.yaml', 'weights/vocoder/melgan.stft-2M.h5')
#Vocoder = network_info('mb_melgan', 'configs/multiband_melgan.v1.yaml', 'weights/vocoder/mb.melgan-940k.h5')

input_text = "Bill got in the habit of asking himself “Is that thought true?” And if he wasn’t absolutely certain it was, he just let it go."

# The first time model run inference will very slow cause by @tf.function.
processor = AutoProcessor.from_pretrained(pretrained_path="TensorFlowTTS/test/files/ljspeech_mapper.json")

def do_synthesis(input_text, Text2Mel, Vocoder):
    input_ids = processor.text_to_sequence(input_text)
    
    # text2mel part
    if Text2Mel.name == "tacotron2":
        _, mel_outputs, _, _ = Text2Mel.model.inference(
            tf.expand_dims(tf.convert_to_tensor(input_ids, dtype=tf.int32), 0),
            tf.convert_to_tensor([len(input_ids)], tf.int32),
            tf.convert_to_tensor([0], dtype=tf.int32)
        )
    elif Text2Mel.name == "fastspeech1":
        _, mel_outputs, _ = Text2Mel.model.inference(
            input_ids=tf.expand_dims(tf.convert_to_tensor(input_ids, dtype=tf.int32), 0),
            speaker_ids=tf.convert_to_tensor([0], dtype=tf.int32),
            speed_ratios=tf.convert_to_tensor([1.0], dtype=tf.float32),
        )
    elif Text2Mel.name == "fastspeech2":
        _, mel_outputs, _, _, _ = Text2Mel.model.inference(
            tf.expand_dims(tf.convert_to_tensor(input_ids, dtype=tf.int32), 0),
            speaker_ids=tf.convert_to_tensor([0], dtype=tf.int32),
            speed_ratios=tf.convert_to_tensor([1.0], dtype=tf.float32),
            f0_ratios=tf.convert_to_tensor([1.0], dtype=tf.float32),
            energy_ratios=tf.convert_to_tensor([1.0], dtype=tf.float32),
        )
    
    audio = Vocoder.model(mel_outputs)[0, :, 0]
    
    return mel_outputs.numpy(), audio.numpy()

print('SpeechGeneration Networks: ', Text2Mel.name, ' + ', Vocoder.name)

start = time.time()

_, audio = do_synthesis(input_text, Text2Mel, Vocoder)

end = time.time()

print('Sysnthesis took: {:5.3f}s'.format(end-start))

now = datetime.now()
dt_string = now.strftime("%Y-%m-%d_%H-%M-%S-%f")
Output_Audio_Path = 'data/generated/' + dt_string + '.wav'
sf.write(Output_Audio_Path, audio, 22050)