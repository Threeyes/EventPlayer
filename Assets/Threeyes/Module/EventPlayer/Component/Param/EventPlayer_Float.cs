using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using Threeyes.Editor;
#endif

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// Event with float
    /// </summary>
    public class EventPlayer_Float : EventPlayerWithParamBase<EventPlayer_Float, FloatEvent, float>
    {
        #region Editor Method
#if UNITY_EDITOR

        //！！MenuItem！！
        static string instName = "FloatEP ";
        [UnityEditor.MenuItem(strMenuItem_Root_Param + "Float", false, intParamMenuOrder + 1)]
        public static void CreateFloatEventPlayer()
        {
            EditorTool.CreateGameObjectAsChild<EventPlayer_Float>(instName);
        }

        //！！Hierarchy GUI！！
        public override string ShortTypeName { get { return "F"; } }

#endif
        #endregion
    }
}
