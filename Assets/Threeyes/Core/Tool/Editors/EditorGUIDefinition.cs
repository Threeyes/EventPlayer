#if UNITY_EDITOR
using UnityEngine;

public static class EditorGUIDefinition
{
    //判断当前主题的Skin，返回合适的颜色

    public static bool IsProSkin
    {
        get
        {
            return UnityEditor.EditorGUIUtility.isProSkin;
        }
    }

    /// <summary>
    /// 整体高亮的颜色
    /// </summary>
    public static Color HLColor
    {
        get
        {
            Color targetColor = Color.cyan;
            return targetColor;
        }
    }
}
#endif
