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
    /// Event with float
    /// </summary>
    [AddComponentMenu(EditorDefinition_EventPlayer.AssetMenuPrefix_Action_Param + "EventPlayer_Float")]
    public class EventPlayer_Float : EventPlayerWithParamBase<EventPlayer_Float, FloatEvent, float>
    {
        #region Editor Method
#if UNITY_EDITOR

        //——MenuItem——
        static string instName = "FloatEP ";
        [UnityEditor.MenuItem(strMenuItem_Root_Param + "Float", false, intParamMenuOrder + 1)]
        public static void CreateFloatEventPlayer()
        {
            EditorTool.CreateGameObjectAsChild<EventPlayer_Float>(instName);
        }

        //——Hierarchy GUI——
        public override string ShortTypeName { get { return "F"; } }

#endif
        #endregion
    }
}
