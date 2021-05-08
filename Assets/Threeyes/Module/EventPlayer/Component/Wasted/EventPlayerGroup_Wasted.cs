using System;
using UnityEngine;
using UnityEngine.Events;

namespace Threeyes.EventPlayer {	
#if UNITY_EDITOR
using UnityEditor;
#endif
	
	/// <summary>
	/// Manage child EventPlayer
	/// </summary>
	[Obsolete("Use EventPlayer + isGroup=true instead")]
	public class EventPlayerGroup_Wasted : EventPlayer
	{
	    private void OnValidate()
	    {
	        //Default Set IsGroup as true
	        if (!IsGroup)
	        {
	            IsGroup = true;
	        }
	    }
	}
}
