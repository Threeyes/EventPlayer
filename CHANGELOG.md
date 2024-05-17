# Changelog

## [4.0.1]
- Reduce dependencies on EP: Move CoroutineManager module to Core Library, move Timeline module to EventPlayer Library. If you have previously used components from these modules, you may need to change the referenced namespace. If you find duplicate code errors, please delete and reimport the entire Threeyes folder.

## [4.0.0]
- Add Asmdef for each library.
- Optimize Sequence related logic.
- Provide basic Networking support.
- Fix error caused by EventPlayer attempting to detect or version when opening project

## [3.0.0]
### Incompatibility Warning! Please backup your project before update to this version!Some class's name are changed to ensure consistency:
- Eventplayer with param, such as: IntEventPlayer→EventPlayer_Int, TimelineEventPlayer→EventPlayer_PlayableInfo
- Sequence, such as：EventPlayerSequence→Sequence_EventPlayer

	
## [2.8.0]
### Warning: To make the Timeline Plugin more felxible, I had remove TLEPListener_BezierWalker's [Go Target] property and replace it with EventPlayerTrack's [Binding] and EventPlayerClip's [BindingOverride]. If you had update this plugin from V2.7, Please set the binding before you play the Timeline, or else it will set the BezierWalkerWithTime's GameObject as default Target!
- Add [Binding] property for EventPlayerTrack and EventPlayerClip, so we can override the target inside Timeline.
- Add DoTweenPro support.

## [2.7.0]
- Fix: Manual Invoke EventPlayer.PlayWithID cause Endless loop Error
- Improve EventPlayerSequence's Hierarchy GUI Info.
- Add BezierSolution support (Thanks to yasirkula again!).


## [2.6.0]
- Fix Time Format display error
- Fix reverse Play/Stop bug
- Improve VideoEventPlayer Component

## [2.5.0]
- Auto Detect and update EventPlayerSetting for newer version.

## [2.4.0]
- Fix Error on build.

## [2.3.0]
- Update Demo
- Add Hierarchy Display for EventPlayerSequence

## [2.2.0]
- Fix Error on build.

## [2.1.0]
- Fix DelayEventPlayer's 'delay' problem
- Add EventPlayerSequence Component
- Add VideoEventPlayer Component
- Provide "EventPlayer Setting" Window to config Extern plugin support.


## [2.0.0]
### Warning：We use EventPlayerClip's [IsFlip] property to change Clip's output process instead of TimelineEventPlayer's [IsReverse] property, be carefully if you use this feature before.
- Now the Label in Hierarchy can show more details about EPs.
- More improvement for CoroutineEventPlayerBase's subclass, check the "3 Coroutine" scene.
- This version has lot's of changes, Please backup your work first!

## [1.7.0]
Cheers! Now that every component that inherit from EventPlayer can be  a 'Group' when the IsGroup property is checked.(EventPlayerGroup, you are fired!) Also, the EditorLabel on the Hierarchy will be Trim when there is not enough space.

## [1.6.0]
- Now you can directly preview the EventPlayer reference of EventPlayerClip in Inspector! I promise that will save you tons of time! (Thanks to the Programmers of Cinemachine! )

## [1.5.0]
- Fix bug
- EditorGUI improment.

## [1.4.0]
- I am so sorry for the inconvenient, but I have to rename RepeatPlayer.duration to RepeatPlayer.defaultDuration,
you may need to reassign this property, hope that don't bring you a lot of trouble.
- Add Extra EditorGUI for TimelineEventPlayer, now it can show the playable info such as time or duration.

## [1.3.0]
- Add Extra EditorGUI for EventPlayer, easier to modify property without using Inspector Window

## [1.2.0]
- Small UI improvement
- Replace System.Action with UnityAction

## [1.1.0]
- Fix bug

## [1.0.0]
- First release.