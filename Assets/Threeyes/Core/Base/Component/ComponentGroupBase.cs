using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 对具有相同组件T的"子物体"进行操作，动态获取
/// </summary>
/// <typeparam name="T">适用于接口</typeparam>
public class ComponentGroupBase<T> : MonoBehaviour
//where T : Component
{
    public bool isIncludeHide = true;
    public bool isRecursive = true;
    public bool includeSelf = false;
    /// <summary>
    /// 适用于外部的链接组件
    /// </summary>
    public List<T> listExtraComp = new List<T>();

    public List<T> ListComp
    {
        get { return GetAllComponent(); }
    }

    public virtual List<T> GetAllComponent()
    {
        List<T> listTempComp = new List<T>();
        ForEachChildComponent((c) => listTempComp.Add(c));
        return listTempComp;
    }

    /// <summary>
    /// 针对特定的Component（比如依赖的组件）
    /// </summary>
    /// <typeparam name="T2"></typeparam>
    /// <param name="func"></param>
    public virtual void ForEachChildComponent(UnityAction<T> func)
    {
        ForEachChildComponent<T>(func);
    }


    /// <summary>
    /// 子物体中的任意组件
    /// </summary>
    /// <typeparam name="T2"></typeparam>
    /// <param name="func"></param>
    public virtual void ForEachChildComponent<T2>(UnityAction<T2> func)
    //where T2 : Component
    {
        transform.ForEachChildComponent(func, isIncludeHide, includeSelf, isRecursive);

        listExtraComp.ForEach((c) =>
        {
            Component cInst = c as Component;
            if (cInst)
                cInst.ForEachSelfComponent(func);
        });
    }

}
