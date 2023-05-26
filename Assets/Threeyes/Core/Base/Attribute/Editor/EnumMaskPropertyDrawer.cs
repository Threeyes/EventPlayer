// (C) 2014 ERAL
// Distributed under the Boost Software License, Version 1.0.
// (See copy at http://www.boost.org/LICENSE_1_0.txt)

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Threeyes.Editor
{
    namespace Threeyes.CommonEditor
    {
        /// <summary>
        /// Ref from: https://gist.github.com/eral/773d2b289200528d1216
        /// 功能：Unity2020以前带Flag的Enum，需要这个字段来标识
        /// </summary>
        [CustomPropertyDrawer(typeof(EnumMaskAttribute))]
        public class EnumMaskPropertyDrawer : PropertyDrawer
        {

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (SerializedPropertyType.Enum == property.propertyType)
                {
                    object current = GetCurrent(property);
                    if (null != current)
                    {
                        EditorGUI.BeginChangeCheck();
                        var value = EditorGUI.EnumFlagsField(position, label, (System.Enum)current);
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.intValue = System.Convert.ToInt32(value);
                        }
                    }
                }
                else
                {
                    EditorGUI.LabelField(position, label, new GUIContent("This type has not supported."));
                }
            }

            private static object GetCurrent(SerializedProperty property)
            {
                object result = property.serializedObject.targetObject;
                var property_names = property.propertyPath.Replace(".Array.data", ".").Split('.');
                foreach (var property_name in property_names)
                {
                    var parent = result;
                    var indexer_start = property_name.IndexOf('[');
                    if (-1 == indexer_start)
                    {
                        var binding_flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
                        result = parent.GetType().GetField(property_name, binding_flags).GetValue(parent);
                    }
                    else if (parent.GetType().IsArray)
                    {
                        var indexer_end = property_name.IndexOf(']');
                        var index_string = property_name.Substring(indexer_start + 1, indexer_end - indexer_start - 1);
                        var index = int.Parse(index_string);
                        var array = (System.Array)parent;
                        if (index < array.Length)
                        {
                            result = array.GetValue(index);
                        }
                        else
                        {
                            result = null;
                            break;
                        }
                    }
                    else
                    {
                        throw new System.MissingFieldException();
                    }
                }
                return result;
            }
        }
    }
}
#endif
