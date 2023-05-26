using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnumTool
{
    /// <summary>
    /// 获取随机的枚举
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <param name="enumVal"></param>
    /// <returns></returns>
    public static E GetRandom<E>()
    {
        var values = Enum.GetValues(typeof(E));
        return (E)values.GetValue(UnityEngine.Random.Range(0, values.Length - 1));
    }

    /// <summary>
    /// 所有值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<string> ToValueList<T>()
    {
        return Enum.GetValues(typeof(T))
                .Cast<int>()
                .Select(v => v.ToString());
    }

    /// <summary>
    /// 所有名称
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<string> ToStringList<T>()
    {
        return Enum.GetNames(typeof(T));
    }


    #region Bit Shifing //以下代码仅做参考，Ref from: https://www.alanzucconi.com/2015/07/26/enum-flags-and-bitwise-operators/

    //public static T SetFlag<T>(T a, T b) 
    //{
    //    return a | b;
    //}
    //public static T UnsetFlag<T>(T a, T b)
    //{
    //    return a & (~b);
    //}
    //// Works with "None" as well
    //public static bool HasFlag<T>(T a, T b)
    //{
    //    return (a & b) == b;
    //}
    //public static T ToogleFlag<T>(T a, T b)
    //{
    //    return a ^ b;
    //}

    #endregion
}
