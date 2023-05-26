using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton，自动创建并保证场景中至少存在一个
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _Instance;
    public static T Instance
    {
        get
        {
            if (!_Instance)
            {
                _Instance = GameObject.FindObjectOfType<T>();
                if (!_Instance)
                {
                    GameObject newGo = new GameObject(typeof(T).ToString(), typeof(T));
                    _Instance = newGo.GetComponent<T>();
                }
                if (Application.isPlaying)
                    DontDestroyOnLoad(_Instance.gameObject);
            }
            return _Instance;
        }
    }
}
