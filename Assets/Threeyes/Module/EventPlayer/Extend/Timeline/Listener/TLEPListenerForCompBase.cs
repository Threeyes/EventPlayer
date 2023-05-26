using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Threeyes.EventPlayer
{
    /// <summary>
    /// PS: 
    /// 1.Ref TComp to use it's param
    /// 2.Keep the name short in case Windows Explorer hates it
    /// 
    /// ToAdd: Interface For TempEP, can simluate and send PlayableInfo data, don't need Timeline support(可以是一个方法Evaluate（perecent）)

    /// </summary>
    public class TLEPListenerForCompBase<TComp> : TLEPListener
           where TComp : Component
    {
        #region Property & Field

        public TComp Comp
        {
            get
            {
                if (!comp)
                    comp = GetCompFunc();
                return comp;
            }
            set
            {
                comp = value;
            }
        }

        [Tooltip("The GameObject that TComp attached to")]
        [SerializeField] protected GameObject goCompTarget;//PS: if set to null, it will be this GameObject
        protected TComp comp;//The relate Component(PS: this can't show in the Inspector, cause it may be missing if the relate Plugin is not active)

        #endregion

        #region Inner Method

        protected virtual TComp GetCompFunc()
        {
            if (this && !goCompTarget && gameObject)
                goCompTarget = gameObject;

            if (goCompTarget)//避免物体被销毁导致丢失
                return goCompTarget.GetComponent<TComp>();

            return null;
        }

        #endregion
    }

    public class TLEPListenerForCompBaseDummy : TLEPListener
    {
        [SerializeField] protected GameObject goCompTarget;//Maintain the reference
    }
}