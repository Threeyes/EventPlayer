using System;
using UnityEngine;

/// <summary>
/// 根据当前RenderingPipeline，返回正确的值
/// 
/// PS：
/// 1.Inspector Debug模式及EditShader可以看到对应的名称
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class MaterialPropertyNameAttribute : PropertyAttribute
{
    public string PropertyName
    {
        get
        {
#if UNITY_PIPELINE_URP
            return urpPropertyName;
#elif UNITY_PIPELINE_HDRP
                return hdrpPropertyName;
#else
                return builtInRPPropertyName;
#endif
        }
    }

    public string builtInRPPropertyName;
    public string urpPropertyName;
    public string hdrpPropertyName;

    public MaterialPropertyNameAttribute(string builtInRPPropertyName, string urpPropertyName, string hdrpPropertyName)
    {
        //PS:如果无此字段，直接设置为null
        this.builtInRPPropertyName = builtInRPPropertyName;
        this.urpPropertyName = urpPropertyName;
        this.hdrpPropertyName = hdrpPropertyName;
    }
}