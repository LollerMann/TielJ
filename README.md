# TielJ      // WARNING // WORK IN PROGRESS // WARNING //
TielJ Is a bot that utilizes youtube-dl to stream content to overwatch games.
It uses Virtual Audio Cable to play content through microphone in-game. It also has in-game voting for the next selected content
# Requirements
You neeed to have:
* Visual Studio with C# development pack  => compiling desktop backend
* Virtual Audio Cable =>  streaming content to game
* youtube-dl => grabbing content urls
* https://github.com/ItsDeltin/Overwatch-Script-To-Workshop for compiling latest TielJ workshop code. You can either download compiler and drag the main script there or install it's extension to your Visual Studio Code installation to generate an usable workshop code
* In-game settings set to 1080p Fullscreen => Sorry, TielJ is still yet to support different set ups.

# How do I use it?
* Set Virtual Audio Cable's microphone as your default communication device (or directly default device) and configure your game to use it.
* Apply skirmish preset on your custom game, generate a working workshop code, paste it into workshop section and start your game.
* Compile and launch your desktop backend with url of the youtube content you wish to start with, ex. `TielJ.exe https://www.youtube.com/watch?v=SuperCoolVidId`
* When informed, don't touch anything. You might interfere with data building process which WILL require restarting both backend and your game server.

# How does it work?
Workshop script sets colored hud texts visible only to host player which the backend makes use of. Depending on what host player's state is, backend may use **PRIMARY FIRE** and **SECONDARY FIRE** to input binary string to game, **INTERACT** and **ULTIMATE** keys to switch between modes or confirm the input.
Entered binary text and numbers are rebuilt in workshop script to be used.

After content infos are submitted listeners on the match can use their **CROUCH** + **INTERACT** key combo to enter a voting state which renders them immune to attacks and disables every ability and movement.
Voters can use their **PRIMARY FIRE** and **SECONDARY FIRE** keys to cycle between options and **ULTIMATE** keybind to confirm their selection

When playing content reaches it's end, winning option gets selected and Host Player enters "Hostage" state. During this state, backend reads the result and loads the selected content to be streamed and the process repeats itself until host quits.

# How do I customize the key bindings backend uses?
Head over to `OWStuff.cs` file and find the private `KeyMap` dictionary (should be around line 177) and work your way there.

# Bugs I encountered during my limited testing
- [ ] Youtube randomly gives an improper response and as a result backend literally dies (Best I can do is try until it works) | (NEEDS TESTING)
- [ ] During binary conversion in workshop code, content's length is set to value a workshop variable can hold max. Resulting in infinite playtime.
- [ ] During buffering, a race condition arises so a considerable portion of content gets skipped (using multi threading might fix this) (NEEDS TESTING)
- [ ] Some content's length might be less than needed to input a complete array of music infos. (Not selecting those videos for input or not pussying out when sending input might fix this issue)

# TODO
- [ ] Ditch primary fire/secondary fire input style and find some way to move cursor in-game to send ASCII character value
