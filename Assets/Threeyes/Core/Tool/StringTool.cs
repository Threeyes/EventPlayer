using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StringTool
{

    /// <summary>
    /// 从文本中尝试获取序号
    /// </summary>
    /// <param name="content"></param>
    /// <param name="fromRear">从后面开始</param>
    /// <returns></returns>
    public static int? GetIndex(string content)
    {
        string contentResult;
        string strNum = GetStringIndexFunc(content, out contentResult);

        if (strNum.IsNullOrEmpty())
            return null;
        return strNum.TryParse<int>();
    }
    public static string RemoveStringIndex(string content)
    {
        string contentResult;
        GetStringIndexFunc(content, out contentResult);
        return contentResult;
    }


    /// <summary>
    /// 获取字符串中代表序号的文本
    /// </summary>
    /// <param name="content"></param>
    /// <param name="contentResult"></param>
    /// <returns></returns>
    static string GetStringIndexFunc(string content, out string contentResult)
    {
        string rawContent = content;

        int lastValidIndex = -1;//记录上次有效的index，确保获得的数字是连续的
        string strNum = "";//文本中包含的数字，通常在尾部（ABC 01)
        while (content.Count() > 0)
        {
            char tempChar = content.Last();//从后往前

            if (tempChar <= '9' && tempChar >= '0')
            {
                if (strNum.Length == 0)
                {
                    strNum = tempChar.ToString();
                    lastValidIndex = rawContent.IndexOf(tempChar);
                }
                else
                {
                    int indexOfChar = rawContent.IndexOf(tempChar);
                    if (lastValidIndex - indexOfChar > 1)//表明不是连续的序号
                        break;
                    else
                    {
                        strNum = tempChar.ToString() + strNum;
                        lastValidIndex = indexOfChar;
                    }
                }
            }
            else
            {
                break;
            }
            content = content.Substring(0, content.Length - 1);
        }
        contentResult = content;
        return strNum;
    }

}
