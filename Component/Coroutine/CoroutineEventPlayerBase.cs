using System.Collections;
using System.Collections.Generic;
using System.Text;
using Threeyes.Core;
using UnityEngine;

namespace Threeyes.EventPlayer
{
    public abstract class CoroutineEventPlayerBase : EventPlayer
    {
        #region Property & Field

        public bool IsCoroutineRunning { get { return isCoroutineRunning; } set { isCoroutineRunning = value; } }
        public bool IsIgnoreTimeScale { get { return isIgnoreTimeScale; } set { isIgnoreTimeScale = value; } }
        protected float CurGameTime { get { return IsIgnoreTimeScale ? Time.realtimeSinceStartup : Time.time; } }
        protected float DeltaGameTime { get { return IsIgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime; } }
        protected bool HasDestoryed { get { return this == null; } }



        [SerializeField]
        [Tooltip("If set to ture, it will ignore the Time scale")]
        protected bool isIgnoreTimeScale = false;
        [SerializeField]
        protected bool isCoroutineRunning = false;//Is the CoroutineRunning
        protected UnityEngine.Coroutine cacheEnum;

        //Editor Display
        protected float CoroutineUsedTime { get { return coroutineUsedTime; } set { coroutineUsedTime = value; RepaintHierarchyWindow(); } }
        private float coroutineUsedTime = 0;

        #endregion

        #region Inner Method

        protected virtual void TryStopCoroutine()
        {
            if (cacheEnum != null)
            {
                CoroutineManager.StopCoroutineEx(cacheEnum);
                cacheEnum = null;
            }
        }
        protected override void StopFunc()
        {
            TryStopCoroutine();//In case the developer has manual stop the Coroutine
            base.StopFunc();
        }

        #endregion

        #region Editor Method
#if UNITY_EDITOR


        const string strCoroutineRunningSymbol1 = "▶";
        const string strCoroutineRunningSymbol2 = "▷";
        /// <summary>
        /// 根据已运行时间，显示对应的提示光标
        /// </summary>
        /// <param name="runedTime"></param>
        /// <returns></returns>
        protected string GetRunningSymbol(float runedTime)
        {
            //定时显隐，模拟闪烁效果
            return (int)(runedTime * 2) % 2 == 0 ? strCoroutineRunningSymbol1 : strCoroutineRunningSymbol2;
        }

        public override void SetInspectorGUISubProperty(GUIPropertyGroup group)
        {
            base.SetInspectorGUISubProperty(group);
            group.listProperty.Add(new GUIProperty(nameof(isCoroutineRunning), "CoroutineRunning", isEnable: false));//Readonly
            group.listProperty.Add(new GUIProperty(nameof(isIgnoreTimeScale), "IgnoreTimeScale"));
        }

#endif
        #endregion
    }
}
