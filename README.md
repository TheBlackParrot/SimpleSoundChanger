# SimpleSoundChanger
Change some of Beat Saber's sound effects by picking random audio files in pre-determined folders.

## Configuration
No configuration needed, the mod looks for OGG files in the following folders:
- `UserData/SimpleSoundChanger/clicks`
  - UI click noises
- `UserData/SimpleSoundChanger/hits`
  - Note cut sounds
- `UserData/SimpleSoundChanger/misses`
  - Bad cut sounds

The default background menu music will also be replaced if `UserData/SimpleSoundChanger/menu.ogg` is present.

Defaults are used if any folder/file is missing, you can safely exclude anything you don't want to replace.

I've provided the sounds I use *(all of which are my own creation)* [here](https://github.com/TheBlackParrot/SimpleSoundChanger/releases/download/0.0.0/sounds.zip).