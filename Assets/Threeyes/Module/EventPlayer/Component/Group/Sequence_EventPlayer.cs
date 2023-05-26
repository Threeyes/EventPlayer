using UnityEngine;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
using Threeyes.Editor;
#endif

namespace Threeyes.EventPlayer
{
    public interface ISequence_EventPlayer
    {
        /// <summary>
        /// 功能：Hierarchy中返回子EP的序号
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int FindIndexForDataEditor(EventPlayer data);
    }
    public class Sequence_EventPlayerBase<TEventPlayer> : SequenceForCompBase<TEventPlayer>, ISequence_EventPlayer
        where TEventPlayer : EventPlayer
    {
        #region Inner Method

        protected override void SetDataValid(TEventPlayer data)
        {
            data.IsActive = true;
        }
        protected override bool IsDataVaild(TEventPlayer data)
        {
            return data.IsActive;
        }
        protected override void SetDataFunc(TEventPlayer data, int index)
        {
            data.Play();
            base.SetDataFunc(data, index);
        }

        protected override void ResetDataFunc(TEventPlayer data, int index)
        {
            data.Stop();
            base.ResetDataFunc(data, index);
        }

        #endregion

        public int FindIndexForDataEditor(EventPlayer data)
        {
            if (IsLoadChildOnAwake && data is TEventPlayer eventPlayerReal)
            {
                return GetComponentsFromChild().IndexOf(eventPlayerReal);
            }
            return -1;
        }
    }

    /// <summary>
    /// Manage List EP
    /// </summary>
    public class Sequence_EventPlayer : Sequence_EventPlayerBase<EventPlayer>
    {
        #region Editor Method
#if UNITY_EDITOR
        //——MenuItem——
        static string instGroupName = "EPS ";
        [MenuItem(EventPlayer.strMenuItem_Root_Collection + "EventPlayerSequence", false, EventPlayer.intCollectionMenuOrder + 2)]
        public static void CreateEventPlayerSequence()
        {
            EditorTool.CreateGameObject<Sequence_EventPlayer>(instGroupName);
        }
        [MenuItem(EventPlayer.strMenuItem_Root_Collection + "EventPlayerSequence Child", false, EventPlayer.intCollectionMenuOrder + 3)]
        public static void CreateEventPlayerSequenceChild()
        {
            EditorTool.CreateGameObjectAsChild<Sequence_EventPlayer>(instGroupName);
        }

        //——Hierarchy GUI——
        public override string ShortTypeName { get { return "EPS"; } }

        public override void SetHierarchyGUIProperty(StringBuilder sB)
        {
            base.SetHierarchyGUIProperty(sB);
        }

#endif
        #endregion
    }
}