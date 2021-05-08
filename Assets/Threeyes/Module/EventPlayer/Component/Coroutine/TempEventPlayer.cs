using System.Collections;
using UnityEngine;
using System.Text;
using Threeyes.Coroutine;

namespace Threeyes.EventPlayer
{

    /// <summary>
    /// Invoke Play Event for a while, then Invoke Stop Event 
    /// </summary>
    public class TempEventPlayer : CoroutineEventPlayerBase
    {
        #region Property & Field

        public bool IsContinuous { get { return isContinuous; } set { isContinuous = value; } }
        public float Duration { get { return defaultDuration; } set { defaultDuration = value; } }

        //[Header("Temp Setting")]
        [SerializeField]
        [Tooltip("Invoke Play Event on every frame, just like Update")]
        protected bool isContinuous = false;
        [SerializeField]
        [Tooltip("Total play time. if less than or equal to 0, it will never stop")]
        protected float defaultDuration = 1;

        #endregion

        #region Public Method

        public void TempPlay(float duration)
        {
            TryStopCoroutine();
            cacheEnum = CoroutineManager.StartCoroutineEx(IETempContinuousPlay(duration));
        }

        #endregion

        #region Inner Method

        protected override void PlayFunc()
        {
            TempPlay(Duration);
        }

        protected override void StopFunc()
        {
            IsCoroutineRunning = false;
            base.StopFunc();
        }

        IEnumerator IETempContinuousPlay(float duration)
        {
            IsCoroutineRunning = true;
#if UNITY_EDITOR
            coroutineUsedTime = 0;
            if (IsLogOnPlay)
                print(name + " TempEventPlay!");
#endif

            base.PlayFunc();
            if (IsPropertyValid(duration))
            {
                float tempStartTime = CurGameTime;
                float leftTime = duration;
                while (leftTime >= 0)
                {
                    if (IsContinuous)
                    {
                        base.PlayFunc();
                    }

                    leftTime -= DeltaGameTime;

#if UNITY_EDITOR
                    coroutineUsedTime += DeltaGameTime;
                    RepaintHierarchyWindow();
#endif
                    if (HasDestoryed)//In case get destroy
                        yield break;
                    yield return null;
                }
                StopFunc();
            }
            else//Infinite
            {
                while (true)
                {
                    if (IsContinuous)
                    {
                        base.PlayFunc();
                    }

#if UNITY_EDITOR
                    coroutineUsedTime += DeltaGameTime;
                    RepaintHierarchyWindow();
#endif

                    if (HasDestoryed)//In case get destroy
                        yield break;
                    yield return null;
                }
            }
        }

        bool IsPropertyValid(float value)
        {
            return value > 0;
        }

        #endregion

        #region Editor Method

#if UNITY_EDITOR

        static string instName = "TempEP ";
        [UnityEditor.MenuItem(strSubCoroutineMenuItem + "TempEventPlayer", false, intCoroutineMenuOrder + 4)]
        public static void CreateTempEventPlayer()
        {
            EditorTool.CreateGameObject<TempEventPlayer>(instName);
        }

        [UnityEditor.MenuItem(strSubCoroutineMenuItem + "TempEventPlayer Child", false, intCoroutineMenuOrder + 5)]
        public static void CreateTempEventPlayerChild()
        {
            EditorTool.CreateGameObjectAsChild<TempEventPlayer>(instName);
        }

        /// <summary>
        /// Show the name of this class in shorter form
        /// </summary>
        /// <param name="sB"></param>
        public override void SetHierarchyGUIType(StringBuilder sB)
        {
            sB.Append("T");
        }

        /// <summary>
        /// Show the key property of this class
        /// </summary>
        /// <param name="sB"></param>
        public override void SetHierarchyGUIProperty(StringBuilder sB)
        {
            base.SetHierarchyGUIProperty(sB);

            //Show Coroutine
            if (IsCoroutineRunning)
            {
                sbCache.Length = 0;
                sbCache.Append(GetRunningSymbol(coroutineUsedTime));
                sbCache.Append(coroutineUsedTime.ToString("#0.00")).Append("s");
                AddSplit(sB, sbCache);
            }


            //——Config——
            sbCache.Length = 0;
            if (IsIgnoreTimeScale)
                sbCache.Append("Ⓘ ");
            if (IsContinuous)
                sbCache.Append("Ⓒ ");//Symbol from: http://www.fhdq.net

            if (IsPropertyValid(Duration))
            { sbCache.Append(":").Append(Duration.ToString()).Append("s"); }
            else
            { sbCache.Append(":").Append("∞").Append("s"); }
            AddSplit(sB, sbCache);
        }

        public override void SetInspectorGUISubProperty(GUIPropertyGroup group)
        {
            base.SetInspectorGUISubProperty(group);
            group.title = "Temp Setting";
            group.listProperty.Add(new GUIProperty("isContinuous", "Continuous"));
            group.listProperty.Add(new GUIProperty("defaultDuration", "Duration"));
        }
#endif
        #endregion
    }
}
