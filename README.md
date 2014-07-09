Arcade-Launcher
===============

A game launcher written in Unity3D for the Arcade Cabinet.  This code base requires the NGUI asset to build properly.  The purpose of the launcher is to facilitate the development of games for the arcade cabinet.  The launcher is designed to read from a shared directory where developers can upload builds and other necessary files without needing to modify the launcher directly.

## Instructions for Developers

You will first need to build an executable artifact for your game.  If you are using Unity3D for example, you will need to target Windows and place both the exe and the data folder into the shared directory.

You will also need to provide a text file (.txt) with json formatted information.  An example of what is needed is below:

```json
{
    "title": "Super Spaceman Shootout",
    "author": "Daniel Fairley",
    "players": 4,
    "executable":"sss.exe"
}
````
The executable is the name of the .exe in the directory.  No directory structure is needed.

Finally you will need to provide a square png image called **card.png** to the same directory.  This will be the cover image that shows when your game is selected.  Your directory should look like this:

![image](http://i.imgur.com/mwYWGjN.png)

If all is done correctly, you should see something like this:

![image](http://i.imgur.com/nc5k2xF.png)
