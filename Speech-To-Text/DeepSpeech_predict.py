#!/usr/bin/env python
# -*- coding: utf-8 -*-

import argparse
import numpy as np
import sys
import json
import librosa

from deepspeech import Model
from timeit import default_timer as timer

def words_from_candidate_transcript(metadata):
    word = ""
    word_list = []
    word_start_time = 0
    # Loop through each character
    for i, token in enumerate(metadata.tokens):
        # Append character to word if it's not a space
        if token.text != " ":
            if len(word) == 0:
                # Log the start time of the new word
                word_start_time = token.start_time

            word = word + token.text
        # Word boundary is either a space or the last character in the array
        if token.text == " " or i == len(metadata.tokens) - 1:
            word_duration = token.start_time - word_start_time

            if word_duration < 0:
                word_duration = 0

            each_word = dict()
            each_word["word"] = word
            each_word["start_time"] = round(word_start_time, 4)
            each_word["duration"] = round(word_duration, 4)

            word_list.append(each_word)
            # Reset
            word = ""
            word_start_time = 0

    return word_list

def metadata_json_output(metadata):
    json_result = dict()
    json_result["transcripts"] = [{
        "confidence": transcript.confidence,
        "words": words_from_candidate_transcript(transcript),
    } for transcript in metadata.transcripts]
    return json.dumps(json_result, indent=2)

def main():
    parser = argparse.ArgumentParser(description='Running DeepSpeech inference.')
    parser.add_argument('--beam_width', type=int,
                        help='Beam width for the CTC decoder')
    parser.add_argument('--lm_alpha', type=float,
                        help='Language model weight (lm_alpha). If not specified, use default from the scorer package.')
    parser.add_argument('--lm_beta', type=float,
                        help='Word insertion bonus (lm_beta). If not specified, use default from the scorer package.')
    parser.add_argument('--hot_words', type=str,
                        help='Hot-words and their boosts.')
    args = parser.parse_args()
    
    args.model = 'weights/deepspeech-0.9.3-models.pbmm'
    args.scorer = 'weights/deepspeech-0.9.3-models.scorer'
    args.audio = 'data/Biden_short.wav'#'8455-210777-0068.wav'

    print('Loading model from file {}'.format(args.model), file=sys.stderr)
    model_load_start = timer()
    # sphinx-doc: python_ref_model_start
    ds = Model(args.model)
    # sphinx-doc: python_ref_model_stop
    model_load_end = timer() - model_load_start
    print('Loaded model in {:.3}s.'.format(model_load_end), file=sys.stderr)

    if args.beam_width:
        #pass
        ds.setBeamWidth(args.beam_width)

    desired_sample_rate = 16000#ds.sampleRate()

    if args.scorer:
        print('Loading scorer from files {}'.format(args.scorer), file=sys.stderr)
        scorer_load_start = timer()
        ds.enableExternalScorer(args.scorer)
        scorer_load_end = timer() - scorer_load_start
        print('Loaded scorer in {:.3}s.'.format(scorer_load_end), file=sys.stderr)

        if args.lm_alpha and args.lm_beta:
            #pass
            ds.setScorerAlphaBeta(args.lm_alpha, args.lm_beta)

    if args.hot_words:
        print('Adding hot-words', file=sys.stderr)
        for word_boost in args.hot_words.split(','):
            word,boost = word_boost.split(':')
            ds.addHotWord(word,float(boost))
    
    # Load audio as np.float32, range [-1 ... 1]
    audio, _ = librosa.core.load(args.audio)#, sr=desired_sample_rate)
    # Extend range to int16 [-32767 ... 32767]
    audio = audio * 32767
    # Convert np.float32 to np.int16
    audio = audio.astype(np.int16)

    audio_length = len(audio) / desired_sample_rate

    print('Running inference.', file=sys.stderr)
    inference_start = timer()
    print(ds.stt(audio))
    # sphinx-doc: python_ref_inference_stop
    inference_end = timer() - inference_start
    print('Inference took %0.3fs for %0.3fs audio file.' % (inference_end, audio_length), file=sys.stderr)

if __name__ == '__main__':
    main()
