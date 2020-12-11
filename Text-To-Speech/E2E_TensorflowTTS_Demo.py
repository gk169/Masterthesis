#!/usr/bin/env python
# coding: utf-8

# # TensorflowTTS real time E2E-TTS demonstration
# 
# This notebook provides a demonstration of the realtime E2E-TTS using TensorflowTTS
# 
# - Github: https://github.com/TensorSpeech/TensorflowTTS
# - Audio samples: https://tensorspeech.github.io/TensorflowTTS/
# - Korean Colab: https://colab.research.google.com/drive/1ybWwOS5tipgPFttNulp77P6DAB5MtiuN?usp=sharing

# ## Install

# In[1]:


import os
get_ipython().system('git clone https://github.com/TensorSpeech/TensorFlowTTS')
os.chdir("TensorFlowTTS")
get_ipython().system('pip install  .')
os.chdir("..")
import sys
sys.path.append("TensorFlowTTS/")


# ## Download pretrained feature generation model
# 
# You can select one from two models. Please only run the seletected model cells.
# 

# ### (a) Tacotron-2

# In[ ]:


print("Downloading Tacotron2 model...")
get_ipython().system('gdown --id {"12jvEO1VqFo1ocrgY9GUHF_kVcLn3QaGW"} -O tacotron2-120k.h5')
get_ipython().system('gdown --id {"1OI86hkN1YCpHBsIKnkELNbSho5Pj-pPY"} -O tacotron2_config.yml')


# ### (b) FastSpeech

# In[ ]:


print("Downloading FastSpeech model...")
get_ipython().system('gdown --id {"1T5GOE_M27zJlCAjnanpOS9HBPUcdE9sB"} -O fastspeech-150k.h5')
get_ipython().system('gdown --id {"1TnkL2-rIZ6N-n4z4oHp3X2wIpxiFwu2H"} -O fastspeech_config.yml')


# ### (c) FastSpeech2

# In[2]:


print("Downloading FastSpeech2 model...")
get_ipython().system('gdown --id {"1EhMD20uAFlKsii1lMnlkrsenVTFKM0ld"} -O fastspeech2-150k.h5')
get_ipython().system('gdown --id {"1wnbIgjTI2iUsCyVJ37ar9CS8-aEjVEee"} -O fastspeech2_config.yml')


# ## Download pretrained Vocoder model
# 
# You can select one from two models. Please only run the seletected model cells.
# 

# ### (a) MelGAN Original

# In[ ]:


print("Downloading MelGAN model...")
get_ipython().system('gdown --id {"1A3zJwzlXEpu_jHeatlMdyPGjn1V7-9iG"} -O melgan-1M6.h5')
get_ipython().system('gdown --id {"1Ys-twSd3m2uqhJOEiobNox6RNQf4txZs"} -O melgan_config.yml')


# ### (b) MelGAN + STFT Loss

# In[3]:


print("Downloading MelGAN-STFT model...")
get_ipython().system('gdown --id {"1WB5iQbk9qB-Y-wO8BU6S2TnRiu4VU5ys"} -O melgan.stft-2M.h5')
get_ipython().system('gdown --id {"1OqdrcHJvtXwNasEZP7KXZwtGUDXMKNkg"} -O melgan.stft_config.yml')


# ### (c) Multi-band MelGAN

# In[ ]:


print("Downloading Multi-band MelGAN model...")
get_ipython().system('gdown --id {"1kChFaLI7slrTtuk3pvcOiJwJDCygsw9C"} -O mb.melgan-940k.h5')
get_ipython().system('gdown --id {"1YC_kZpuRZGQ-JfMKj1LC0YRyKXsgLTJL"} -O mb.melgan_config.yml')


# ## Load Model

# In[4]:


import tensorflow as tf

import yaml
import numpy as np
import matplotlib.pyplot as plt

import IPython.display as ipd

from tensorflow_tts.inference import TFAutoModel
from tensorflow_tts.inference import AutoConfig
from tensorflow_tts.inference import AutoProcessor


# ### (a) Tacotron 2

# In[ ]:


tacotron2_config = AutoConfig.from_pretrained('TensorFlowTTS/examples/tacotron2/conf/tacotron2.v1.yaml')
tacotron2 = TFAutoModel.from_pretrained(
    config=tacotron2_config,
    pretrained_path="tacotron2-120k.h5",
    name="tacotron2"
)


# ### (b) FastSpeech

