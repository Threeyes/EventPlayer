using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Threeyes.EventPlayer
{
    public static class EditorDefinition_EventPlayer
    {
        //——Asset Menu——
        public const string AssetMenuPrefix_EventPlayer = "EventPlayer/";

        public const string AssetMenuPrefix_Action_Coroutine = AssetMenuPrefix_EventPlayer + "Coroutine/";
        public const string AssetMenuPrefix_Action_Param = AssetMenuPrefix_EventPlayer + "Param/";   
        public const string AssetMenuPrefix_Action_Sequence = AssetMenuPrefix_EventPlayer + "Sequence/";
    }
}