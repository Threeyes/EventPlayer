using System;
using System.Text;
using Threeyes.Editor;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// （Mainly used by SequenceHierarchyView)
/// </summary>
public abstract class SequenceAbstract : MonoBehaviour, IHierarchyViewInfo
{
    #region Networking

    public bool IsCommandMode { get { return isCommandMode; } set { isCommandMode = value; } }//Set to true to invoke UnityAction instead
    private bool isCommandMode = false;
    public UnityAction<int> actCommandSetDelta;
    public UnityAction<int> actCommandSet;
    public UnityAction<int> actCommandReset;

    public UnityAction<int> actRealSetDelta;
    public UnityAction<int> actRealSet;
    public UnityAction<int> actRealReset;

    #endregion

    #region Property & Field

    [HideInInspector] public IntEvent onBeforeSet;//When the index of Data is begin to Init, the param is target index
    [HideInInspector] public IntEvent onSet;//When the target index of element get set (You can use this to get notify by any set too)
    [HideInInspector] public IntEvent onReset;//When the target index of element get reset (You can use this to get notify by any set too)
    [HideInInspector] public IntEvent onSetInvalid;//When target index can't be set, the param is target index (eg: data not valid or the targetIndex is out of range)
    [HideInInspector] public UnityEvent onFirst;//Cur Data is the first one
    [HideInInspector] public UnityEvent onFinish;//Cur Data is the last one
    [HideInInspector] public UnityEvent onComplete;// Occur when cur index is the first/last one, and you Invoke SetPrevious/SetNext, only valiable when isloop=false(use this to point out that there's no more data in the next/previout index)
    [HideInInspector] public BoolEvent onCanSetPrevious;//（use this to control change page button's state）
    [HideInInspector] public BoolEvent onCanSetNext;// (use this to control change page button's state）
    [HideInInspector] public StringEvent onSetPageNumberText;//Set the page Number（eg: 1/10）

    public DataResetType ResetType { get { return resetType; } set { resetType = value; } }
    public bool IsLoop { get { return isLoop; } set { isLoop = value; } }
    public bool IsFirst { get { return CurIndex == 0; } }
    public bool IsLast { get { return CurIndex == Count - 1; } }
    public int CurIndex { get { return curIndex; } set { SetCurIndexFunc(value); } }
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

    [Header("Sequence")]
    [SerializeField] protected bool isLoop = false;//Loop Or Not
    [SerializeField] protected DataResetType resetType = DataResetType.ResetPrevious;
    [SerializeField] protected string formatPageNumber = "{0}/{1}";
    [SerializeField] protected int curIndex = -1;

    #endregion


    #region Public Method

    //——Set Delta——

    [ContextMenu("SetPrevious")]
    public void SetPrevious()
    {
        SetDelta(-1);
    }
    [ContextMenu("SetNext")]
    public void SetNext()
    {
        SetDelta(+1);
    }
    public void SetNext(bool isSetNext)
    {
        SetDelta(isSetNext ? 1 : -1);
    }

    //——Set——

    /// <summary>
    /// 重新设置当前数据，常用于重新开始
    /// </summary>
    public void SetCur()
    {
        Set(CurIndex);
    }
    public void SetFirst()
    {
        Set(0);
    }
    public void SetLastOne()
    {
        Set(Count - 1);
    }
    public void SetRandom()
    {
        if (Count == 0)
            return;
        Set(UnityEngine.Random.Range(0, Count));
    }
    public void SetAll()
    {
        for (int i = 0; i != Count; i++)
            Set(i);
    }
    [System.Obsolete("Use Set instead")]
    public void SetDataByIndex(int index)
    {
        Set(index);
    }


    //——Reset——

    [ContextMenu("ResetCur")]
    public void ResetCur()
    {
        Reset(CurIndex);
    }
    public void ResetAll(bool isResetIndex = true)
    {
        for (int i = 0; i != Count; i++)
            Reset(i);

        if (isResetIndex)
            CurIndex = -1;
    }

    //——Entry method——
    public void SetDelta(int delta)
    {
        if (IsCommandMode)
            actCommandSetDelta.Execute(delta);
        else
        {
            actRealSetDelta.Execute(delta);
            SetDeltaFunc(delta);
        }
    }
    public void Set(int index)
    {
        if (IsCommandMode)
            actCommandSet.Execute(index);
        else
        {
            actRealSet.Execute(index);
            SetFunc(index);
        }
    }
    public void Reset(int index)
    {
        if (IsCommandMode)
            actCommandReset.Execute(index);
        else
        {
            actRealReset.Execute(index);
            ResetFunc(index);
        }
    }

    #endregion

    #region Inner Method

    protected abstract bool SetFunc(int index);
    protected abstract bool SetDeltaFunc(int delta);
    protected abstract bool ResetFunc(int index);
    protected virtual void SetCurIndexFunc(int index)
    {
        curIndex = index;
    }

    #endregion

    #region Define

    /// <summary>
    /// How to reset others when the new element is set
    /// </summary>
    public enum DataResetType
    {
        Null = 0,
        ResetPrevious = 1,//Reset previous Obj（Reset上一个激活的物体）
        ResetAll = 2,//Reset All(Reset所有物体）
        SetAllPreviousAndResetAllNext = 3, //Set all previous and reset all next Objs（Reset在当前Index之后的物体）
    }
    #endregion

    #region Obsolete
    [System.Obsolete("Use onBeforeSet Instead")] [HideInInspector] public IntEvent onBeforeSetData;//When the index of Data is begin to Init, the param is target index
    [System.Obsolete("Use onSet Instead")] [HideInInspector] public IntEvent onSetData;//When the index of Data has been Inited, the param is target index
#if UNITY_EDITOR
    void OnValidate()
    {
#pragma warning disable CS0618 
        EditorVersionUpdateTool.TransferUnityEvent(this, ref onSetData, ref onSet);
        EditorVersionUpdateTool.TransferUnityEvent(this, ref onBeforeSetData, ref onBeforeSet);
#pragma warning restore CS0618
    }
#endif
    #endregion

    #region Editor Method
#if UNITY_EDITOR

    //——Hierarchy GUI——

    //Hierarchy GUI Format： [ Basic Setting | Goup Setting | ID Setting | SubClass Setting ] EventPlayer_Type 
    protected static StringBuilder sbCache = new StringBuilder();
    public virtual string ShortTypeName { get { return ""; } }

    /// <summary>
    /// Set the Type of this
    /// </summary>
    /// <returns></returns>
    public virtual void SetHierarchyGUIType(StringBuilder sB)
    {

    }

    public virtual void SetInspectorGUIUnityEventProperty(GUIPropertyGroup group)
    {
        group.title = "Unity Event";
        group.listProperty.Add(new GUIProperty(nameof(onBeforeSet)));
        group.listProperty.Add(new GUIProperty(nameof(onSet)));
        group.listProperty.Add(new GUIProperty(nameof(onReset)));
        group.listProperty.Add(new GUIProperty(nameof(onSetInvalid)));
        group.listProperty.Add(new GUIProperty(nameof(onFirst)));
        group.listProperty.Add(new GUIProperty(nameof(onFinish)));
        group.listProperty.Add(new GUIProperty(nameof(onComplete)));
        group.listProperty.Add(new GUIProperty(nameof(onCanSetPrevious)));
        group.listProperty.Add(new GUIProperty(nameof(onCanSetNext)));
        group.listProperty.Add(new GUIProperty(nameof(onSetPageNumberText)));
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
