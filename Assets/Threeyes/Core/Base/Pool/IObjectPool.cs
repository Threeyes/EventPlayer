using System;

namespace Threeyes.Pool
{
    /// <summary>
    /// Ref: UnityEngine.Pool.IObjectPool��Ϊ�˼��ݾɰ汾���Խ���ԭ�ӿڸ���
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectPool<T> where T : class
    {
        int CountInactive
        {
            get;
        }

        T Get();

        PooledObject<T> Get(out T v);

        void Release(T element);

        void Clear();
    }
}