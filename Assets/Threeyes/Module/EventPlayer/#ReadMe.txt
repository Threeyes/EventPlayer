___________________________________

		 	 EventPlayer
			 by Threeyes
___________________________________

	
* Add/Remove extern support?
	Open Window/Threeyes/EventPlayer Setting, select the plugin you need to use, then click 'Apply'.
	
* How to test the Example?
	Select the extern support as mention above, then add all demo scenes to the Build Setting, then hit play the first scene! You can see the explanation from [Remarker] Component.


* Support platform?
	Since this implement only use the Unity Buildin EventSystem, and doesn't use Reflection feature at runtime, it may support as many platform as possible.

* Your unity version is 2018.4 or higher and want to use Timeline?
	Because the new version of Unity has remove Timeline by default, you need to download Timeline from package Manager, and you are good to go. (If you are using asmdef, you will also need to reference the relate file: https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html )

---------------------------   
      Version History      
---------------------------  
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
1.We use EventPlayerClip's [IsFlip] property to change Clip's output process instead of TimelineEventPlayer's [IsReverse] property, be carefully if you use this feature before.
2.Now the Label in Hierarchy can show more details about EPs.
3.More improvement for CoroutineEventPlayerBase's subclass, check the "3 Coroutine" scene.
4.This version has lot's of changes, Please backup your work first!

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