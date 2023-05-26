using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Threeyes.Pool
{
    /// <summary>
    /// Manage GameObject
    /// 
    /// Ref：https://thegamedev.guru/unity-cpu-performance/object-pooling/#object-pooling-in-unity-2021-your-options
    /// </summary>
    public class GameObjectPool : ObjectPool<GameObject>
    {
        public GameObject GOPoolManager
        {
            get
            {
                if (goPoolManager == null)
                {
                    goPoolManager = new GameObject("PoolManager");
                }
                return goPoolManager;
            }
        }
        GameObject goPoolManager;

        public GameObjectPool(Func<GameObject> createFunc = null, UnityAction<GameObject> actionOnGet = null, UnityAction<GameObject> actionOnRelease = null, UnityAction<GameObject> actionOnDestroy = null, bool collectionCheck = false, int defaultCapacity = 10, int maxSize = 10000) : base(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck, defaultCapacity, maxSize)
        {
        }

        #region Default Method
        protected override GameObject DefaultCreateFunc()
        {
            return new GameObject("PooledObject");
        }
        protected override void DefaultOnGetFunc(GameObject target)
        {
            target.SetActive(true);
        }
        protected override void DefaultOnReleaseFunc(GameObject target)
        {
            target.SetActive(false);
            target.transform.SetParent(GOPoolManager ? GOPoolManager.transform : null);//存放到一个Manager中
        }
        protected override void DefaultOnDestroyFunc(GameObject target)
        {
            UnityEngine.Object.Destroy(target);
        }
        #endregion

        ///PS：
        ///1.如果使用了Pool技术，那就会调用IPoolHandler接口对应的方法（参考LeanGameObjectPool.InvokeOnDespawn)
        ///2.不应该实现任何非通用代码（如设置父物体），以避免不通用。如果需要，请自行传入默认的方法

        static UnityAction<IPoolableHandler> actOnSpawn = (ele) => ele.OnSpawn();
        static UnityAction<IPoolableHandler> actOnDespawn = (ele) => ele.OnDespawn();
        protected override void InvokeOnGetFunc(GameObject element)
        {
            SendMessage(element, actOnSpawn);
            base.InvokeOnGetFunc(element);
        }
        protected override void InvokeOnReleaseFunc(GameObject element)
        {
            SendMessage(element, actOnDespawn);
            base.InvokeOnReleaseFunc(element);
        }

        #region Utility
        static List<IPoolableHandler> tempPoolables = new List<IPoolableHandler>();
        static void SendMessage(GameObject target, UnityAction<IPoolableHandler> act)
        {
            target.GetComponents(tempPoolables);
            for (var i = tempPoolables.Count - 1; i >= 0; i--)
                act?.Invoke(tempPoolables[i]);
        }
        #endregion
    }
}