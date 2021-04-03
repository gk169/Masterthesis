#!/usr/bin/env python
# coding: utf-8

import tensorflow as tf
#from datetime import datetime
import soundfile as sf
import sys
#import pathlib

from tensorflow_tts.inference import TFAutoModel
from tensorflow_tts.inference import AutoConfig
from tensorflow_tts.inference import AutoProcessor


#import time

# Common paths
#Common_File_Path = pathlib.Path(__file__).parent.parent.parent.joinpath('weights').joinpath('TensorFlowTTS')
#Common_File_Path_En = Common_File_Path.joinpath('en-US')
#Common_File_Path_De = Common_File_Path.joinpath('de-DE')

# English feature generations
#Feature_Generation_Path_En = Common_File_Path_En.joinpath('Feature_Generation')
#Fastspeech1_Weights_Path_En = str(Feature_Generation_Path_En.joinpath('Fastspeech1').joinpath('fastspeech1-150k.h5'))
#Fastspeech2_Weights_Path_En = str(Feature_Generation_Path_En.joinpath('Fastspeech2').joinpath('fastspeech2-150k.h5'))
#Tacotron2_Weights_Path_En = str(Feature_Generation_Path_En.joinpath('Tacotron2').joinpath('tacotron2-120k.h5'))
Feature_Generation = sys.argv[3]
Feature_Config = sys.argv[4]

# English feature generation configs
#Fastspeech1_Configs_Path_En = str(Feature_Generation_Path_En.joinpath('Fastspeech1').joinpath('fastspeech1.v1.yaml'))
#Fastspeech2_Configs_Path_En = str(Feature_Generation_Path_En.joinpath('Fastspeech2').joinpath('fastspeech2.v1.yaml'))
#Tacotron2_Configs_Path_En = str(Feature_Generation_Path_En.joinpath('Tacotron2').joinpath('tacotron2.v1.yaml'))

# English vocoders
#Vocoder_Path_En = Common_File_Path_En.joinpath('Vocoder')
#MelGAN_Weights_Path_En = str(Vocoder_Path_En.joinpath('MelGAN').joinpath('melgan-1M6.h5'))
#MelGAN_STFT_Weights_Path_En = str(Vocoder_Path_En.joinpath('MelGAN STFT').joinpath('melgan.stft-2M.h5'))
#MB_MelGAN_Weights_Path_En = str(Vocoder_Path_En.joinpath('Multiband MelGAN').joinpath('mb.melgan-940k.h5'))
Vocoder = sys.argv[6]

# English vocoder configs
#MelGAN_Configs_Path_En = str(Vocoder_Path_En.joinpath('MelGAN').joinpath('melgan.v1.yaml'))
#MelGAN_STFT_Configs_Path_En = str(Vocoder_Path_En.joinpath('MelGAN STFT').joinpath('melgan_stft.v1.yaml'))
#MB_MelGAN_Configs_Path_En = str(Vocoder_Path_En.joinpath('Multiband MelGAN').joinpath('multiband_melgan.v1.yaml'))
Vocoder_Config = sys.argv[7]

# English Processor
#Processor_Path_En = Common_File_Path_En.joinpath('ljspeech_mapper.json')
Processor = sys.argv[8]


# German feature generations
#Feature_Generation_Path_De = Common_File_Path_De.joinpath('Feature_Generation')
#Tacotron2_Weights_Path_De = str(Feature_Generation_Path_De.joinpath('Tacotron2').joinpath('thorsten-tacotron2-97554.h5'))

# German feature generation configs
#Tacotron2_Configs_Path_De = str(Feature_Generation_Path_De.joinpath('Tacotron2').joinpath('tacotron2.v1.yaml'))

# German vocoders
#Vocoder_Path_De = Common_File_Path_De.joinpath('Vocoder')
#MB_MelGAN_Weights_Path_De = str(Vocoder_Path_De.joinpath('Multiband MelGAN').joinpath('thorsten-mb_melgan-820533.h5'))

# German vocoder configs
#MB_MelGAN_Configs_Path_De = str(Vocoder_Path_De.joinpath('Multiband MelGAN').joinpath('multiband_melgan.v1.yaml'))

# German Processor
#Processor_Path_De = Common_File_Path_De.joinpath('thorsten_mapper.json')



# Adapt TensorFlowTTS here!

Feature_Config = AutoConfig.from_pretrained(Feature_Config) #Tacotron2_Configs_Path_De)
Vocoder_Config = AutoConfig.from_pretrained(Vocoder_Config) #MB_MelGAN_Configs_Path_De)
Processor = AutoProcessor.from_pretrained(Processor) #Processor_Path_De)
#Feature_Generation_Weights = Tacotron2_Weights_Path_De
#Vocoder_Weights = MB_MelGAN_Weights_Path_De
Feature_Generation_Name = sys.argv[2]#'Tacotron2'
Vocoder_Name = sys.argv[5] #'MB_MelGAN'

# Edit text
input_text = sys.argv[1] #"Bill got in the habit of asking himself “Is that thought true?” And if he wasn’t absolutely certain it was, he just let it go."
#input_text = "Coronavirus verändert zur Zeit das Leben in unserem Land dramatisch"#" unsere Vorstellung von Normalität von öffentlichen Leben von sozialem Miteinander all das wird auf die Probe gestellt wie nie zuvor Millionen von ihnen können nicht zur Arbeit ihre Kinder können nicht zur Schule oder in die Kita Theater und Kinos und Geschäfte sind geschlossen und was vielleicht das Schwerste ist uns allen fehlen die Begegnung die sonst selbst"


# Run TensorFlowTTS

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

#print('SpeechGeneration Networks: ', Feature_Generator.name, ' + ', Vocoder.name)

#start = time.time()

_, audio = do_synthesis(input_text, Feature_Generator, Vocoder)

#end = time.time()

#print('Sysnthesis took: {:5.3f}s'.format(end-start))

#now = datetime.now()
#dt_string = now.strftime("%Y-%m-%d_%H-%M-%S-%f")

Output_Audio_Path = sys.argv[9] 
#str(pathlib.Path(__file__).parent.parent.parent
#                        .joinpath('data')
#                        .joinpath('generated')
#                        .joinpath('audios')
#                        .joinpath('TensorFlowTTS_' + dt_string + '.wav'))

sf.write(Output_Audio_Path, audio, 22050)