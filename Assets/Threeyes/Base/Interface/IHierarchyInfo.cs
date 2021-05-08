using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
/// <summary>
/// Display Info in Hierarchy
/// </summary>
public interface IHierarchyInfo
{
#if UNITY_EDITOR
    void SetHierarchyGUIType(StringBuilder sB);
    void SetHierarchyGUIProperty(StringBuilder sB);
#endif
}
