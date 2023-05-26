using System.Reflection;
using UnityEngine;
using System;
using System.Runtime.Serialization;

/// <summary>
/// Base class for all custom ScriptableObject
/// 
/// PS:
/// 1.非必须继承，这里更多只是作为一个模板，表明ScriptableObject可用的事件回调，有需要的自行继承接口并实现对应方法
/// </summary>
public sealed class SOBase : ScriptableObject
{
    [OnDeserialized]//Get invoked after deserialization.
    internal void OnDeserializedMethod(StreamingContext context)
    {
    }

    #region Builtin callback (Do it yourself!)
    //Detail: https://forum.unity.com/threads/scriptableobject-behaviour-discussion-how-scriptable-objects-work.541212/

    ///// <summary>
    ///// This function is called when the ScriptableObject script is started:
    ///// 1.When the ScriptableObject is created (in editor or at runtime)
    ///// 2.When the ScriptableObject is selected from the project window in the Editor
    ///// 3.When a scene is loaded IF at least one MonoBehaviour in that scene is referencing the ScriptableObject asset
    ///// 
    ///// Warning:ScriptableObjects will only receive Awake() if OnDisable() was previously called on the object. 
    ///// </summary>
    //protected virtual void Awake()
    //{
    //}

    ///// <summary>
    ///// This function is called when the object is loaded:
    ///// 1.Immediately after the ScriptableObject's Awake() (before other callbacks on this or other objects)
    ///// 2.When the Unity Editor reloads IF in a scene that has a MonoBehaviour referencing that ScriptableObject asset (right after OnDisable())
    ///// 3.When entering play mode IF in a scene that has a MonoBehaviour referencing that ScriptableObject asset (right after OnDisable())
    ///// </summary>
    //protected virtual void OnEnable()
    //{
    //}
    ///// <summary>
    ///// This function is called when the scriptable object goes out of scope:
    ///// 1.When a scene is loaded and there are no MonoBehaviours in that scene that reference the ScriptableObject asset
    ///// 2.When the Unity Editor reloads IF in a scene that has a MonoBehaviour referencing that ScriptableObject
    ///// 3.When entering play mode IF in a scene that has a MonoBehaviour referencing that ScriptableObject
    ///// 4.Right before any OnDestroyed() callback
    ///// 
    ///// Warning:OnDisable() will only be called if OnEnable() was previously called on the ScriptableObject.
    ///// </summary>
    //protected virtual void OnDisable()
    //{
    //}
    ///// <summary>
    ///// This function is called when the scriptable object will be destroyed:
    ///// 1.The ScriptableObject is deleted in code
    ///// 2.The ScriptableObject is deleted from the assets folder in the Editor
    ///// 3.The ScriptableObject was created at runtime and the application is quitting(or exiting play mode)
    ///// </summary>
    //protected virtual void OnDestroy()
    //{
    //}
    #endregion
}