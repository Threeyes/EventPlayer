using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 基于Content的元素
/// </summary>
/// <typeparam name="TData"></typeparam>
public class ContentElementBase<TUIManager, TElement, TData> : ElementBase<TData>, IContentElement<TUIManager>
    where TUIManager : UIContentPanelBase<TElement, TData>
    where TElement : ElementBase<TData>
        where TData : class
{
    public TUIManager UIManager
    {
        get
        {
            return uIManager;
        }
        set
        {
            uIManager = value;
        }
    }
    [SerializeField] protected TUIManager uIManager;


    /// <summary>
    /// Index in content
    /// </summary>
    public int Index
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
        }
    }

    [SerializeField] protected int index;
}

public interface IContentElement<TUIManager>
{
    TUIManager UIManager { get; set; }
    int Index { get; set; }
}
