# -*- mode: python ; coding: utf-8 -*-

block_cipher = None


a = Analysis(['DeepSpeech_Script.py'],
             pathex=['E:\\206309_Gann_Kevin\\git\\Speech-To-Text\\DeepSpeech'],
             binaries=[],
             datas=[ ('C:\\ProgramData\\Anaconda3\\envs\\DeepSpeech\\Lib\\site-packages\\numpy\\', 'numpy\\'), ('C:\\ProgramData\\Anaconda3\\envs\\DeepSpeech\\Lib\\site-packages\\deepspeech', 'deepspeech'), ('C:\\ProgramData\\Anaconda3\\envs\\DeepSpeech\\Lib\\site-packages\\librosa', 'librosa'), ('C:\\ProgramData\\Anaconda3\\envs\\DeepSpeech\\Lib\\site-packages\\scipy', 'scipy'), ('C:\\ProgramData\\Anaconda3\\envs\\DeepSpeech\\Lib\\site-packages\\sklearn', 'sklearn') ],
             hiddenimports=[],
             hookspath=[],
             runtime_hooks=[],
             excludes=[],
             win_no_prefer_redirects=False,
             win_private_assemblies=False,
             cipher=block_cipher,
             noarchive=False)
pyz = PYZ(a.pure, a.zipped_data,
             cipher=block_cipher)
exe = EXE(pyz,
          a.scripts,
          a.binaries,
          a.zipfiles,
          a.datas,
          [],
          name='DeepSpeech_Script',
          debug=False,
          bootloader_ignore_signals=False,
          strip=False,
          upx=True,
          upx_exclude=[],
          runtime_tmpdir=None,
          console=True )
