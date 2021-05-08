using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public abstract class SequenceAbstract : MonoBehaviour, IHierarchyInfo
{
    #region Property & Field

    //[Header("Data")]
    public IntEvent onBeforeSetData;//When the index of Data is begin to Init, the param is target index
    public IntEvent onSetData;//When the index of Data has been Inited, the param is target index
    public UnityEvent onSet;//When Any Data get set
    public IntEvent onSetInvalid;//When target index can't be set, the param is target index (eg: data not valid or the targetIndex is out of range)

    //[Header("State")]
    public UnityEvent onFirst;//Cur Data is the first one
    public UnityEvent onFinish;//Cur Data is the last one
    public BoolEvent onCanSetPrevious;//（use this to control change page button's state）
    public BoolEvent onCanSetNext;// (use this to control change page button's state）
    public UnityEvent onComplete;// Occur when cur index is the first/last one, and you Invoke SetPrevious/SetNext, only valiable when isloop=false(use this to point out that there's no more data in the next/previout index)
    public StringEvent onSetPageNumberText;//Set the page Number（eg: 1/10）


    public bool IsLoop { get { return isLoop; } set { isLoop = value; } }
    public bool IsFirst { get { return CurIndex == 0; } }
    public bool IsLast { get { return CurIndex == Count - 1; } }
    public abstract int CurIndex { get; set; }
    public abstract int Count { get; set; }

    public string StrPageNumber
    {
        get
        {
            string result = "";
            int curIndex = Mathf.Clamp(CurIndex, 0, Count) + 1;
            try
            {
                result = string.Format(formatPageNumber, curIndex, Count);
            }
            catch (System.Exception e)
            {
                Debug.LogError("The Foramt is incorrect!\r\n" + e);
                result = string.Format("{0}/{1}", curIndex, Count); ;
            }
            return result;
        }
    }//Page number


    [SerializeField] protected bool isLoop = false;//Loop Or Not
    [SerializeField] protected string formatPageNumber = "{0}/{1}";

    #endregion

    #region Public Method

    //——Set——

    /// <summary>
    /// 第一个
    /// </summary>
    public void SetFirst()
    {
        SetDataFunc(0);
    }

    public void SetLastOne()
    {
        SetDataFunc(Count - 1);
    }

    /// <summary>
    /// 重设当前数据，常用于重新开始
    /// </summary>
    public void SetCur()
    {
        SetDataFunc(CurIndex);
    }

    [ContextMenu("SetPrevious")]
    public void SetPrevious()
    {
        SetDeltaData(-1);
    }

    public void SetNext(bool isSetNext)
    {
        SetDeltaData(isSetNext ? 1 : -1);
    }

    /// <summary>
    /// 下一个
    /// </summary>
    [ContextMenu("SetNext")]
    public void SetNext()
    {
        SetDeltaData(+1);
    }

    #endregion

    #region Inner Method

    protected abstract bool SetDeltaData(int delta);
    protected abstract bool SetDataFunc(int index);

    #endregion

    #region Editor Method
#if UNITY_EDITOR

    //——Hierarchy GUI——

    //Hierarchy GUI Format： [ Basic Setting | Goup Setting | ID Setting | SubClass Setting ] EventPlayer_Type 
    protected static StringBuilder sbCache = new StringBuilder();

    /// <summary>
    /// Set the Type of this EventPlayer
    /// </summary>
    /// <returns></returns>
    public virtual void SetHierarchyGUIType(StringBuilder sB)
    {

    }

    /// <summary>
    /// Display Property of this EventPlayer
    /// </summary>
    /// <param name="texProperty"></param>
    /// <param name="texEPType"></param>
    public virtual void SetHierarchyGUIProperty(StringBuilder sB)
    {
        if (IsLoop)
        {
            sB.Append("Ⓛ ");
        }
        sB.Append(StrPageNumber);
    }

#endif
    #endregion
}
