using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 初始化单例类，每个继承InstanceBase的物体都需要挂载该脚本
/// </summary>
[DefaultExecutionOrder(-24200)]
public class InstanceManager : MonoBehaviour
{
    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        ISetInstance[] arrInterface = transform.GetComponents<ISetInstance>();

        foreach (var inter in arrInterface)
        {
            inter.SetInstance();
        }
    }
}
