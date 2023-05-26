using System.Reflection;
using System;
using System.Runtime.Serialization;
using UnityEngine;
#if USE_JsonDotNet
using Newtonsoft.Json;
#endif

public static class JsonDotNetTool
{
    /// <summary>
    /// �ж�class�е�Member�Ƿ�Ӧ�����л�
    /// Ref��https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_MemberSerialization.htm
    /// </summary>
    /// <param name="objectType"></param>
    /// <param name="memberInfo"></param>
    /// <returns></returns>
    public static bool ShouldSerialize(Type objectType, MemberInfo memberInfo)
    {
#if USE_JsonDotNet

        bool ignoreSerializableAttribute;
#if HAVE_BINARY_SERIALIZATION
            ignoreSerializableAttribute = true;
#else
        ignoreSerializableAttribute = true;
#endif
        return ShouldSerialize(GetObjectMemberSerialization(objectType, ignoreSerializableAttribute), memberInfo);
#else
        Debug.LogError("Please Active USE_JsonDotNet!");
        return true;
#endif

    }

#if USE_JsonDotNet

    /// <summary>
    /// ��ȡ�ඨ��[JsonObject]�е�MemberSerialization
    /// 
    /// Ref: Newtonsoft.Json.Serialization.JsonTypeReflector#154
    /// </summary>
    /// <param name="objectType"></param>
    /// <param name="ignoreSerializableAttribute"></param>
    /// <returns></returns>
    public static MemberSerialization GetObjectMemberSerialization(Type objectType, bool ignoreSerializableAttribute)
    {
        JsonObjectAttribute objectAttribute = objectType.GetCustomAttribute<JsonObjectAttribute>();
        if (objectAttribute != null)
        {
            return objectAttribute.MemberSerialization;
        }
#if HAVE_DATA_CONTRACTS//[MS]
        DataContractAttribute dataContractAttribute = objectType.GetCustomAttribute<DataContractAttribute>();
        if (dataContractAttribute != null)
        {
            return MemberSerialization.OptIn;
        }
#endif

#if HAVE_BINARY_SERIALIZATION
            if (!ignoreSerializableAttribute && IsSerializable(objectType))
            {
                return MemberSerialization.Fields;
            }
#endif

        // the default
        return MemberSerialization.OptOut;
    }

    //Ref: Newtonsoft.Json.Serialization.DefaultContractResolver#1583
    public static bool ShouldSerialize(MemberSerialization memberSerialization, MemberInfo memberInfo)
    {
#if HAVE_DATA_CONTRACTS
        DataContractAttribute dataContractAttribute = memberInfo.GetCustomAttribute<DataContractAttribute>();
        DataMemberAttribute dataMemberAttribute = null;
        if (dataContractAttribute != null && memberInfo != null)
        {
            dataMemberAttribute = memberInfo.GetCustomAttribute<DataMemberAttribute>();
        }
#endif

        JsonPropertyAttribute propertyAttribute = memberInfo.GetCustomAttribute<JsonPropertyAttribute>();
        JsonRequiredAttribute requiredAttribute = memberInfo.GetCustomAttribute<JsonRequiredAttribute>();
        bool hasMemberAttribute = false;//�Ƿ�ʹ����JsonPropertyAttribute��JsonRequiredAttribute
        if (propertyAttribute != null)
        {
            hasMemberAttribute = true;
        }
        else
        {
#if HAVE_DATA_CONTRACTS
                if (dataMemberAttribute != null)
                {
                    hasMemberAttribute = true;
                }
#endif
        }

        if (requiredAttribute != null)
        {
            hasMemberAttribute = true;
        }

        bool hasJsonIgnoreAttribute =
              memberInfo.GetCustomAttribute<JsonIgnoreAttribute>() != null
              // automatically ignore extension data dictionary property if it is public
              || memberInfo.GetCustomAttribute<JsonExtensionDataAttribute>() != null
#if HAVE_NON_SERIALIZED_ATTRIBUTE
                || IsNonSerializable(memberInfo)
#endif
                ;

        bool Ignored;

        if (memberSerialization != MemberSerialization.OptIn)
        {
            bool hasIgnoreDataMemberAttribute = false;

#if HAVE_IGNORE_DATA_MEMBER_ATTRIBUTE
                hasIgnoreDataMemberAttribute = (memberInfo.GetCustomAttribute<IgnoreDataMemberAttribute>() != null);
#endif

            // ignored if it has JsonIgnore or NonSerialized or IgnoreDataMember attributes
            Ignored = (hasJsonIgnoreAttribute || hasIgnoreDataMemberAttribute);
        }
        else//[OptIn]: Only members marked with JsonPropertyAttribute or DataMemberAttribute are serialized. This member serialization mode can also be set by marking the class with DataContractAttribute.
        {
            // ignored if it has JsonIgnore/NonSerialized or does not have DataMember or JsonProperty attributes
            Ignored = (hasJsonIgnoreAttribute || !hasMemberAttribute);
        }

        return !Ignored;
    }


    public static bool IsSerializable(Type objectType)
    {
        return objectType.GetCustomAttribute<SerializableAttribute>() != null;
    }
    public static bool IsNonSerializable(MemberInfo memberInfo)
    {
        return memberInfo.GetCustomAttribute<NonSerializedAttribute>() != null;
    }
#endif
}
