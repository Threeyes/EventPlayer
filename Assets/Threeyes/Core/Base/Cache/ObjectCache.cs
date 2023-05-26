using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Threeyes.Cache
{
    public interface IObjectCache<TID, T> where T : class
    {
        int CountAll { get; }

        void Add(TID id, T element);
        T Get(TID id, bool useCache);
        T Get(TID id);

        CachedObject<TID, T> Get(TID id, out T v);

        void Release(TID id, T element);

        void Clear();
    }
    /// <summary>
    /// Cache Asset by ID
    /// ToUpdate:
    /// </summary>
    /// <typeparam name="TID"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class ObjectCache<TID, T> : IObjectCache<TID, T>, IDisposable
        where T : class
    {
        public int CountAll { get { return m_Dictionary.Count; } }

        internal readonly Dictionary<TID, T> m_Dictionary;

        private readonly Func<TID, T> m_CreateFunc;
        private readonly UnityAction<TID, T> m_ActionOnGet;
        private readonly UnityAction<TID, T> m_ActionOnRelease;
        private readonly UnityAction<TID, T> m_ActionOnDestroy;

        private readonly int m_MaxSize;
        internal bool m_CollectionCheck;

        //ToThink:是否需要限制上限（或者把限制的方法做成Func，子类可通过根据当前数据的总大小是否超过上限，来决定是否缓存）
        public ObjectCache(Func<TID, T> createFunc, UnityAction<TID, T> actionOnGet = null, UnityAction<TID, T> actionOnRelease = null, UnityAction<TID, T> actionOnDestroy = null, bool collectionCheck = false, int maxSize = 10000)
        {
            m_Dictionary = new Dictionary<TID, T>();
            m_MaxSize = maxSize;
            m_CollectionCheck = collectionCheck;
            m_CreateFunc = createFunc != null ? createFunc : DefaultCreateFunc;
            m_ActionOnGet = actionOnGet != null ? actionOnGet : DefaultOnGetFunc;
            m_ActionOnRelease = actionOnRelease != null ? actionOnRelease : DefaultOnReleaseFunc;
            m_ActionOnDestroy = actionOnDestroy != null ? actionOnDestroy : DefaultOnDestroyFunc;

            if (m_MaxSize <= 0)
            {
                throw new ArgumentException("Max Size must be greater than 0", "maxSize");
            }
        }
        public virtual void Add(TID id, T element)
        {
            if (element != null && !m_Dictionary.ContainsKey(id))//判断是否创建成功
                m_Dictionary.Add(id, element);//Save to dic
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>null if not exists</returns>
        public virtual T Get(TID id)
        {
            return Get(id, true);
        }

        public T Get(TID id, bool useCache)
        {
            if (!useCache)
                return m_CreateFunc(id);//直接调用创建方法，不需要缓存

            T val = null;

            if (!m_Dictionary.ContainsKey(id))
            {
                val = m_CreateFunc(id);//Create new one
                if (val != null)//Check if create successed
                    m_Dictionary.Add(id, val);//Save to dic                                            
                //UnityEngine.Debug.Log("Load from File: " + typeof(T) + " " + id);
            }
            else
            {
                val = m_Dictionary[id];//Get from cache                                  
                //UnityEngine.Debug.Log("Get from cache: " + typeof(T) + " " + id);
            }

            InvokeOnGetFunc(id, val);
            return val;
        }

        public CachedObject<TID, T> Get(TID id, out T v)
        {
            return new CachedObject<TID, T>(id, v = Get(id), this);
        }

        public void Release(TID id, T element)
        {
            InvokeOnReleaseFunc(id, element);
            //PS：因为Cache不使用堆栈，所以不需要调用其他代码
        }
        public void Clear()
        {
            if (m_ActionOnDestroy != null)
            {
                foreach (var item in m_Dictionary)
                {
                    m_ActionOnDestroy(item.Key, item.Value);
                }
            }
            m_Dictionary.Clear();
        }
        public void Dispose()
        {
            Clear();
        }

        protected virtual void InvokeOnGetFunc(TID id, T element)
        {
            m_ActionOnGet?.Invoke(id, element);
        }
        protected virtual void InvokeOnReleaseFunc(TID id, T element)
        {
            m_ActionOnRelease?.Invoke(id, element);
        }

        #region Default Method
        protected virtual T DefaultCreateFunc(TID id) { throw new NotImplementedException(); }
        protected virtual void DefaultOnGetFunc(TID id, T target) { }

        protected virtual void DefaultOnReleaseFunc(TID id, T target) { }
        protected virtual void DefaultOnDestroyFunc(TID id, T target) { }

        #endregion
    }

    public struct CachedObject<TID, T> : IDisposable where T : class
    {
        private readonly TID m_ID;

        private readonly T m_ToReturn;

        private readonly IObjectCache<TID, T> m_Pool;

        internal CachedObject(TID id, T value, IObjectCache<TID, T> pool)
        {
            m_ID = id;
            m_ToReturn = value;
            m_Pool = pool;
        }

        void IDisposable.Dispose()
        {
            m_Pool.Release(m_ID, m_ToReturn);
        }
    }
}