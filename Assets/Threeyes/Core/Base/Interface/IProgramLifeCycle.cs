using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 程序退出的相关事件
/// </summary>
public interface IProgramLifeCycle
{
    /// <summary>
    /// 程序能否退出(需要注意，执行完毕后需要将该值设置为true）
    /// </summary>
    bool CanQuit { get; }

    /// <summary>
    /// 开始退出
    /// </summary>
    void OnQuitEnter();

    /// <summary>
    /// 尝试退出时的多帧更新方法（如进入退出动画）
    /// </summary>
    /// <returns></returns>
    IEnumerator IETryQuit();

    int QuitExecuteOrder { get; }//退出的顺序

}
