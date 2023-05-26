using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Ping and HighLight target
/// </summary>
public class EditorHighlight : MonoBehaviour
{
    public GameObject GOTarget
    {
        get
        {
            if (!goTarget)
                goTarget = gameObject;
            return goTarget;
        }
        set { goTarget = value; }
    }
    public bool IsActive
    {
        get { return isActive; }
    }

    [SerializeField] protected GameObject goTarget;
    [SerializeField] protected bool isActive = true;

    public void PingAndHighlight()
    {
#if UNITY_EDITOR
        if (!IsActive)
            return;

        PingAndHighlight(GOTarget);
#endif
    }

    public static void PingAndHighlight(Object obj)
    {
#if UNITY_EDITOR
        if (EditorHighlightManager.Instance && !EditorHighlightManager.Instance.GlobalActive)
            return;

        UnityEditor.EditorGUIUtility.PingObject(obj);
        UnityEditor.Selection.activeObject = obj;
#endif
    }

}
