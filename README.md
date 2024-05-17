<h1 align="center">EventPlayer</h1>
<p align="center">
    <a href="https://assetstore.unity.com/packages/tools/level-design/bezier-solution-113074"><img src="https://github.com/Threeyes/EventPlayer/wiki/images/Logo.png" alt="Logo" width="300px" height="200px" />
    <br />
	<a><img src="https://img.shields.io/badge/%20Unity-2028.4+%20-blue" /></a>
	<a href="https://github.com/Threeyes/AliveCursorSDK/blob/main/LICENSE"><img src="https://img.shields.io/badge/License-MIT-brightgreen.svg" /></a>
    <br />
</p>

## Language
<p float="left">
  <a href="https://github.com/Threeyes/EventPlayer/blob/main/locale/README-zh-CN.md">中文</a> | 
  <a href="https://github.com/Threeyes/EventPlayer">English</a>
</p>


## Description
I have sick of writing scripts that can't be reuse, so here it is！Visual Scripting without extra editorwindow!

**EventPlayer** extends Unity's built-in Event System,  provide several Components that invoke Event in specific Time, such as Delay, Repeat and CountDown, You can visual reorder, reorganize or deactive events, execute them at desire time.


## Features
- Easy to use, no coding required.
- Visual parenting and organizing Event, manage Event by Group or Sequence, mark them by ID.
- Supports most platforms.
- Support following Plugin:
    - [Timeline](https://docs.unity3d.com/Packages/com.unity.timeline@1.5/manual/index.html): Unity's timeline library.
    - [VideoPlayer](https://docs.unity3d.com/Manual/class-VideoPlayer.html): Unity's built-in video player library.
    - [BezierSolution](https://assetstore.unity.com/packages/tools/level-design/bezier-solution-113074): Runtime bezier splines.
    - [DoTweenPro](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416): Powerful animation tool.
    - (You can also write code to support more plugins...)


## Note
This plugin depends on the **Threeyes Core** library (The AssetStore version already includes this library). If your project does not have this library, please download it from [GitHub](https://github.com/Threeyes/Core).


## Setup
### Add/Remove extern plugin support
Click 'Tools/Threeyes/EventPlayer Setting' menu, select the plugin you need to use, then click 'Apply'. Please ensure that you have install those plugin in project and link to their's asmdef file (if exists) correctly.

## Samples
Select the extern support as mention above, then add all demo scenes in 'Samples' folder to the BuildSettings Windows, then hit play the first scene! You can see the explanation from **Remarker** Component in the Hierarchy window.


## Trouble Shooting
### Some files conflicts after update to newer version?
I might move/rename the folder, please delete the old plugin and reimport the latest plugin (always backup your project first!).
	
### Unity version is 2018.4 or higher and can't find the Timeline plugin?
Because the new version of Unity has remove Timeline by default, you need to download Timeline from package Manager, and you are good to go. If you are using asmdef, you will also need to reference the relate file: https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html

### Why does the code related to DOTweenPro report errors?
This may occur because the library corresponding to DOTweenPro is not properly linked. Firstly, ensure that the DOTweenPro folder contains the 'DOTweenPro. Scripts. asmdef' file, if the file does not exist, you can open its settings window through the menu 'Tools/Demographics/DOTween Utility Panel', and then click 'Create ASMDEF...' to generate it. If there are still errors, it is possible that the meta information of the above generated asmdef file has changed, you need to add that asmdef file to the Threeyes.EventPlayer.asmdef's 'Assembly Definition References' list and click Apply.

### Sample's material become purple on Built-In Pipeline?
Sample include URP material by default. You need to import 'Materials_BuildInRP. unitypackage' from the Sample folder to cover all existing materials.

### Sample's material become purple on HDRP Pipeline?
First, import all materials of Built-In Pipeline as mentioned above, and then select all materials, then click on the menu 'Edit/Rendering/Materials/Convert selected Built-in Materials to HDRP'.


## Q&A
### Support platform?
Since this implement only use the Unity Buildin EventSystem, and doesn't use Reflection feature at runtime, it may support as many platform as possible.

## More Info
[AssetStore Version](https://assetstore.unity.com/packages/tools/visual-scripting/event-player-116731)

[Unity Forum Thread](https://forum.unity.com/threads/release-event-player-visual-play-and-organize-unityevent.536984/)

[Unity Community - Chinese Tutorial](https://developer.unity.cn/projects/603086a7edbc2a00202c3878)