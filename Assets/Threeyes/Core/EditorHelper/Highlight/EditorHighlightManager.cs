using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Control state for current Scene
/// 
/// (To Add: SOEditorHighlightManager, simulator to SOEventPlayerSettingManager)
/// </summary>
public class EditorHighlightManager : InstanceBase<EditorHighlightManager>
{
    public bool GlobalActive { get { return globalActive; } set { globalActive = value; } }
    [SerializeField] protected bool globalActive = true;
}
