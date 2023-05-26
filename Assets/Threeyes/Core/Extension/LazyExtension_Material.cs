using UnityEngine;

public static class LazyExtension_Material
{
    public static string GetPropertyName(this MaterialFloatType materialValueType, string customPropertyName = null)
    {
        switch (materialValueType)
        {
            case MaterialFloatType.Custom:
                return customPropertyName;
            default:
                var attribute = materialValueType.GetAttributeOfType<MaterialPropertyNameAttribute>();
                if (attribute != null)
                    return attribute.PropertyName;

                Debug.LogError(materialValueType.ToString() + " doesn't have " + nameof(MaterialPropertyNameAttribute) + " !");
                return null;
        }
    }
    public static string GetPropertyName(this MaterialColorType colorType, string customPropertyName = null)
    {
        switch (colorType)
        {
            case MaterialColorType.Custom:
                return customPropertyName;
            default:
                var attribute = colorType.GetAttributeOfType<MaterialPropertyNameAttribute>();
                if (attribute != null)
                    return attribute.PropertyName;

                Debug.LogError(colorType.ToString() + " doesn't have " + nameof(MaterialPropertyNameAttribute) + " !");
                return null;
        }
    }
    public static string GetPropertyName(this MaterialTextureType materialTextureType, string customPropertyName = null)
    {
        switch (materialTextureType)
        {
            case MaterialTextureType.Custom:
                return customPropertyName;
            default:
                var attribute = materialTextureType.GetAttributeOfType<MaterialPropertyNameAttribute>();
                if (attribute != null)
                    return attribute.PropertyName;

                Debug.LogError(materialTextureType.ToString() + " doesn't have " + nameof(MaterialPropertyNameAttribute) + " !");
                return null;
        }
    }
}