# In[ ]:


fastspeech_config = AutoConfig.from_pretrained('TensorFlowTTS/examples/fastspeech/conf/fastspeech.v1.yaml')
fastspeech = TFAutoModel.from_pretrained(
    config=fastspeech_config,
    pretrained_path="fastspeech-150k.h5",
    name="fastspeech"
)


# ### (c) FastSpeech2

# In[5]:


fastspeech2_config = AutoConfig.from_pretrained('TensorFlowTTS/examples/fastspeech2/conf/fastspeech2.v1.yaml')
fastspeech2 = TFAutoModel.from_pretrained(
    config=fastspeech2_config,
    pretrained_path="fastspeech2-150k.h5",
    name="fastspeech2"
)


# ### (d) MelGAN Original

# In[ ]:


melgan_config = AutoConfig.from_pretrained('TensorFlowTTS/examples/melgan/conf/melgan.v1.yaml')
melgan = TFAutoModel.from_pretrained(
    config=melgan_config,
    pretrained_path="melgan-1M6.h5",
    name="melgan"
)


# ### (e) MelGAN STFT

# In[7]:


melgan_stft_config = AutoConfig.from_pretrained('TensorFlowTTS/examples/melgan_stft/conf/melgan_stft.v1.yaml')
melgan_stft = TFAutoModel.from_pretrained(
    config=melgan_stft_config,
    pretrained_path="melgan.stft-2M.h5",
    name="melgan_stft"
)


# ### (f) Multi-band MelGAN

# In[6]:


mb_melgan_config = AutoConfig.from_pretrained('TensorFlowTTS/examples/multiband_melgan/conf/multiband_melgan.v1.yaml')
mb_melgan = TFAutoModel.from_pretrained(
    config=mb_melgan_config,
    pretrained_path="mb.melgan-940k.h5",
    name="mb_melgan"
)


# ## Inference
# - The first time model run inference will very slow cause by @tf.function.

# In[8]:


print("Downloading ljspeech_mapper.json ...")
get_ipython().system('gdown --id {"1YBaDdMlhTXxsKrH7mZwDu-2aODq5fr5e"} -O ljspeech_mapper.json')


# In[9]:


processor = AutoProcessor.from_pretrained(pretrained_path="./ljspeech_mapper.json")


# In[10]:


def do_synthesis(input_text, text2mel_model, vocoder_model, text2mel_name, vocoder_name):
  input_ids = processor.text_to_sequence(input_text)

  # text2mel part
  if text2mel_name == "TACOTRON":
    _, mel_outputs, stop_token_prediction, alignment_history = text2mel_model.inference(
        tf.expand_dims(tf.convert_to_tensor(input_ids, dtype=tf.int32), 0),
        tf.convert_to_tensor([len(input_ids)], tf.int32),
        tf.convert_to_tensor([0], dtype=tf.int32)
    )
  elif text2mel_name == "FASTSPEECH":
    mel_before, mel_outputs, duration_outputs = text2mel_model.inference(
        input_ids=tf.expand_dims(tf.convert_to_tensor(input_ids, dtype=tf.int32), 0),
        speaker_ids=tf.convert_to_tensor([0], dtype=tf.int32),
        speed_ratios=tf.convert_to_tensor([1.0], dtype=tf.float32),
    )
  elif text2mel_name == "FASTSPEECH2":
    mel_before, mel_outputs, duration_outputs, _, _ = text2mel_model.inference(
        tf.expand_dims(tf.convert_to_tensor(input_ids, dtype=tf.int32), 0),
        speaker_ids=tf.convert_to_tensor([0], dtype=tf.int32),
        speed_ratios=tf.convert_to_tensor([1.0], dtype=tf.float32),
        f0_ratios=tf.convert_to_tensor([1.0], dtype=tf.float32),
        energy_ratios=tf.convert_to_tensor([1.0], dtype=tf.float32),
    )
  else:
    raise ValueError("Only TACOTRON, FASTSPEECH, FASTSPEECH2 are supported on text2mel_name")

  # vocoder part
  if vocoder_name == "MELGAN" or vocoder_name == "MELGAN-STFT":
    audio = vocoder_model(mel_outputs)[0, :, 0]
  elif vocoder_name == "MB-MELGAN":
    audio = vocoder_model(mel_outputs)[0, :, 0]
  else:
    raise ValueError("Only MELGAN, MELGAN-STFT and MB_MELGAN are supported on vocoder_name")

  if text2mel_name == "TACOTRON":
    return mel_outputs.numpy(), alignment_history.numpy(), audio.numpy()
  else:
    return mel_outputs.numpy(), audio.numpy()

