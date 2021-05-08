#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Threeyes.EventPlayer
{
    [InitializeOnLoad]
    public static class EventPlayerVersionManager
    {
        public static readonly string EventPlayer_Version = "2.5"; //Plugin Version

        static EventPlayerVersionManager()
        {
            if (SOEventPlayerSettingManager.Instance)
            {
                SOEventPlayerSettingManager.Instance.UpdateVersion(EventPlayer_Version);
            }
        }

    }
}

#endif