using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
/// <summary>
/// Property组，可用于FoldOut或Box
/// </summary>
[Serializable]
public class GUIPropertyGroup
{
    public string title;//The title of the Group
    public StringBuilder sbTipsTop = new StringBuilder();//Extra Tips info on Top
    public bool HasContent
    {
        get { return sbTipsTop.Length > 0 || listProperty.Count > 0; }
    }
    [SerializeField]
    public List<GUIProperty> listProperty = new List<GUIProperty>();

    public void Clear()
    {
        title = "";
        listProperty.Clear();
        sbTipsTop.Length = 0;//Reset
    }
}

/// <summary>
/// 单个Property
/// </summary>
[Serializable]
public class GUIProperty
{
    public GUIProperty(string propertyPath, string text = "", string tooltip = "", bool isShow = true, bool isEnable = true)
    {
        this.propertyPath = propertyPath;
        this.text = text;
        this.tooltip = tooltip;
        this.isShow = isShow;
        this.isEnable = isEnable;
    }

    public string propertyPath;

    public GUIContent gUIContent { get { return text.NotNullOrEmpty() ? new GUIContent(text, tooltip) : null; } }//如果为空，那就使用默认绘制
    public string text;
    public string tooltip;
    public bool isShow = true;//Show
    public bool isEnable = true;//Readonly
}