def visualize_attention(alignment_history):
  import matplotlib.pyplot as plt

  fig = plt.figure(figsize=(8, 6))
  ax = fig.add_subplot(111)
  ax.set_title(f'Alignment steps')
  im = ax.imshow(
      alignment_history,
      aspect='auto',
      origin='lower',
      interpolation='none')
  fig.colorbar(im, ax=ax)
  xlabel = 'Decoder timestep'
  plt.xlabel(xlabel)
  plt.ylabel('Encoder timestep')
  plt.tight_layout()
  plt.show()
  plt.close()

def visualize_mel_spectrogram(mels):
  mels = tf.reshape(mels, [-1, 80]).numpy()
  fig = plt.figure(figsize=(10, 8))
  ax1 = fig.add_subplot(311)
  ax1.set_title(f'Predicted Mel-after-Spectrogram')
  im = ax1.imshow(np.rot90(mels), aspect='auto', interpolation='none')
  fig.colorbar(mappable=im, shrink=0.65, orientation='horizontal', ax=ax1)
  plt.show()
  plt.close()


# In[11]:


input_text = "Bill got in the habit of asking himself “Is that thought true?” And if he wasn’t absolutely certain it was, he just let it go."


# In[ ]:


# setup window for tacotron2 if you want to try
tacotron2.setup_window(win_front=10, win_back=10)


# ### (a) Tacotron2 + MELGAN

# In[ ]:


mels, alignment_history, audios = do_synthesis(input_text, tacotron2, melgan, "TACOTRON", "MELGAN")
visualize_attention(alignment_history[0])
visualize_mel_spectrogram(mels[0])
ipd.Audio(audios, rate=22050)


# ### (b) Tacotron2 + MELGAN-STFT

# In[ ]:


mels, alignment_history, audios = do_synthesis(input_text, tacotron2, melgan_stft, "TACOTRON", "MELGAN-STFT")
visualize_attention(alignment_history[0])
visualize_mel_spectrogram(mels[0])
ipd.Audio(audios, rate=22050)


# ### (c) Tacotron2 + MB-MELGAN

# In[ ]:


mels, alignment_history, audios = do_synthesis(input_text, tacotron2, mb_melgan, "TACOTRON", "MB-MELGAN")
visualize_attention(alignment_history[0])
visualize_mel_spectrogram(mels[0])
ipd.Audio(audios, rate=22050)


# ### (d) FastSpeech + MB-MELGAN

# In[ ]:


mels, audios = do_synthesis(input_text, fastspeech, mb_melgan, "FASTSPEECH", "MB-MELGAN")
visualize_mel_spectrogram(mels[0])
ipd.Audio(audios, rate=22050)


# ### (e) FastSpeech + MELGAN-STFT

# In[ ]:


mels, audios = do_synthesis(input_text, fastspeech, melgan_stft, "FASTSPEECH", "MELGAN-STFT")
visualize_mel_spectrogram(mels[0])
ipd.Audio(audios, rate=22050)


# ### (f) FastSpeech + MELGAN

# In[ ]:


mels, audios = do_synthesis(input_text, fastspeech, melgan, "FASTSPEECH", "MELGAN")
visualize_mel_spectrogram(mels[0])
ipd.Audio(audios, rate=22050)


# ### (g) FastSpeech2 + MB-MELGAN

# In[ ]:


mels, audios = do_synthesis(input_text, fastspeech2, mb_melgan, "FASTSPEECH2", "MB-MELGAN")
visualize_mel_spectrogram(mels[0])
ipd.Audio(audios, rate=22050)


# ### (h) FastSpeech2 + MELGAN-STFT

# In[12]:


mels, audios = do_synthesis(input_text, fastspeech2, melgan_stft, "FASTSPEECH2", "MELGAN-STFT")
visualize_mel_spectrogram(mels[0])
ipd.Audio(audios, rate=22050)


# ### (i) FastSpeech2 + MELGAN

# In[ ]:


mels, audios = do_synthesis(input_text, fastspeech2, melgan, "FASTSPEECH2", "MELGAN")
visualize_mel_spectrogram(mels[0])
ipd.Audio(audios, rate=22050)


# In[ ]:




