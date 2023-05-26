using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComponentHelperGroupBase<THelper, TComp> : ComponentGroupBase<THelper>
    where THelper : ComponentHelperBase<TComp>
    where TComp : Component
{

    public bool EnableComp
    {
        get { return enabled; }
        set { ForEachChildComponent((h) => h.EnableComp = value); }
    }

    /// <summary>
    /// 如果子对象符合条件，自动生成相应的组件
    /// </summary>
    public bool isAutoGenerateComp = false;

    public override void ForEachChildComponent(UnityAction<THelper> func)
    {
        //自动添加相应的Helper组件
        if (isAutoGenerateComp)
        {
            UnityAction<TComp> generateFunc = (com) =>
             {
                 com.gameObject.AddComponentOnce<THelper>();
             };
            ForEachChildComponent<TComp>(generateFunc);
        }

        base.ForEachChildComponent(func);
    }

}
