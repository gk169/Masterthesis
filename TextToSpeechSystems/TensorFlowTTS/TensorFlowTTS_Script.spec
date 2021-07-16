# -*- mode: python ; coding: utf-8 -*-

block_cipher = None


a = Analysis(['TensorFlowTTS_Script.py'],
             pathex=['E:\\206309_Gann_Kevin\\git\\Text-To-Speech\\TensorFlowTTS'],
             binaries=[],
             datas=[
			 ('C:\\ProgramData\\Anaconda3\\envs\\TensorFlowTTS\\Lib\\site-packages\\jamo', 'jamo\\'),
			 ('C:\\ProgramData\\Anaconda3\\envs\\TensorFlowTTS\\Lib\\site-packages\\librosa', 'librosa\\'),
			 ('C:\\ProgramData\\Anaconda3\\envs\\TensorFlowTTS\\Lib\\site-packages\\sklearn', 'sklearn\\'),
			 ('C:\\ProgramData\\Anaconda3\\envs\\TensorFlowTTS\\Lib\\site-packages\\g2p_en', 'g2p_en\\'),
			 ],
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
          [],
          exclude_binaries=True,
          name='TensorFlowTTS_Script',
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
               name='TensorFlowTTS_Script')
