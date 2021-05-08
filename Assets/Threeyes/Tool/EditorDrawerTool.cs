#if UNITY_EDITOR
using Threeyes.Extension;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System.Text;
/// <summary>
/// 在Hierarchy绘制小图标
/// </summary>
public static class EditorDrawerTool
{
    #region Property & Field

    //列起始位置：30(spaceSize + triangleSiz)

    public static Vector2 triangleSize = new Vector2(14, 14);//展开三角形的大小

    public static Vector2 spaceSize = new Vector2(16, 16);//行间隔（行高）
    public static Vector2 intervalSize = Vector2.one * 2;//绘制组件之间的间隔

    //组件的大小，注意高度与行高一致能够保证居中
    public static Vector2 toggleSize = new Vector2(16, 16);//Toggle的大小
    public static Vector2 buttonSize = new Vector2(16, 16);//Button的大小
    public static Vector2 sliderSize = new Vector2(48, 16);
    public static int fontSize = 11;

    public static Color colorTransparent = new Color(0, 0, 0, 0);
    public static Color colorWarning = new Color(1, 0.64f, 0, 0);


    #region GUIStyle

    //EditorStyles.textField//系统自定义的GUIStyle，尝试从此获取



    //——Label Style——
    static GUIStyle _gUISytleGroupTitleText;
    public static GUIStyle gUISytleGroupTitleText
    {
        get
        {
            if (_gUISytleGroupTitleText == null)
            {
                _gUISytleGroupTitleText = new GUIStyle()
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                };
                _gUISytleGroupTitleText.normal.textColor = Color.cyan * 0.8f; //Todo:根据Editor的Skin决定颜色
            }
            return _gUISytleGroupTitleText;
        }
    }

    //——Label Style——
    static GUIStyle _gUIStyleLabel;
    public static GUIStyle gUIStyleLabel
    {
        get
        {
            if (_gUIStyleLabel == null)
            {
                _gUIStyleLabel = new GUIStyle()
                {
                    fontSize = fontSize,
                    alignment = TextAnchor.MiddleRight,
                    richText = true,
                };
                _gUIStyleLabel.normal.textColor = Color.cyan * 0.65f; //Todo:根据Editor的Skin决定颜色

            }
            return _gUIStyleLabel;
        }
    }

    //——TextArea Style——
    static GUIStyle _gUIStyleTextArea;
    public static GUIStyle gUIStyleTextArea
    {
        get
        {
            if (_gUIStyleTextArea == null)
            {
                _gUIStyleTextArea = new GUIStyle(EditorStyles.textArea)
                {
                    wordWrap = true,
                    richText = true,
                };
            }
            return _gUIStyleTextArea;
        }
    }

    //——Toggle Style——
    static GUIStyle gUIStyleSwtichToggle
    {
        get
        {
            if (_gUIStyleSwitchToggle == null)
            {
                _gUIStyleSwitchToggle = new GUIStyle(GUI.skin.toggle);//复制默认（因为不同Unity版本的默认设置不同）
                _gUIStyleSwitchToggle.border = new RectOffset(6, 6, 4, 4);//类似Sprite的Border
                _gUIStyleSwitchToggle.margin = new RectOffset(4, 4, 2, 2);
                _gUIStyleSwitchToggle.padding = new RectOffset(2, 2, 2, 2);
                _gUIStyleSwitchToggle.overflow = new RectOffset(0, 0, 0, 0);//The margins between elements rendered in this style and any other GUI elements.
                _gUIStyleSwitchToggle.alignment = TextAnchor.MiddleCenter;
                _gUIStyleSwitchToggle.normal.background = TexSwitchBGOff;
                _gUIStyleSwitchToggle.hover.background = TexSwitchBGOff;
                _gUIStyleSwitchToggle.active.background = TexSwitchBGOff;
                _gUIStyleSwitchToggle.focused.background = TexSwitchBGOff;
                _gUIStyleSwitchToggle.onNormal.background = TexSwitchToggleBGOn;
                _gUIStyleSwitchToggle.onHover.background = TexSwitchToggleBGOn;
                _gUIStyleSwitchToggle.onActive.background = TexSwitchToggleBGOn;
                _gUIStyleSwitchToggle.onFocused.background = TexSwitchToggleBGOn;
            }
            return _gUIStyleSwitchToggle;
        }
    }
    static GUIStyle _gUIStyleSwitchToggle;

    static GUIStyle gUIStyleDisplayButton
    {
        get
        {
            if (_gUIStyleDisplayButton == null)
            {
                _gUIStyleDisplayButton = new GUIStyle(GUI.skin.button);//复制默认（因为不同Unity版本的默认设置不同）
                _gUIStyleSwitchToggle.border = new RectOffset(6, 6, 4, 4);//类似Sprite的Border
                _gUIStyleSwitchToggle.margin = new RectOffset(4, 4, 2, 2);
                _gUIStyleSwitchToggle.padding = new RectOffset(2, 2, 2, 2);
                _gUIStyleSwitchToggle.overflow = new RectOffset(0, 0, -1, 2);//The margins between elements rendered in this style and any other GUI elements.
                _gUIStyleDisplayButton.alignment = TextAnchor.MiddleCenter;
                _gUIStyleDisplayButton.normal.background = TexDisplayBG;
                _gUIStyleDisplayButton.hover.background = TexDisplayBG;
                _gUIStyleDisplayButton.active.background = TexDisplayBG;
                _gUIStyleDisplayButton.focused.background = TexDisplayBG;
            }
            return _gUIStyleDisplayButton;
        }
    }
    static GUIStyle _gUIStyleDisplayButton;

    //——GUI Texture——
    static Texture2D TexSwitchToggleBGOn
    {
        get
        {
            if (!_texSwitchBGOn)
            {
                _texSwitchBGOn = LoadResourcesSprite("SwitchToggleBG_On");
            }
            return _texSwitchBGOn;
        }
    }
    static Texture2D _texSwitchBGOn;
    static Texture2D TexSwitchBGOff
    {
        get
        {
            if (!_texSwitchBGOff)
            {
                _texSwitchBGOff = LoadResourcesSprite("SwitchToggleBG_Off");
            }
            return _texSwitchBGOff;
        }
    }
    static Texture2D _texSwitchBGOff; static Texture2D TexDisplayBG
    {
        get
        {
            if (!_texDisplayBG)
            {
                _texDisplayBG = LoadResourcesSprite("DisplayBG");
            }
            return _texDisplayBG;
        }
    }
    static Texture2D _texDisplayBG;

    public static Texture TexArrLeftIcon { get { if (!_texArrLeftIcon) _texArrLeftIcon = EditorDrawerTool.LoadResourcesSprite("Arrow_Left"); return _texArrLeftIcon; } }
    static Texture _texArrLeftIcon;
    public static Texture TexArrRightIcon { get { if (!_texArrRightIcon) _texArrRightIcon = EditorDrawerTool.LoadResourcesSprite("Arrow_Right"); return _texArrRightIcon; } }
    static Texture _texArrRightIcon;


    //——GUI Skin——
    //Visual Debug(可视化测试各种Style)
    static GUISkin guiSkinDebug
    {
        get
        {
            if (!_guiSkinDebug)
            {
                _guiSkinDebug = Resources.Load<GUISkin>("Icons/" + "DebugSkin");
                _guiSkinDebug.button = new GUIStyle(GUI.skin.button);
                _guiSkinDebug.toggle = new GUIStyle(GUI.skin.toggle);
            }
            return _guiSkinDebug;
        }
    }
    static GUISkin _guiSkinDebug;

    #endregion

    #endregion

    #region GUI Draw

    public static void DrawSpace()
    {
        GUILayout.Space(2);
    }

    public static bool DrawFoldOut(SerializedObject serializedObject, GUIPropertyGroup gUIPropertyGroup, bool isFoldout)
    {
        return DrawFoldOut(isFoldout, new GUIContent(gUIPropertyGroup.title),
            () =>
            {
                DrawTextArea(gUIPropertyGroup.sbTipsTop, true);//Draw Tips
                foreach (var property in gUIPropertyGroup.listProperty) //Draw Property
                {
                    if (property.isShow)
                    {
                        if (!property.isEnable)
                            GUI.enabled = false;
                        DrawPropertyField(serializedObject, property.propertyPath, new GUIContent(property.text));
                        if (!property.isEnable)
                            GUI.enabled = true;
                    }
                }
            });
    }

    /// <summary>
    /// 可以折叠的区域
    /// </summary>
    /// <param name="isFoldout"></param>
    /// <param name="gUIContent"></param>
    /// <param name="actionDrawContentOnUnFold">在展开时绘制的内容</param>
    /// <returns></returns>
    public static bool DrawFoldOut(bool isFoldout, GUIContent gUIContent, UnityAction actionDrawContentOnUnFold)
    {
        bool isFoldOut = EditorGUILayout.Foldout(isFoldout, gUIContent);
        if (isFoldOut)
        {
            actionDrawContentOnUnFold.Execute();//Draw Content
            EditorDrawerTool.DrawLine(padding: 4);//Draw Line
        }
        return isFoldOut;
    }

    /// <summary>
    /// 绘制分割线
    /// </summary>
    /// <param name="color"></param>
    /// <param name="thickness"></param>
    /// <param name="padding"></param>
    public static void DrawLine(Color? tempcolor = null, int thickness = 1, int padding = 10)
    {
        Color color = tempcolor.HasValue ? tempcolor.Value : Color.gray;
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        //r.x -= 2;
        r.width -= 4;
        EditorGUI.DrawRect(r, color);
    }

    /// <summary>
    /// 调用默认的方法进行绘制
    /// </summary>
    /// <param name="serializedObject"></param>
    /// <param name="propertyPath"></param>
    /// <param name="gUIContent"></param>
    public static bool DrawPropertyField(SerializedObject serializedObject, string propertyPath, GUIContent gUIContent, bool includeChildren = true)
    {
        try
        {
            return EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyPath), gUIContent, includeChildren);
        }
        catch (System.Exception e)
        {
            //PS:如果多选UnityEvent然后增加Event，可能会报空错，不影响
            //Debug.LogError(e);
        }
        return false;
    }


    /// <summary>
    /// 显示属性专用的按钮
    /// </summary>
    /// <param name="gUIContent"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public static void DrawDisplayButton(GUIContent gUIContent)
    {
        GUI.enabled = false;
        GUILayout.Button(gUIContent, gUIStyleDisplayButton);
        GUI.enabled = true;
    }
    public static bool DrawButton(Rect rect, Texture texture, Color colTex = default(Color))
    {
        bool isButPress = false;
        //绘制透明按钮（因为太丑）
        GUI.backgroundColor = EditorDrawerTool.colorTransparent;
        if (GUI.Button(rect, ""))
            isButPress = true;

        if (colTex != default(Color))
            GUI.color = colTex;
        GUI.DrawTexture(rect, texture);

        return isButPress;
    }

    /// <summary>
    /// 切换状态的按钮
    /// </summary>
    /// <param name="title"></param>
    /// <param name="getter"></param>
    /// <param name="setter">用于设置值，可空</param>
    /// <param name="target">用于调用Undo</param>
    /// <returns></returns>
    public static bool DrawSwitchButton(GUIContent gUIContent, CustomFunc<bool> getter, UnityAction<bool> setter = null, Object target = null)
    {
        bool curValue = getter();
        bool result = GUILayout.Toggle(curValue, gUIContent, gUIStyleSwtichToggle);
        if (result != curValue)
        {
            if (setter != null && target != null)
            {
                Undo.RecordObject(target, "Changed Property");
                setter(result);//Set the value
            }
        }
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gUIContent">如果Text为空，则不显示</param>
    /// <param name="getter"></param>
    /// <param name="setter"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static System.Enum DrawEnumPopup(GUIContent gUIContent, CustomFunc<System.Enum> getter, UnityAction<System.Enum> setter = null, Object target = null)
    {
        System.Enum curValue = getter();
        System.Enum result = gUIContent.text == "" ? EditorGUILayout.EnumPopup(curValue) : EditorGUILayout.EnumPopup(gUIContent, curValue);
        if (result != curValue)
        {
            if (setter != null && target != null)
            {
                Undo.RecordObject(target, "Changed Property");
                setter(result);
            }
        }
        return result;
    }

    /// <summary>
    /// Group的标题，居中形式
    /// </summary>
    /// <param name="text"></param>
    public static void DrawGroupTitleText(string text)
    {
        GUILayout.Label(text, gUISytleGroupTitleText);//Warning:如果不使用缓存直接修改值，会导致频繁更新而卡顿
    }
    /// <summary>
    ///  TextArea with autowarp
    /// </summary>
    /// <returns></returns>
    public static string DrawTextArea(string text)
    {
        return EditorGUILayout.TextArea(text, gUIStyleTextArea);//Warning:如果不使用缓存直接修改值，会导致频繁更新而卡顿
    }

    static string textAreaResult;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="isReadOnly">是否只读，只有使用返回值时才有效</param>
    /// <returns></returns>
    public static string DrawTextArea(StringBuilder sb, bool isReadOnly = false)
    {
        if (sb != null && sb.Length > 0)
        {
            if (isReadOnly)
                GUI.enabled = false;
            textAreaResult = EditorGUILayout.TextArea(sb.ToString(), gUIStyleTextArea);//Warning:如果不使用缓存直接修改值，会导致频繁更新而卡顿
            if (isReadOnly)
                GUI.enabled = true;
            return textAreaResult;
        }
        return "";
    }


    ///// <summary>
    ///// 绘制文本，如果长度不就就缩短
    ///// </summary>
    ///// <param name="str"></param>
    ///// <param name="rectToDraw"></param>
    ///// <param name="isHideOnOverflow"></param>
    ///// <param name="replaceTextOnOverFlow"></param>
    ///// <param name="tempLabelWidth"></param>
    ///// <returns></returns>
    //public static Rect DrawLabelAutoFix(string str, Rect rectToDraw, string strFormat = "", float? tempLabelWidth = null)
    //{
    //    float labelWidth = str.CalculateLabelSize(gUIStyleLabel).x;

    //    bool isOverFlow = labelWidth > rectToDraw.width;

    //    if (isOverFlow)
    //    {
    //        if (str.Length > 3)
    //        {
    //            str = str.Remove(str.Length - 3) + "...";
    //            labelWidth = str.CalculateLabelSize(gUIStyleLabel).x;
    //        }
    //    }

    //    Rect labelRect = new Rect(rectToDraw);
    //    labelRect.height = spaceSize.y;//保持高度不变，方便对齐
    //    EditorGUILayout.Label(labelRect, str, gUIStyleLabel);
    //    return rectToDraw.GetRemainRect(labelWidth, isAddInterval: false);
    //}

    /// <summary>
    /// 绘制文本
    /// </summary>
    /// <param name="str"></param>
    /// <param name="rectToDraw"></param>
    /// <param name="isClipOnOverflow"></param>
    /// <param name="isReplaceOnOverflow"></param>
    /// <param name="replaceText"></param>
    /// <returns></returns>
    public static Rect DrawLabel(string str, Rect rectToDraw, bool isClipOnOverflow = true, bool isReplaceOnOverflow = false, string replaceText = "...")
    {
        float rectWidth = rectToDraw.width;
        float labelWidth = str.CalculateLabelLength(gUIStyleLabel);
        bool isOverFlow = labelWidth > rectWidth;

        //——Replace——
        if (isOverFlow && isReplaceOnOverflow)//动态裁剪，可能会有性能问题(ToUpdate:如果剩余空间过多，则从进行裁剪；如果空间过少，则使用增加)
        {
            if (replaceText != "")
            {
                while (str.Length > 0 && (replaceText + str + 16).CalculateLabelLength(gUIStyleLabel) > rectWidth)
                {
                    str = str.Remove(0, 1);//将溢出部分删掉(性能消耗过大，研究其他方式)
                }
                str = replaceText + str;
            }
        }

        //——Clip——
        TextClipping textClippingCache = gUIStyleLabel.clipping;
        if (isClipOnOverflow)
        {
            gUIStyleLabel.clipping = TextClipping.Clip;
        }

        Rect labelRect = new Rect(rectToDraw);
        labelRect.height = spaceSize.y;//保持高度不变，方便对齐
        GUI.Label(labelRect, str, gUIStyleLabel);


        if (isClipOnOverflow)
        {
            gUIStyleLabel.clipping = textClippingCache;
        }

        return rectToDraw.GetRemainRect(labelWidth, isAddInterval: false);
    }

    #endregion

    #region HierarchyView Draw

    private static StringBuilder sbProperty = new StringBuilder();
    private static StringBuilder sbType = new StringBuilder();
    public static void DrawHierarchyViewInfo(Rect remainRect, IHierarchyInfo iHierarchyInfo, bool isShowType = true, bool isShowProperty = true)
    {
        //Hierarchy GUI Format： [ Basic Setting | Goup Setting | ID Setting | SubClass Setting ] Type 

        //#Type
        if (isShowType)
        {
            sbType.Length = 0;
            iHierarchyInfo.SetHierarchyGUIType(sbType);
            remainRect = EditorDrawerTool.DrawLabel(sbType.ToString(), remainRect, false);//强制显示Type
        }

        //#Property 
        if (isShowProperty)
        {
            sbProperty.Length = 0;
            iHierarchyInfo.SetHierarchyGUIProperty(sbProperty);
            if (sbProperty.Length > 0)
                sbProperty.Insert(0, "[").Append("]");

            //Warning：Unity2018.4及以上，在最右侧会预留一个预制物的箭头，因此要减去其占用的空间
#if UNITY_2018_4_OR_NEWER
            remainRect.x += buttonSize.x;
            remainRect.width -= buttonSize.x;
#endif

            //EditorDrawerTool.DrawLabel(sbProperty.ToString(), remainRect, false, true);//绘制Property(使用...替换)（Warning效果不好）
            EditorDrawerTool.DrawLabel(sbProperty.ToString(), remainRect, true);//绘制Property（Clip）
        }
    }

    #endregion


    #region StringBuilder

    public static void AppendWarningText(StringBuilder sB, params object[] arrObj)
    {
        sB.AppendRichText("orange", arrObj);
    }

    /// <summary>
    /// 对两个不同的属性进行划分
    /// </summary>
    /// <param name="sB"></param>
    /// <param name="sbOther"></param>
    public static void AddSplit(StringBuilder sB, StringBuilder sbOther)
    {
        if (sB.Length > 0 && sbOther.Length > 0)
        {
            sB.Append("|");
        }
        sB.Append(sbOther);
    }


    #endregion

    #region Multi-Select Version

    /// <summary>
    /// 兼容多选
    /// </summary>
    /// <typeparam name="TComp"></typeparam>
    /// <param name="gUIContent"></param>
    /// <param name="comp"></param>
    /// <param name="getter"></param>
    /// <param name="setter"></param>
    /// <returns></returns>
    public static bool DrawSwitchButton_SupportMultiSelect<TComp>(Object[] targets, GUIContent gUIContent, TComp comp, CustomFunc<TComp, bool> getter, UnityAction<TComp, bool> setter) where TComp : Component
    {
        return DrawElement_SupportMultiSelect(
            targets, comp,
            () => DrawSwitchButton(gUIContent, () => getter(comp)),//暂时不需要调用setter
            getter, setter);
    }

    public static System.Enum DrawEnumPopup_SupportMultiSelect<TComp>(Object[] targets, GUIContent gUIContent, TComp comp, CustomFunc<TComp, System.Enum> getter, UnityAction<TComp, System.Enum> setter) where TComp : Component
    {
        return DrawElement_SupportMultiSelect(
            targets, comp,
            () => DrawEnumPopup(gUIContent, () => getter(comp)),//暂时不需要调用setter
            getter, setter);
    }



    public static TValue DrawElement_SupportMultiSelect<TComp, TValue>(Object[] targets, TComp comp, CustomFunc<TValue> guiDrawData, CustomFunc<TComp, TValue> getter, UnityAction<TComp, TValue> setter) where TComp : Component
    {
        TValue curValue = getter(comp);
        TValue result = guiDrawData();
        if (!curValue.Equals(result))//更新值
        {
            InvokeSelection<TComp>(targets, (c) => setter(c, result));
        }
        return result;
    }

    /// <summary>
    /// 使用Undo记录，针对任意所选组件进行统一调用方法(常用于多选组件然后调用其按键)
    /// Bug: 有问题，暂不使用.原因是默认Inspector的target只绘制最后一个Comp
    /// </summary>
    /// <param name="comp"></param>
    /// <param name="action"></param>
    public static void InvokeSelection<TComp>(Object[] targets, UnityAction<TComp> action) where TComp : class
    {
        //或者改为遍历targets
        //Todelete:改为 https://answers.unity.com/questions/1214493/custompropertydrawer-cant-restrict-multi-editing.html
        Undo.RecordObjects(targets, "Changed Property");
        foreach (Object obj in targets)
        {
            TComp com = obj as TComp;
            if (com != null)
                action.Execute(com);
        }
    }

    #endregion

    #region Utility

    class GUIInfo
    {
        public Color color;
        public Color contentColor;
        public Color backgroundColor;

        public void Record()
        {
            color = GUI.color;
            contentColor = GUI.contentColor;
            backgroundColor = GUI.backgroundColor;
        }
        public void Set()
        {
            GUI.color = color;
            GUI.contentColor = contentColor;
            GUI.backgroundColor = backgroundColor;
        }

    }
    static GUIInfo cacheGUIInfo = new GUIInfo();
    public static void RecordGUIColors()
    {
        cacheGUIInfo.Record();
    }
    public static void RestoreGUIColors()
    {
        cacheGUIInfo.Set();
    }

    public static Texture2D LoadResourcesSprite(string texName)
    {
        var sprite = Resources.Load<Sprite>("Icons/" + texName);
        if (!sprite)
            Debug.LogWarning("Null tex:" + texName);
        return sprite ? sprite.texture : null;
    }


    /// <summary>
    /// 检查指定区域是否被选中
    /// </summary>
    /// <param name="isMouseDown"></param>
    /// <param name="selectionRect"></param>
    /// <param name="buttonIndex"></param>
    /// <param name="extraOnCondition"></param>
    /// <returns></returns>
    public static bool CheckSelect(ref bool isMouseDown, Rect selectionRect, int buttonIndex, System.Func<bool> extraOnCondition = null)
    {
        bool isSelected = false;

        bool isMouseOn = Event.current.type == EventType.MouseDown && Event.current.button == buttonIndex;
        if (extraOnCondition != null)
            isMouseOn &= extraOnCondition();
        if (isMouseOn)
        {
            if (!isMouseDown & selectionRect.Contains(Event.current.mousePosition))
            {
                isMouseDown = true;
                isSelected = true;
            }
        }

        bool isMouseOff = Event.current.type == EventType.MouseUp && Event.current.button == buttonIndex;
        if (isMouseOff)
        {
            isMouseDown = false;
        }
        return isSelected;
    }
    #endregion
}

#endif

/// <summary>
/// 对齐方向
/// </summary>
public enum GUIAlign
{
    Left,
    Right
}
