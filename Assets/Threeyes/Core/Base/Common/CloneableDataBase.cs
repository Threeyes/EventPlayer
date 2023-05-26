using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Threeyes.Base
{
    public interface ICloneableData : ICloneable
    {
        TInst Clone<TInst>();
    }


    /// <summary>
    /// Make this data cloneable
    /// </summary>
    public abstract class CloneableDataBase : ICloneable
    {
        #region Editor Utility

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        /// <summary>
        /// Copy data to other instance
        /// (PS: only clone struct and reference. For class type instance, you should create new instance using their construct function(eg: AnimationCurve(params Keyframe[] keys))
        /// </summary>
        /// <typeparam name="TInst"></typeparam>
        /// <returns></returns>
        public virtual TInst Clone<TInst>()
        {
            return (TInst)Clone();
        }

        #endregion
    }
}