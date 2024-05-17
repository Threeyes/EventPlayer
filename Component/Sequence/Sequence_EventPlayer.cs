using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Threeyes.Core.Editor;
#endif

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// Manage List EP
    /// </summary>
    [AddComponentMenu(EditorDefinition_EventPlayer.AssetMenuPrefix_Action_Sequence + "Sequence_EventPlayer")]
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