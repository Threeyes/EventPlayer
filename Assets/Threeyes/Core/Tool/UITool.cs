using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UITool
{
    public static bool IsHoveringUI()
    {
        if (EventSystem.current)
            return EventSystem.current.IsPointerOverGameObject();
        return false;
    }
    public static bool IsHoveringUI(GameObject goUIElement)
    {
        if (IsHoveringUI())
        {
            return EventSystem.current.currentSelectedGameObject == goUIElement;
        }
        return false;
    }
}
