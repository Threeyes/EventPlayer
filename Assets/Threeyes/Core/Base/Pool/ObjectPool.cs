using System.Collections.Generic;
using UnityEngine.Events;

namespace Threeyes.Pool
{
    /// <summary>
    /// Ref: UnityEngine.Pool.ObjectPool
    /// A stack based Pool.IObjectPool_1.
    /// </summary>
    public class ObjectPool<T> : System.IDisposable, IObjectPool<T> where T : class
    {
        public int CountAll { get; private set; }
        public int CountActive => CountAll - CountInactive;
        public int CountInactive => m_Stack.Count;

        internal readonly Stack<T> m_Stack;

        private readonly System.Func<T> m_CreateFunc;
        private readonly UnityAction<T> m_ActionOnGet;
        private readonly UnityAction<T> m_ActionOnRelease;
        private readonly UnityAction<T> m_ActionOnDestroy;

        private readonly int m_MaxSize;
        internal bool m_CollectionCheck;

        public ObjectPool(System.Func<T> createFunc, UnityAction<T> actionOnGet = null, UnityAction<T> actionOnRelease = null, UnityAction<T> actionOnDestroy = null, bool collectionCheck = false, int defaultCapacity = 10, int maxSize = 10000)
        {

            m_Stack = new Stack<T>(defaultCapacity);
            m_MaxSize = maxSize;
            m_CollectionCheck = collectionCheck;
            m_CreateFunc = createFunc != null ? createFunc : DefaultCreateFunc;
            m_ActionOnGet = actionOnGet != null ? actionOnGet : DefaultOnGetFunc;
            m_ActionOnRelease = actionOnRelease != null ? actionOnRelease : DefaultOnReleaseFunc;
            m_ActionOnDestroy = actionOnDestroy != null ? actionOnDestroy : DefaultOnDestroyFunc;


            //if (m_CreateFunc == null)
            //{
            //    throw new ArgumentNullException("createFunc");
            //}

            if (m_MaxSize <= 0)
            {
                throw new System.ArgumentException("Max Size must be greater than 0", "maxSize");
            }
        }
        public T Get()
        {
            T val;
            if (m_Stack.Count == 0)
            {
                val = m_CreateFunc();
                CountAll++;
            }
            else
            {
                val = m_Stack.Pop();
            }

            InvokeOnGetFunc(val);
            return val;
        }


        /// <summary>
        /// 
        /// PS:使用方式：通过using封装，临时使用某个缓存值，使用完成后会回到池中，案例如下 (https://docs.unity3d.com/2021.1/Documentation/ScriptReference/Pool.ObjectPool_1.Get.html)
        ///     StringBuilder stringBuilder;
        ///     using (stringBuilderPool.Get(out stringBuilder))
        ///     {
        ///         stringBuilder.AppendLine("Some text");
        ///         Debug.Log(stringBuilder.ToString());
        ///     }
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public PooledObject<T> Get(out T v)
        {
            return new PooledObject<T>(v = Get(), this);
        }

        public void Release(T element)
        {
            if (m_CollectionCheck && m_Stack.Count > 0 && m_Stack.Contains(element))
            {
                throw new System.InvalidOperationException("Trying to release an object that has already been released to the pool.");
            }

            InvokeOnReleaseFunc(element);

            if (CountInactive < m_MaxSize)
            {
                m_Stack.Push(element);
            }
            else
            {
                m_ActionOnDestroy?.Invoke(element);
            }
        }


        public void Clear()
        {
            if (m_ActionOnDestroy != null)
            {
                foreach (T item in m_Stack)
                {
                    m_ActionOnDestroy(item);
                }
            }

            m_Stack.Clear();
            CountAll = 0;
        }

        public void Dispose()
        {
            Clear();
        }

        protected virtual void InvokeOnGetFunc(T element)
        {
            m_ActionOnGet?.Invoke(element);
        }
        protected virtual void InvokeOnReleaseFunc(T element)
        {
            m_ActionOnRelease?.Invoke(element);
        }

        #region Default Method
        protected virtual T DefaultCreateFunc() { throw new System.NotImplementedException(); }
        protected virtual void DefaultOnGetFunc(T target) { }

        protected virtual void DefaultOnReleaseFunc(T target) { }
        protected virtual void DefaultOnDestroyFunc(T target) { }
        #endregion
    }
}
