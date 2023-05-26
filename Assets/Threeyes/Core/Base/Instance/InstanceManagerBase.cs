using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 用于管理任意数量(1~N)的同类组件，方便其他代码通过静态类访问
/// </summary>
/// <typeparam name="TInst"></typeparam>
/// <typeparam name="TComp"></typeparam>
[RequireComponent(typeof(InstanceManager))]
public class InstanceManagerBase<TInst, TComp> : InstanceBase<TInst>
    where TInst : InstanceManagerBase<TInst, TComp>
    where TComp : Component
{
    /// <summary>
    /// 当前的默认组件
    /// </summary>
    public static TComp Comp
    {
        get
        {
            if (!Instance.comp)
                Instance.comp = Instance.GetCompFunc();
            return Instance.comp;
        }
    }

    public TComp comp;//待绑定的组件

    protected virtual TComp GetCompFunc()
    {
        TComp comp = GameObject.FindObjectOfType<TComp>();
        if (!comp)
        {
            Debug.LogError("Can't find type of" + typeof(TComp) + "!");
        }
        return comp;
    }

}
