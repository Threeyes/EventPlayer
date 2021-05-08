#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
namespace Threeyes.Extension
{
    public static class EditorGUILazyExtension
    {
        public static Vector2 spaceSize { get { return EditorDrawerTool.spaceSize; } }//每个元素的间隔（行高）
        public static Vector2 intervalSize { get { return EditorDrawerTool.intervalSize; } }//默认的间隔

        #region Rect

        /// <summary>
        /// 获取可用的区域
        /// </summary>
        /// <param name="selectionRect">总区域</param>
        /// <param name="sizeToUse">右侧需要占用的空间，比如右边的按钮</param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static Rect GetAvaliableRect(this Rect selectionRect, Vector2 sizeToUse, Vector2 interval = default(Vector2), TextAlignment align = TextAlignment.Right)
        {
            Rect rectLeft = new Rect(selectionRect);//selectionRect 为有效宽度
            rectLeft.size = selectionRect.size.x < sizeToUse.x ? selectionRect.size : sizeToUse;

            if (interval == default(Vector2))
                interval = EditorDrawerTool.intervalSize;

            if (align == TextAlignment.Right)
            {
                rectLeft.x = selectionRect.max.x - sizeToUse.x - interval.x;//  从右侧计算x的位置
            }
            else if (align == TextAlignment.Left)
            {
                rectLeft.x += interval.x;
            }
            else
            {
                Debug.LogError("Not Define!");
            }
            rectLeft.y = selectionRect.y + (selectionRect.height - sizeToUse.y) / 2;//Center
            return rectLeft;
        }

        /// <summary>
        /// 计算剩余的区域
        /// </summary>
        /// <param name="sumSpace"></param>
        /// <param name="usedSpace"></param>
        /// <param name="align"></param>
        /// <param name="isAddInterval"></param>
        /// <returns></returns>
        public static Rect GetRemainRect(this Rect sumSpace, Rect usedSpace, GUIAlign align = GUIAlign.Right, bool isAddInterval = true)
        {
            float rawSumSpaceX = sumSpace.x;
            if (align == GUIAlign.Left)
            {
                sumSpace.x = usedSpace.x;
                if (isAddInterval)
                    sumSpace.x -= intervalSize.x;
                sumSpace.width -= sumSpace.x - rawSumSpaceX;
            }
            else
            {
                sumSpace.width -= usedSpace.width;
                if (isAddInterval)
                    sumSpace.width -= intervalSize.x;
            }
            return sumSpace;
        }

        public static Rect GetRemainRect(this Rect sumSpace, float usedWidth, GUIAlign align = GUIAlign.Right, bool isAddInterval = true)
        {
            if (isAddInterval)
            {
                usedWidth += intervalSize.x;
            }

            if (align == GUIAlign.Left)
                sumSpace.x += usedWidth;
            sumSpace.width -= usedWidth;
            return sumSpace;
        }

        /// <summary>
        ///  计算除去名字Label后的剩余空间
        /// </summary>
        /// <param name="selectionRect"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public static Rect GetRemainRectWithoutNameLabel(this Rect selectionRect, Component component, float space = 2)
        {
            float nameLabelWidth = CalculateLabelLength(component.name) + space/* spaceSize.x*/;//左侧物体Name Label + 空间
            return GetRemainRect(selectionRect, nameLabelWidth, GUIAlign.Left);
        }

        static List<char> listCharHalfAngle = new List<char>()
        {
         '[',
         ']',
         '{',
         '}',
         '|',
         ',',
         '.'
        };
        static int halfAngleCount;
        static Vector2 labelRect;
        public static float CalculateLabelLength(this string text, GUIStyle guiStyle = null, bool isReplaceHalfAngle = true)
        {
            if (guiStyle == null)
                guiStyle = new GUIStyle();

            labelRect = guiStyle.CalcSize(new GUIContent(text));

            float rawWidth = labelRect.x;
            //（统计半角字符数，然后减少对应长度（减半））（https://baike.baidu.com/item/%E5%8D%8A%E8%A7%92%E5%AD%97%E7%AC%A6/758199?fr=aladdin）
            if (isReplaceHalfAngle)
            {
                halfAngleCount = System.Array.FindAll(text.ToCharArray(), (c) => listCharHalfAngle.Contains(c)).Length;
                labelRect.x -= halfAngleCount * guiStyle.fontSize / 2;//半角字符减半
            }
            return labelRect.x;
        }

        #endregion
    }
}
#endif