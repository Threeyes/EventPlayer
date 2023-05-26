using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
/// <summary>
/// 实现分页，每页呈现固定上限的元素
/// </summary>
/// <typeparam name="TPageData"></typeparam>
/// <typeparam name="TElement"></typeparam>
public class UISplitPageBase<TPageData, TElement> : SequenceBase<TPageData>
        where TPageData : PageContent<TElement>, new()
{
    [Header("分页")]
    public List<TElement> listElement = new List<TElement>();//每页的元素
    public bool isSplitPage = false;//是否分页
    [Range(1, 10000)]
    public int numberPerPage = 6;//每页数量
    public int SumPage { get { return Mathf.CeilToInt((float)listElement.Count / numberPerPage); } }
    public int curPageIndex;

    /// <summary>
    /// 将一系列元素整合到自定义的Page类中
    /// </summary>
    /// <param name="tempListElement"></param>
    public void ConvertData(List<TElement> tempListElement)
    {
        listElement = tempListElement;
        List<TPageData> listPage = new List<TPageData>();
        if (tempListElement.Count > 0)
        {
            for (int index = 0; index != SumPage; index++)
            {
                int startIndex = index * numberPerPage;
                int lastIndex = Mathf.Min(startIndex + (numberPerPage - 1), tempListElement.Count - 1);
                var listContent = tempListElement.GetRange(startIndex, (lastIndex - startIndex) + 1);
                listPage.Add(new TPageData()
                {
                    listElement = listContent
                });
            }
        }
        else
        {
            listPage.Add(new TPageData());//添加空元素
        }

        ListData = listPage;
    }

    //public List<TElement> GetPageDataAtIndex(int index)
    //{
    //    List<TElement> listContent = new List<TElement>();

    //    int startIndex = index * numberPerPage;
    //    int lastIndex = Mathf.Min(startIndex + (numberPerPage - 1), listContent.Count - 1);
    //    listContent = ListData.GetRange()
    //    return listContent;
    //}

}
/// <summary>
/// 页中的内容
/// </summary>
/// <typeparam name="TElement"></typeparam>
public class PageContent<TElement>
{
    public List<TElement> listElement = new List<TElement>();
}

