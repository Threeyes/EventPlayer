#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Threeyes.Extension;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class SequenceHierarchyView
{
    static SequenceHierarchyView()
    {
        //Delegate for OnGUI events for every visible list item in the HierarchyWindow.
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
    }

    private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (!go)
            return;

        SequenceAbstract comp = go.GetComponent<SequenceAbstract>();
        if (comp)
        {
            EditorDrawerTool.RecordGUIColors();

            Rect remainRect = DrawButton(selectionRect, comp);
            EditorDrawerTool.DrawHierarchyViewInfo(remainRect, comp);

            EditorDrawerTool.RestoreGUIColors();
        }
    }

    private static StringBuilder sbProperty = new StringBuilder();
    private static Rect DrawButton(Rect selectionRect, SequenceAbstract comp)
    {
        Rect remainRect = selectionRect;
        Rect rectEle = remainRect.GetAvaliableRect(EditorDrawerTool.buttonSize);
        if (EditorDrawerTool.DrawButton(rectEle, EditorDrawerTool.TexArrRightIcon))
            comp.SetNext();
        rectEle = remainRect.GetRemainRect(rectEle).GetAvaliableRect(EditorDrawerTool.buttonSize);
        if (EditorDrawerTool.DrawButton(rectEle, EditorDrawerTool.TexArrLeftIcon))
            comp.SetPrevious();

        //计算剩余可用长度
        remainRect = remainRect.GetRemainRect(EditorDrawerTool.buttonSize.x * 2 + 4);
        return remainRect;
    }
}
#endif