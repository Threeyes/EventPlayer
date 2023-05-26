using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 所有ComponentHelper的基类
/// </summary>
/// <typeparam name="TComp"></typeparam>
public class ComponentHelperBase<TComp> : MonoBehaviour
    where TComp : Component
{
    public virtual TComp Comp
    {
        get
        {
            if (!comp)
                comp = GetCompFunc();
            return comp;
        }
        set
        {
            comp = value;
        }
    }
    public TComp comp;//待绑定的组件

    public TComp[] comps = null;//待绑定的组件
    public TComp[] Comps
    {
        get
        {
            if (comps.Length == 0)
                comps = GetCompsFunc();
            return comps;
        }
        set
        {
            comps = value;
        }
    }

    protected virtual TComp GetCompFunc()
    {
        if (this)//避免物体被销毁
            return GetComponent<TComp>();

        return default(TComp);
    }

    protected virtual TComp[] GetCompsFunc()
    {
        if (this)//避免物体被销毁
            return GetComponents<TComp>();

        return default(TComp[]);
    }


    /// <summary>
    /// 激活/隐藏（继承于Behaviour的）组件
    /// </summary>
    public bool EnableComp
    {
        get { return behaviour ? behaviour.enabled : false; }
        set { if (behaviour) behaviour.enabled = value; }
    }

    /// <summary>
    /// 尝试转换为Behaviour
    /// </summary>
    Behaviour behaviour { get { return Comp as Behaviour; } }
}
