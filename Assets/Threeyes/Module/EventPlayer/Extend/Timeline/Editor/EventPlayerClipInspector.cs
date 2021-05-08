#if UNITY_EDITOR
#if true // Threeyes_Timeline
namespace Threeyes.EventPlayer
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(EventPlayerClip))]
    public class EventPlayerClipInspector : BaseClipEditor<EventPlayerClip>
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
            if (serializedObject != null)
            {
                mserialPropertyEventPlayer = FindProperty(x => x.eventPlayer);
            }
            BeginInspector();
            EditorGUI.indentLevel = 0; // otherwise subeditor layouts get screwed up

            Rect rect;
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

                rect = EditorGUILayout.GetControlRect(true);
                rect.width -= createSize.x;

                EditorGUI.PropertyField(rect, mserialPropertyEventPlayer, tipsLabel);
                rect.x += rect.width; rect.width = createSize.x;
                if (GUI.Button(rect, createLabel))
                {
                    ep = EventPlayer.CreateEventPlayer();
                    mserialPropertyEventPlayer.exposedReferenceValue = ep;
                }
                serializedObject.ApplyModifiedProperties();
            }

            DrawRemainingPropertiesInInspector();

            if (ep != null)
                DrawSubEditors(ep);
        }
    }
}
#endif
#endif
