# -*- mode: python ; coding: utf-8 -*-

block_cipher = None


a = Analysis(['PyTTSx3_Script.py'],
             pathex=['E:\\206309_Gann_Kevin\\git\\Text-To-Speech\\PyTTSx3'],
             binaries=[],
             datas=[
			 ('C:\\ProgramData\\Anaconda3\\envs\\PyTTSx3\\Lib\\site-packages\\pyttsx3', 'pyttsx3\\'),
			 ],
             hiddenimports=['comtypes', 'pythoncom'],
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
          name='PyTTSx3_Script',
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
               name='PyTTSx3_Script')
