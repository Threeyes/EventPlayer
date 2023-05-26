using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 单例类的基类，需要InstanceManager辅助初始化，保证了Instance优先于其他代码进行初始化
/// </summary>
/// <typeparam name="T"></typeparam>
[RequireComponent(typeof(InstanceManager))]
public class InstanceBase<T> : MonoBehaviour, ISetInstance
    where T : InstanceBase<T>
{
    public static T Instance { get { return instance; } protected set { instance = value; } }
    private static T instance;

    public bool isDontDestroyOnLoad = false;
    bool isInit = false;

    public virtual void SetInstance()
    {
        if (isDontDestroyOnLoad && Instance && Instance != this)//因为已经实例化过一次，所以可以删掉非单例的物体
        {
            Destroy(gameObject);
            return;
        }

        if (!isInit)
        {
            SetInstanceFunc();

            if (isDontDestroyOnLoad)
            {
                transform.SetParent(null);//移动到根层级，避免父物体被销毁
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    protected virtual void SetInstanceFunc()
    {
        Instance = this as T;
        isInit = true;
    }
}
public interface ISetInstance
{
    void SetInstance();
}