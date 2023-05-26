using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Threeyes.Pool
{
    /// <summary>
    /// A linked list version of Pool.IObjectPool_1.
    /// 
    /// Ref: UnityEngine.Pool.LinkedPool
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkedPool<T> : IDisposable, IObjectPool<T> where T : class
    {
        internal class LinkedPoolItem
        {
            internal LinkedPoolItem poolNext;

            internal T value;
        }

        private readonly Func<T> m_CreateFunc;

        private readonly UnityAction<T> m_ActionOnGet;

        private readonly UnityAction<T> m_ActionOnRelease;

        private readonly UnityAction<T> m_ActionOnDestroy;

        private readonly int m_Limit;

        internal LinkedPoolItem m_PoolFirst;

        internal LinkedPoolItem m_NextAvailableListItem;

        private bool m_CollectionCheck;

        public int CountInactive
        {
            get;
            private set;
        }

        public LinkedPool(Func<T> createFunc, UnityAction<T> actionOnGet = null, UnityAction<T> actionOnRelease = null, UnityAction<T> actionOnDestroy = null, bool collectionCheck = true, int maxSize = 10000)
        {
            if (createFunc == null)
            {
                throw new ArgumentNullException("createFunc");
            }

            if (maxSize <= 0)
            {
                throw new ArgumentException("maxSize", "Max size must be greater than 0");
            }

            m_CreateFunc = createFunc;
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
            m_ActionOnDestroy = actionOnDestroy;
            m_Limit = maxSize;
            m_CollectionCheck = collectionCheck;
        }

        public T Get()
        {
            T val = null;
            if (m_PoolFirst == null)
            {
                val = m_CreateFunc();
            }
            else
            {
                LinkedPoolItem poolFirst = m_PoolFirst;
                val = poolFirst.value;
                m_PoolFirst = poolFirst.poolNext;
                poolFirst.poolNext = m_NextAvailableListItem;
                m_NextAvailableListItem = poolFirst;
                m_NextAvailableListItem.value = null;
                CountInactive--;
            }

            m_ActionOnGet?.Invoke(val);
            return val;
        }

        public PooledObject<T> Get(out T v)
        {
            return new PooledObject<T>(v = Get(), this);
        }

        public void Release(T item)
        {
            if (m_CollectionCheck)
            {
                for (LinkedPoolItem linkedPoolItem = m_PoolFirst; linkedPoolItem != null; linkedPoolItem = linkedPoolItem.poolNext)
                {
                    if (linkedPoolItem.value == item)
                    {
                        throw new InvalidOperationException("Trying to release an object that has already been released to the pool.");
                    }
                }
            }

            m_ActionOnRelease?.Invoke(item);
            if (CountInactive < m_Limit)
            {
                LinkedPoolItem linkedPoolItem2 = m_NextAvailableListItem;
                if (linkedPoolItem2 == null)
                {
                    linkedPoolItem2 = new LinkedPoolItem();
                }
                else
                {
                    m_NextAvailableListItem = linkedPoolItem2.poolNext;
                }

                linkedPoolItem2.value = item;
                linkedPoolItem2.poolNext = m_PoolFirst;
                m_PoolFirst = linkedPoolItem2;
                CountInactive++;
            }
            else
            {
                m_ActionOnDestroy?.Invoke(item);
            }
        }

        public void Clear()
        {
            if (m_ActionOnDestroy != null)
            {
                for (LinkedPoolItem linkedPoolItem = m_PoolFirst; linkedPoolItem != null; linkedPoolItem = linkedPoolItem.poolNext)
                {
                    m_ActionOnDestroy(linkedPoolItem.value);
                }
            }

            m_PoolFirst = null;
            m_NextAvailableListItem = null;
            CountInactive = 0;
        }

        public void Dispose()
        {
            Clear();
        }
    }
}