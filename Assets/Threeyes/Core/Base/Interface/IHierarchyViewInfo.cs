using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
/// <summary>
/// Display Info in Hierarchy
/// </summary>
public interface IHierarchyViewInfo
{
#if UNITY_EDITOR

    /// <summary>
    /// Type name of this Component for short
    /// </summary>
    string ShortTypeName { get;  }

    /// <summary>
    /// Display Content
    /// </summary>
    /// <param name="sB"></param>
    void SetHierarchyGUIProperty(StringBuilder sB);

#endif
}
