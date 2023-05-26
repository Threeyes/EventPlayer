#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Threeyes.Editor
{
    /// <summary>
    /// 用途：绘制TimelineClip所引用的其他剩余组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseClipEditor<T> : BaseEditor<T> where T : class
    {

        protected virtual void OnDisable()
        {
            DestroyComponentEditors();
        }

        /// <summary>
        /// Draw for ref Component
        /// </summary>
        /// <param name="comp"></param>
        protected void DrawSubEditors(Component comp)
        {
            // Create an editor for each of the cinemachine virtual cam and its components
            GUIStyle foldoutStyle;
            foldoutStyle = EditorStyles.foldout;
            foldoutStyle.fontStyle = FontStyle.Bold;
            UpdateComponentEditors(comp);

            if (m_editors != null)
            {
                foreach (UnityEditor.Editor e in m_editors)
                {
                    if (e == null || e.target == null)
                        continue;

                    EditorDrawerTool.DrawLine();

                    bool expanded = true;
                    if (!s_EditorExpanded.TryGetValue(e.target.GetType(), out expanded))
                        expanded = true;
                    expanded = EditorGUILayout.Foldout(expanded, e.target.GetType().Name, true, foldoutStyle);
                    if (expanded)
                        e.OnInspectorGUI();
                    s_EditorExpanded[e.target.GetType()] = expanded;
                }
            }
        }

        Component m_cachedReferenceObject;
        UnityEditor.Editor[] m_editors = null;
        static Dictionary<System.Type, bool> s_EditorExpanded = new Dictionary<System.Type, bool>();

        void UpdateComponentEditors(Component comp)
        {
            MonoBehaviour[] components = null;
            if (comp != null)
                components = comp.gameObject.GetComponents<MonoBehaviour>();

            int numComponents = (components == null) ? 0 : components.Length;
            int numEditors = (m_editors == null) ? 0 : m_editors.Length;

            if (m_cachedReferenceObject != comp || (numComponents + 1) != numEditors)
            {
                DestroyComponentEditors();
                m_cachedReferenceObject = comp;
                if (comp != null)
                {
                    m_editors = new UnityEditor.Editor[components.Length + 1];

                    DrawTransformEditor(comp, ref m_editors);//不绘制Transform

                    for (int i = 0; i < components.Length; ++i)
                        CreateCachedEditor(components[i], null, ref m_editors[i + 1]);//创建界面（排除Transform）
                }
            }
        }

        protected virtual void DrawTransformEditor(Component comp, ref UnityEditor.Editor[] m_editors)
        {

        }

        protected void DestroyComponentEditors()
        {
            m_cachedReferenceObject = null;
            if (m_editors != null)
            {
                for (int i = 0; i < m_editors.Length; ++i)
                {
                    if (m_editors[i] != null)
                        UnityEngine.Object.DestroyImmediate(m_editors[i]);
                    m_editors[i] = null;
                }
                m_editors = null;
            }
        }
    }
}

#endif