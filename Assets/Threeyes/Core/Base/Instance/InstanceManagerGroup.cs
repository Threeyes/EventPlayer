using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// 主动调用自身及子集下的Init方法，保证单例即使被隐藏也能正常初始化
/// 适用于：挂在顶层非隐藏物体上
/// </summary>
[DefaultExecutionOrder(-24201)]//保证比InstanceManager优先执行
public class InstanceManagerGroup : ComponentGroupBase<InstanceManager>
{
    private void Awake()
    {
        Init();
    }
    public void Init()
    {
        ForEachChildComponent((instM) => instM.Init());
    }
}
