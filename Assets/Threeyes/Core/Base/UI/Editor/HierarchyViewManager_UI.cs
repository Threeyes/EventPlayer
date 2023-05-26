#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Threeyes.Editor
{
    [InitializeOnLoad]
    public static class HierarchyViewManager_UI
    {
        static HierarchyViewManager_UI()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (!go)
                return;

            EditorDrawerTool.RecordGUIColors();

            DrawUGUI(selectionRect, go);

            EditorDrawerTool.RestoreGUIColors();
        }

        static void DrawUGUI(Rect selectionRect, GameObject go)
        {
            Button button = go.GetComponent<Button>();
            if (button)
            {
                Rect rectEle = selectionRect.GetAvaliableRect(EditorDrawerTool.buttonSize);

                if (GUI.Button(rectEle, ""))
                    button.onClick.Invoke();
            }

            Toggle toggle = go.GetComponent<Toggle>();
            if (toggle)
            {
                Rect rectEle = selectionRect.GetAvaliableRect(EditorDrawerTool.toggleSize);

                bool cacheToggle = toggle.isOn;
                bool newToggle = GUI.Toggle(rectEle, toggle.isOn, "");
                if (newToggle != cacheToggle)
                    toggle.isOn = newToggle;
            }

            Slider slider = go.GetComponent<Slider>();
            if (slider)
            {
                Rect rectEle = selectionRect.GetAvaliableRect(EditorDrawerTool.sliderSize);

                float cacheValue = slider.value;
                float resultValue = GUI.HorizontalSlider(rectEle, slider.value, slider.minValue, slider.maxValue);
                if (resultValue != cacheValue)
                {
                    Undo.RecordObject(slider, "Change CurValue");
                    slider.value = resultValue;
                }
            }
        }
    }
}
#endif