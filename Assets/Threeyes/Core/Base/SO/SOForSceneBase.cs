using UnityEngine;
using System.Text;

#if USE_NaughtyAttributes
using NaughtyAttributes;
#endif

/// <summary>
/// SO that interactive with scene object (eg: SOAction)
/// </summary>
public abstract class SOForSceneBase : ScriptableObject
{
#if USE_NaughtyAttributes
    [ResizableTextArea]
#endif
    public string remark;//(PS:Remark 仅用于SO实例，Component子类就单独增加Remark组件）

    public static void OnManualDestroy(SOForSceneBase so, GameObject target, string id = "")
    {
        so.OnManualDestroy(target, id);
    }
    /// <summary>
    /// 手动调用的Destroy，保证指定物体的关联实例能销毁（如Tween）
    /// </summary>
    /// <param name="target"></param>
    protected virtual void OnManualDestroy(GameObject target, string id = "") { }


    #region Editor Method
#if UNITY_EDITOR

    /// <summary>
    /// Inspector Common Tips
    /// </summary>
    /// <param name="sB"></param>
    public virtual void SetInspectorGUICommonTextArea(StringBuilder sB, GameObject target, string id = "") { }

#endif
    #endregion
}
public static class SOForSceneExtension
{
    /// <summary>
    /// Call Destroy if the param if not null
    /// </summary>
    /// <param name="so"></param>
    /// <param name="target"></param>
    public static void OnManualDestroy(this SOForSceneBase so, GameObject target, string id = "")
    {
        if (so && target)
            SOForSceneBase.OnManualDestroy(so, target, id);
    }
}
