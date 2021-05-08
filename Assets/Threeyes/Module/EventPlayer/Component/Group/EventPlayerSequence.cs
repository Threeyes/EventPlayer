using UnityEngine;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// Manage List EP
    /// </summary>
    public class EventPlayerSequence : ComponentSequenceBase<EventPlayer>
    {
        #region Inner Method

        protected override void SetDataValid(EventPlayer data)
        {
            data.IsActive = true;
        }
        protected override bool IsDataVaild(EventPlayer data)
        {
            return data.IsActive;
        }
        protected override void SetDataFunc(EventPlayer data, int index)
        {
            data.Play();
        }

        protected override void ResetDataFunc(EventPlayer data, int index)
        {
            data.Stop();
            base.ResetDataFunc(data, index);
        }

        #endregion

        #region Editor Method
#if UNITY_EDITOR

        //——MenuItem——
        static string instGroupName = "EPS ";
        [MenuItem(EventPlayer.strSubCollectionMenuItem + "EventPlayerSequence", false, EventPlayer.intCollectionMenuOrder + 2)]
        public static void CreateEventPlayerSequence()
        {
            EditorTool.CreateGameObject<EventPlayerSequence>(instGroupName);
        }
        [MenuItem(EventPlayer.strSubCollectionMenuItem + "EventPlayerSequence Child", false, EventPlayer.intCollectionMenuOrder + 3)]
        public static void CreateEventPlayerSequenceChild()
        {
            EditorTool.CreateGameObjectAsChild<EventPlayerSequence>(instGroupName);
        }

        //——Hierarchy GUI——
        public override void SetHierarchyGUIType(StringBuilder sB)
        {
            sB.Append("EPS");
        }

        public override void SetHierarchyGUIProperty(StringBuilder sB)
        {
            base.SetHierarchyGUIProperty(sB);
        }

#endif
        #endregion

    }
}