using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// PS: 该接口的作用是便于EventPlayerInspector鉴别EP类型
/// </summary>
namespace Threeyes.EventPlayer
{
    public interface IEventPlayerWithParam
    {
        bool IsPlayWithParam { get; set; }
        bool IsStopWithParam { get; set; }

        string ValueToString { get; }//Editor Display usage

    }
}
