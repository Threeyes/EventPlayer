using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
/// <summary>
/// 针对NewInpusSystem
/// </summary>
public static class LazyExtension_InputSystem
{
#if ENABLE_INPUT_SYSTEM //ENABLE_INPUT_SYSTEM and ENABLE_LEGACY_INPUT_MANAGER  https://forum.unity.com/threads/package-dependent-compilation.541894/

    //Ref: https://forum.unity.com/threads/solved-getbuttondown-getbuttonup-with-the-new-system.876451/#post-5764510

    /// <summary>
    /// 任意不为0时的值
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static bool GetButton(this InputAction action)
    {
        return action.ReadValue<float>() > 0;
    }

    ///// <summary>
    ///// 按下及抬起瞬间
    ///// </summary>
    ///// <param name="action"></param>
    ///// <returns></returns>
    //public static bool GetButtonDownUp(this InputAction action)
    //{
    //    return action.triggered && action.ReadValue<float>() > 0;
    //}

    public static bool GetButtonDown(this InputAction action)
    {
        return action.triggered && action.ReadValue<float>() == 1;
    }

    public static bool GetButtonUp(this InputAction action)
    {
        return action.triggered && action.ReadValue<float>() == 0;
    }
#endif
}
