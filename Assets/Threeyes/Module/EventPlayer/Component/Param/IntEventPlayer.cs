using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Threeyes.EventPlayer
{   /// <summary>
    /// Event with int
    /// </summary>
    public class IntEventPlayer : EventPlayerWithParamBase<IntEventPlayer, IntEvent, int>
    {

        #region Editor Method
#if UNITY_EDITOR

        static string instName = "IntEP ";

        [UnityEditor.MenuItem(strParamMenuGroup + "IntEventPlayer", false, intParamMenuOrder + 0)]
        public static void CreateFloatEventPlayer()
        {
            EditorTool.CreateGameObject<IntEventPlayer>(instName);
        }

        [UnityEditor.MenuItem(strParamMenuGroup + "IntEventPlayer Child", false, intParamMenuOrder + 1)]
        public static void CreateFloatEventPlayerChild()
        {
            EditorTool.CreateGameObjectAsChild<IntEventPlayer>(instName);
        }

        /// <summary>
        /// Show the name of this class in shorter form
        /// </summary>
        /// <param name="sB"></param>
        public override void SetHierarchyGUIType(StringBuilder sB)
        {
            sB.Append("I");
        }

#endif
        #endregion

    }
}
