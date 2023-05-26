
using UnityEngine;
/// <summary>
/// 针对多选枚举（枚举要有[System.Flags]标识）
/// 功能：Unity2020以前带Flag的Enum，需要这个字段标识才能绘制多选下来框（尽量不使用，如有必要就将项目转为2020）
/// </summary>
public class EnumMaskAttribute : PropertyAttribute
{
}
