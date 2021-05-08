using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Using Sequence to manage a set of Data
/// 
/// **Created by Threeyes
/// </summary>
/// <typeparam name="TData"></typeparam>
public class SequenceBase<TData> : SequenceAbstract
    where TData : class
{
    #region Property & Field

    public UnityAction<int, TData> actionSetData;//Which Data get set, use this for coding

    public override int CurIndex { get { return curIndex; } set { curIndex = value; UpdateState(); } }
    public override int Count { get { return listData.Count; } set { /*Add this Set to support SerializeField*/ } }
    public virtual List<TData> ListData { get { return listData; } set { listData = value; UpdateState(); } }
    public TData CurData
    {
        get
        {
            if (CurIndex < ListData.Count)
            {
                return ListData[CurIndex];
            }
            Debug.LogError("Current data is null!");
            return null;
        }
    }


    [Header("Runtime Info")]
    [SerializeField] protected int curIndex = -1;
    [SerializeField] protected List<TData> listData = new List<TData>();

    #endregion

    #region Public Method

    /// <summary>
    /// 激活下一个
    /// </summary>
    public void ActiveNext()
    {
        int nextIndex = ListData.GetIndex(CurIndex + 1);
        if (!IsDataVaild(nextIndex))
            SetDataValid(ListData[nextIndex]);
    }

    /// <summary>
    /// 激活下一个并调用
    /// </summary>
    public void ActiveAndSetNext()
    {
        int nextIndex = ListData.GetIndex(CurIndex + 1);

        if (!IsIndexValid(nextIndex))
        {
            Debug.LogError("越界！ " + nextIndex + "/" + Count);
            return;
        }

        if (!IsDataVaild(nextIndex))
            SetDataValid(ListData[nextIndex]);
        SetDeltaData(+1);
    }

    public void SetDataByIndex(int index)
    {
        SetDataFunc(index);
    }

    public void SetData(TData data)
    {
        int targetIndex = ListData.IndexOf(data);
        if (targetIndex >= 0)
        {
            SetDataFunc(targetIndex);
        }
    }

    #endregion

    #region Inner Method

    /// <summary>
    /// 设置数据可用
    /// </summary>
    /// <param name="data"></param>
    protected virtual void SetDataValid(TData data)
    {
        Debug.LogError("未实现！");
    }

    protected virtual bool IsIndexValid(int index)
    {
        if (!ListData.IsIndexValid(index))
            return false;
        return true;
    }

    /// <summary>
    /// 检查数据是否可用
    /// </summary>
    protected virtual bool IsDataVaild(int index)
    {
        if (!IsIndexValid(index))
            return false;
        TData data = ListData[index];
        if (!IsDataVaild(data))
            return false;

        return true;
    }
    protected virtual bool IsDataVaild(TData data)
    {
        return data != null;
    }

    /// <summary>
    /// 获取队列中间隔为delta的数据
    /// </summary>
    /// <param name="delta">Not 0</param>
    /// <returns></returns>
    protected override bool SetDeltaData(int delta)
    {
        int nextIndex = CurIndex + delta;
        bool isGetNewData = false;
        if (IsLoop)//循环获取
        {
            nextIndex = ListData.GetIndex(nextIndex);
            if (CurIndex == nextIndex)//页数相同（如只有一页）
                return false;

            isGetNewData = true;
        }
        else//普通获取
        {
            if (!ListData.IsIndexValid(nextIndex))//越界
            {
                if (ListData.Count > 0)
                {
                    if (nextIndex == 0 || nextIndex >= ListData.Count)
                        onComplete.Invoke();//所有数据已经调用完毕，到达头/尾部
                }
            }
            else
            {
                isGetNewData = true;
            }
        }

        if (isGetNewData)
        {
            SetDataFunc(nextIndex);
        }
        return isGetNewData;
    }

    /// <summary>
    /// 设置指定序号的数据
    /// </summary>
    /// <param name="index"></param>
    /// <returns>是否成功设置</returns>
    protected override bool SetDataFunc(int index)
    {
        if (!IsDataVaild(index))
        {
            onSetInvalid.Invoke(index);
            return false;
        }

        //调用当前Data的重置方法
        if (IsDataVaild(CurIndex))
        {
            if (CurIndex != index)//避免重复调用
                ResetDataFunc(ListData[CurIndex], index);
        }

        //调用新的Data
        TData data = ListData[index];
        onBeforeSetData.Invoke(index);
        SetDataFunc(data, index);
        CurIndex = index;
        onSetData.Invoke(index);
        actionSetData.Execute(index, data);
        onSet.Invoke();

        if (index == 0) //第一位
        {
            onFirst.Invoke();
        }
        else if (index == ListData.Count - 1)//最后一位
        {
            onFinish.Invoke();//在后期移除
        }
        return true;
    }


    /// <summary>
    /// 处理特定Data
    /// </summary>
    /// <param name="data"></param>
    /// <param name="index"></param>
    protected virtual void SetDataFunc(TData data, int index)
    {

    }

    /// <summary>
    /// 重置特定Data（比如退出界面后重置）
    /// </summary>
    /// <param name="data"></param>
    /// <param name="index"></param>
    protected virtual void ResetDataFunc(TData data, int index)
    {
    }

    #endregion

    #region Utility

    /// <summary>
    /// Update relate state after modify List or Data
    /// </summary>
    protected void UpdateState()
    {
        //PSl 在这个方法不能调用ListData，否则会引起死循环。其他地方应该使用ListData，便于子类进行初始化
        bool canPrevious = false;
        bool canNext = false;

        if (IsLoop)
        {
            canPrevious = true;
            canNext = true;
        }
        else
        {
            canPrevious = CurIndex != 0;
            canNext = CurIndex != listData.Count - 1;
        }

        onCanSetPrevious.Invoke(canPrevious);
        onCanSetNext.Invoke(canNext);
        onSetPageNumberText.Invoke(StrPageNumber);
    }

    #endregion
}