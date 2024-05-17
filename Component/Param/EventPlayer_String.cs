using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Threeyes.Core.Editor;
using Threeyes.Core;
#if UNITY_EDITOR
#endif

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// Event with string
    /// </summary>
    [AddComponentMenu(EditorDefinition_EventPlayer.AssetMenuPrefix_Action_Param + "EventPlayer_String")]
    public class EventPlayer_String : EventPlayerWithParamBase<EventPlayer_String, StringEvent, string>
    {
        #region Editor Method
#if UNITY_EDITOR

        //——MenuItem——
        static string instName = "StringEP ";
        [UnityEditor.MenuItem(strMenuItem_Root_Param + "String", false, intParamMenuOrder + 2)]
        public static void CreateFloatEventPlayer()
        {
            EditorTool.CreateGameObjectAsChild<EventPlayer_String>(instName);
        }

        //——Hierarchy GUI——
        public override string ShortTypeName { get { return "S"; } }

#endif
        #endregion
    }
}