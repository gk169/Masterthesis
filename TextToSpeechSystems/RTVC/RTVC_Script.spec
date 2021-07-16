# -*- mode: python ; coding: utf-8 -*-

block_cipher = None


a = Analysis(['RTVC_Script.py'],
             pathex=['E:\\206309_Gann_Kevin\\git\\Text-To-Speech\\RTVC'],
             binaries=[
			 ('C:\\ProgramData\\Anaconda3\\envs\\RTVC\\Lib\\site-packages\\_webrtcvad.cp36-win_amd64.pyd', '.'),
			 ('C:\\ProgramData\\Anaconda3\\envs\\RTVC\\Lib\\site-packages\\webrtcvad.py', '.'),
			 ],
             datas=[
			 ('C:\\ProgramData\\Anaconda3\\envs\\RTVC\\Lib\\site-packages\\numpy', 'numpy\\'),
			 ('C:\\ProgramData\\Anaconda3\\envs\\RTVC\\Lib\\site-packages\\scipy', 'scipy\\'),
			 ('C:\\ProgramData\\Anaconda3\\envs\\RTVC\\Lib\\site-packages\\librosa', 'librosa\\'),
			 ('C:\\ProgramData\\Anaconda3\\envs\\RTVC\\Lib\\site-packages\\sklearn', 'sklearn\\'),
			 ('C:\\ProgramData\\Anaconda3\\envs\\RTVC\\Lib\\site-packages\\webrtcvad_wheels-2.0.10.post2.dist-info', 'webrtcvad_wheels-2.0.10.post2.dist-info'),
			 ('E:\\206309_Gann_Kevin\\git\\Text-To-Speech\\RTVC\\RealTimeVoiceCloning\\synthesizer', 'synthesizer\\'),
			 ('E:\\206309_Gann_Kevin\\git\\Text-To-Speech\\RTVC\\RealTimeVoiceCloning\\vocoder', 'vocoder\\'),
			 ('E:\\206309_Gann_Kevin\\git\\Text-To-Speech\\RTVC\\RealTimeVoiceCloning\\encoder', 'encoder\\'),
			 ],
             hiddenimports=['librosa', 'unidecode', 'inflect'],
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
          [],
          exclude_binaries=True,
          name='RTVC_Script',
          debug=False,
          bootloader_ignore_signals=False,
          strip=False,
          upx=True,
          console=True )
coll = COLLECT(exe,
               a.binaries,
               a.zipfiles,
               a.datas,
               strip=False,
               upx=True,
               upx_exclude=[],
               name='RTVC_Script')
