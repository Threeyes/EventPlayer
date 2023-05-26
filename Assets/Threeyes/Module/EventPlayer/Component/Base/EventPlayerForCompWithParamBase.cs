using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// Manage Sepcify Component
    /// </summary>
    /// <typeparam name="TComp"></typeparam>
    /// <typeparam name="TEP"></typeparam>
    /// <typeparam name="TUnityEvent"></typeparam>
    /// <typeparam name="TParam"></typeparam>
    public class EventPlayerForCompWithParamBase<TComp, TEP, TUnityEvent, TParam> : EventPlayerWithParamBase<TEP, TUnityEvent, TParam>
        where TEP : EventPlayerWithParamBase<TEP, TUnityEvent, TParam>
        where TUnityEvent : UnityEvent<TParam>
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

        [SerializeField] protected TComp comp;//The relate Component
        [SerializeField] protected GameObject goTarget;//The GameObject that Component attached to

        #endregion

        #region Inner Method

        protected virtual TComp GetCompFunc()
        {
            if (this && !goTarget && gameObject)
                goTarget = gameObject;

            if (goTarget)//避免物体被销毁导致丢失
                return goTarget.GetComponent<TComp>();

            return null;
        }

        #endregion

        #region Editor Method
#if UNITY_EDITOR

        public override void SetInspectorGUISubProperty(GUIPropertyGroup group)
        {
            base.SetInspectorGUISubProperty(group);
            group.listProperty.Add(new GUIProperty(nameof(goTarget), "Comp"));
        }
#endif
        #endregion
    }

    public class EventPlayerForCompWithParamBaseDummy : EventPlayer
    {
        [SerializeField] protected GameObject goTarget;//Maintain the reference

        #region Editor Method
#if UNITY_EDITOR

        public override void SetInspectorGUISubProperty(GUIPropertyGroup group)
        {
            base.SetInspectorGUISubProperty(group);
            group.listProperty.Add(new GUIProperty(nameof(goTarget), "Comp"));
        }
#endif
        #endregion
    }
}