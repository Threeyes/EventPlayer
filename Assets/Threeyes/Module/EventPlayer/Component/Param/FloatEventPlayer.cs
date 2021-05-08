using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// Event with float
    /// </summary>
    public class FloatEventPlayer : EventPlayerWithParamBase<FloatEventPlayer, FloatEvent, float>
    {
        #region Editor Method
#if UNITY_EDITOR

        static string instName = "FloatEP ";

        [UnityEditor.MenuItem(strParamMenuGroup + "FloatEventPlayer", false, intParamMenuOrder + 2)]
        public static void CreateFloatEventPlayer()
        {
            EditorTool.CreateGameObject<FloatEventPlayer>(instName);
        }

        [UnityEditor.MenuItem(strParamMenuGroup + "FloatEventPlayer Child", false, intParamMenuOrder + 3)]
        public static void CreateFloatEventPlayerChild()
        {
            EditorTool.CreateGameObjectAsChild<FloatEventPlayer>(instName);
        }

        /// <summary>
        /// Show the name of this class in shorter form
        /// </summary>
        /// <param name="sB"></param>
        public override void SetHierarchyGUIType(StringBuilder sB)
        {
            sB.Append("F");
        }

#endif
        #endregion
    }
}
