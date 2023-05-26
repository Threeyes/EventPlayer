#if Threeyes_Timeline
#if UNITY_EDITOR
using System.Collections.Generic;
using Threeyes.Editor;
using UnityEditor;
using UnityEngine;

namespace Threeyes.EventPlayer.Editor
{
    /// <summary>
    /// Bug: 其他绑定的Component可能不会及时更新
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(EventPlayerClip))]
    public class InspectorView_EventPlayerClip : BaseClipEditor<EventPlayerClip>
    {
        private SerializedProperty mserialPropertyEventPlayer = null;
        private static readonly GUIContent tipsLabel = new GUIContent("Event Player", "Event Player / TimelineEventPlayer");

        /// <summary>
        /// 需要排除的物体
        /// </summary>
        /// <returns></returns>
        protected override List<string> GetExcludedPropertiesInInspector()
        {
            List<string> excluded = base.GetExcludedPropertiesInInspector();
            excluded.Add(FieldPath(x => x.eventPlayer));
            return excluded;
        }


        public override void OnInspectorGUI()
        {
            if (serializedObject == null)
                return;

            BeginInspector();

            EditorGUI.indentLevel = 0; // otherwise subeditor layouts get screwed up
            EditorGUI.BeginChangeCheck();

            //Draw EP Field
            mserialPropertyEventPlayer = FindProperty(x => x.eventPlayer);
            EventPlayer ep = mserialPropertyEventPlayer.exposedReferenceValue as EventPlayer;
            if (ep != null)
            {
                //Todo:如果是刚拖进来，那就根据参数设置其长度
                EditorGUILayout.PropertyField(mserialPropertyEventPlayer, tipsLabel);
            }
            else//点击创建EP
            {
                GUIContent createLabel = new GUIContent("Create");
                Vector2 createSize = GUI.skin.button.CalcSize(createLabel);

                Rect rect = EditorGUILayout.GetControlRect(true);
                rect.width -= createSize.x;

                EditorGUI.PropertyField(rect, mserialPropertyEventPlayer, tipsLabel);
                rect.x += rect.width; rect.width = createSize.x;
                if (GUI.Button(rect, createLabel))
                {
                    ep = EventPlayer.CreateEventPlayer(false);
                    mserialPropertyEventPlayer.exposedReferenceValue = ep;
                }
                serializedObject.ApplyModifiedProperties();
            }
            EditorDrawerTool.DrawSpace();

            //只绘制未在Inspector显示的代码.以下代码复制自：DrawRemainingPropertiesInInspector
            DrawPropertiesExcluding(serializedObject, GetExcludedPropertiesInInspector().ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                //EditorUtility.SetDirty(target);//！因为Target是ScriptableObject，不需要调用该方法保存更改
            }

            //只有当EP不为空才绘制其物体所有的Component
            if (ep != null)
                DrawSubEditors(ep);
        }


        int flags;
    }
}
#endif
#endif
