using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using Threeyes.Editor;
#endif
namespace Threeyes.EventPlayer
{   /// <summary>
    /// Event with int
    /// </summary>
    public class EventPlayer_Int : EventPlayerWithParamBase<EventPlayer_Int, IntEvent, int>
    {
        #region Editor Method
#if UNITY_EDITOR

        //！！MenuItem！！
        static string instName = "IntEP ";
        [UnityEditor.MenuItem(strMenuItem_Root_Param + "Int", false, intParamMenuOrder + 0)]
        public static void CreateFloatEventPlayer()
        {
            EditorTool.CreateGameObjectAsChild<EventPlayer_Int>(instName);
        }

        //！！Hierarchy GUI！！
        public override string ShortTypeName { get { return "I"; } }

#endif
        #endregion
    }
}
