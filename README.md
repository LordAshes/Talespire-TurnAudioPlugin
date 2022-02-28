# Turn Audio Plugin

This unofficial TaleSpire mod that plays a sound when the initiative turn changes. Supports always playing a sound
or just when your own mini is up. 

## Change Log

```
1.0.0: Initial release
```

## Install

Use R2ModMan or similar installer to install.

## Usage

Configure R2ModMan configuration for this plugin to determine if you want global desitribution or not. When true,
a sound will play each time the initiative turn changes. If false, a sound will play only when the turn changes to
a mini under the player's control.

The plugin will first try to see if there is a mini specific sound by looking for a file in matching the mini name
in an Audio folder. For example, ``Audio/Jon.wav``. If found, this is the audio that is played.

If not found, the plugin will look for a default audio in the Audio folder. For example, ``Audio/Jon.wav``.
If found, this audio will be played for any mini that does not have a mini specific file.

Note: The Audio folder must be in a File Access Plugin friendly location (typically CustomData folder of a plugin
or the TaleSpire_CustomData common folder).
