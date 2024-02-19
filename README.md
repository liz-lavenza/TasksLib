# TasksLib
Adds a basic framework for creating modded tasks in Yandere Simulator using MelonLoader mods. Allows overriding a student's existing task, for those with one, or giving students with generic tasks their own task. Modded tasks can run custom C# code on task activation, task completion, and when checking if the task can be turned in. Currently has support for custom task icons and subtitles, with support for accompanying voicelines planned in the future. For an example of how to create a task with TasksLib, see [ExampleTask](https://github.com/liz-lavenza/ExampleTask).

# Installation
First, make sure you have MelonLoader installed for Yandere Simulator. If you also use PoseMod, you'll need to install the [PoseMelon](https://github.com/liz-lavenza/PoseMelon) plugin so that it doesn't break MelonLoader.
Then, download `TaskMod.dll` from the latest release on the Releases page (available in the right sidebar) and place it in the `Mods` folder in the Yandere Simulator directory.
***If you are not a developer, you do not need to download `TaskMod.pdb`.***