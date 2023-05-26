#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Threeyes.Editor
{
    /// <summary>
    /// PS:因为是通用插件的一部分，所以不依赖于HierarchyViewManager
    /// </summary>
    [InitializeOnLoad]
    public static class HierarchyView_Sequence
    {
        static HierarchyView_Sequence()
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
}
#endif