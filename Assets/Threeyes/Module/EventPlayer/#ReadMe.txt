___________________________________

		 	 EventPlayer
			 by Threeyes
___________________________________

	
* Add/Remove extern support?
	Open Tools/Threeyes/EventPlayer Setting, select the plugin you need to use, then click 'Apply'. Please ensure that you have those plugin and link to their .asmdef file (if exists) correctly.
	
* How to test the scenes in "Samples" folder?
	Select the extern support as mention above, then add all demo scenes to the Build Setting, then hit play the first scene! You can see the explanation from [Remarker] Component.

* Same file conflict after update to newer version?
	I might move/rename the folder, please delete the old plugin and reimport the latest plugin (always backup your project first!).

* Support platform?
	Since this implement only use the Unity Buildin EventSystem, and doesn't use Reflection feature at runtime, it may support as many platform as possible.

* Your unity version is 2018.4 or higher and want to use Timeline?
	Because the new version of Unity has remove Timeline by default, you need to download Timeline from package Manager, and you are good to go. (If you are using asmdef, you will also need to reference the relate file: https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html )

* Sample's material become purple on URP?
	Select alll materials assets in Materials folder, then select Edit/Rendering/Materials/Convert selected Built-in Materials to URP
---------------------------   
      Version History      
--------------------------- 
-3.0
##Incompatibility Warning£¡ Please backup your project before update to this version!Some class's name are changed to ensure consistency:
	-Eventplayer with param, such as: IntEventPlayer¡úEventPlayer_Int, TimelineEventPlayer¡úEventPlayer_PlayableInfo
	-Sequence, such as£ºEventPlayerSequence¡úSequence_EventPlayer

	
-2.8
##Warning: To make the Timeline Plugin more felxible, I had remove TLEPListener_BezierWalker's [Go Target] property and replace it with EventPlayerTrack's [Binding] and EventPlayerClip's [BindingOverride]. If you had update this plugin from V2.7, Please set the binding before you play the Timeline, or else it will set the BezierWalkerWithTime's GameObject as default Target!
1.Add  [Binding] property for EventPlayerTrack and EventPlayerClip, so we can override the target inside Timeline.
2.Add DoTweenPro support.

-2.7
1.Fix: Manual Invoke EventPlayer.PlayWithID cause Endless loop Error
2.Improve EventPlayerSequence's Hierarchy GUI Info.
3.Add BezierSolution support (Thanks to yasirkula again!).


-2.6
1.Fix Time Format display error
2.Fix reverse Play/Stop bug
3.Improve VideoEventPlayer Component

-2.5
1.Auto Detect and update EventPlayerSetting for newer version 

-2.4
1.Fix Error on build.

-2.3
1.Update Demo
2.Add Hierarchy Display for EventPlayerSequence

-2.2
1.Fix Error on build.

-2.1
1.Fix DelayEventPlayer's 'delay' problem
2.Add EventPlayerSequence Component
3.Add VideoEventPlayer Component
4.Provide "EventPlayer Setting" Window to config Extern plugin support.


-2.0
##Warning£ºWe use EventPlayerClip's [IsFlip] property to change Clip's output process instead of TimelineEventPlayer's [IsReverse] property, be carefully if you use this feature before.
1.Now the Label in Hierarchy can show more details about EPs.
2.More improvement for CoroutineEventPlayerBase's subclass, check the "3 Coroutine" scene.
3.This version has lot's of changes, Please backup your work first!

-1.7
Cheers! Now that every component that inherit from EventPlayer can be  a 'Group' when  the IsGroup property is checked.(EventPlayerGroup, you are fired!)
Also, the EditorLabel on the Hierarchy will be Trim when there is not enough space.

-1.6
Now you can directly preview the EventPlayer reference of EventPlayerClip in Inspector! I promise that will save you tons of time! (Thanks to the Programmers of Cinemachine! )

-1.5
Fix bug, also small EditorGUI improment.

-1.4
1. I am so sorry for the inconvenient, but I have to rename RepeatPlayer.duration to RepeatPlayer.defaultDuration,
you may need to reassign this property, hope that don't bring you a lot of trouble.
2.Add Extra EditorGUI for TimelineEventPlayer, now it can show the playable info such as time or duration.

-1.3
1. Add Extra EditorGUI for EventPlayer, easier to modify property without using Inspector Window

- 1.2
1. Small UI improvement
2. Replace System.Action with UnityAction

- 1.1
Fix bug

- 1.0
First release