# -*- coding: utf-8 -*-
"""
Created on Thu Feb 25 18:07:14 2021

@author: AI-Student
"""

import ffmpeg

Stream = ffmpeg.input('p240_00000.mp3')
Stream = ffmpeg.output(Stream, 'p240_00000.wav')
Stream.run(overwrite_output = True)

(
    ffmpeg
    .input('p240_00000.mp3')
    .hflip()
    .output('p240_00000.wav')
    .run()
)