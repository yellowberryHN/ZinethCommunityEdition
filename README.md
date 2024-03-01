# Zineth Community Edition

Zineth Community Edition is a fan-made mod for [Zineth](https://zinethgame-blog.tumblr.com),
a game by [Arcane Kids](https://arcanekids.com). it aims to expand upon the ideas present in the
original game, add new features, complete unfinished features, and polish the gameplay experience,
all while staying true to the games original aesthetics.

## Features

- Trail and robot color saving

- Restored multiplayer functionality

- Restored update checking functionality

- Players appear on the radar in multiplayer

- Additional phone game monster names

- Hotkey for returning back to the main menu

- ***A speedrunning mode***
    - Built-in speedrunning timer
    - Timer auto-stop when reaching specific goals
    - Hotkey for restarting run
    - Hotkey for stopping timer

- Removed broken Twitter integration (it will be missed...)

- Improvements to custom map functionality

## Download

Head over to the
[latest release](https://github.com/yellowberryHN/ZinethCommunityEdition/releases/latest)
and download the ZinethCE_v0_XX.zip file, extract and run!

## Custom Maps

If you're interested in making custom maps, **please check out the [Zineth Mapping Kit](https://github.com/yellowberryHN/ZinethMappingKit)!**

## Technical Details

Since the original project files are not available, I've had to make due with what I have.

This project hinges on a few things:
- The decompiled source of all of the Unity Assemblies for the game
- Hex edited asset files
- Mud and sticks

As asset hex edits are involved, I've just included the entire game in the `dist` folder. The 
post-build tasks automatically copy the new version of the assemblies to the game folder, so
you can test your changes by building and running the game from inside the `dist`

Right now, due to the fragile nature of the build process, assembly binaries are included
with the commit history and are required to be able to build. Hoping to change that soon.

In the future, I'd like to attempt to rebuild the project as closely as possible,
as that will make things a lot easier, but that'll be a project down the road.

## Contributing

As this is a decompilation, the code is a bit of a mess,
so ideally, don't make it more of a mess!

If you would like to contribute, please check out the issues page,
see if there's anything you'd like to tackle in there! If the thing you'd like to
work on doesn't have an issue yet, add one!

## Note

This project could not have been possible without the efforts of the Arcane Kids, thank you!
This has not been officially sanctioned by them, and it almost feels out of character
to ask for permission, for some reason I just can't put my finger on.

That being said, if one of you discover this project and like it, I'd love to hear about it!

Please reach out to me on [twitter](https://twitter.com/yellowberry__) or [any of my other lines](/)!