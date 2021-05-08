#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// inherit to Update Hierarchy
/// </summary>
[CustomEditor(typeof(SequenceAbstract), true)]//editorForChildClasses
public class SequenceInspector : InspectorSyncWithHierarchyBase
{

}
#endif
