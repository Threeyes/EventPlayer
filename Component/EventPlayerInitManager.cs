using System.Collections;
using System.Collections.Generic;
using Threeyes.Core;
using Threeyes.EventPlayer;
using UnityEngine;
namespace Threeyes.EventPlayer
{
    /// <summary>
    /// Force All child EP to Init, useful for EP with ID that is hidden
    /// </summary>
    [DefaultExecutionOrder(-23000)]
    [AddComponentMenu(EditorDefinition_EventPlayer.AssetMenuPrefix_EventPlayer + "EventPlayerInitManager", -99)]
    public class EventPlayerInitManager : ComponentGroupBase<EventPlayer>
    {
        private void Awake()
        {
            Init();
        }
        public void Init()
        {
            ForEachChildComponent((ep) => ep.Init());
        }
    }
}