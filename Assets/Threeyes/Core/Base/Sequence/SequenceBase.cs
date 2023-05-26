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
public class SequenceBase<TData> : SequenceAbstract, IEnumerable<TData>
    where TData : class
{
    #region Property & Field

    public UnityAction<int, TData> actionSetData;//Invoke when Data get set, use this for coding
    public UnityAction<int, TData> actionResetData;//Invoke when Data get reset, use this for coding
    public UnityAction<int, TData, bool> actionSetResetData;//Invoke when Data get set/reset, use this for coding(bool means get set/reset)

    public override int Count { get { return listData.Count; } set { /*Add this Null Set to support SerializeField*/ } }//Warning:不能是ListData.Count,因为会导致死循环而闪退
    public virtual List<TData> ListData { get { return listData; } set { listData = value; UpdateState(); } }
    public TData CurData
    {
        get
        {
            if (CurIndex > 0 && CurIndex < ListData.Count)
            {
                return ListData[CurIndex];
            }
            //Debug.LogError("Current data is null!");
            return null;
        }
    }


    //[Header("Runtime Info")]
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

    public void Active(int index)
    {
        if (!IsDataVaild(index))
            SetDataValid(ListData[index]);
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
        SetDelta(+1);
    }

    public void SetData(TData data)
    {
        if (data == null)
        {
            Debug.LogError("The data is null!");
            return;
        }
        int targetIndex = ListData.IndexOf(data);
        if (targetIndex >= 0)
        {
            Set(targetIndex);
        }
        else
        {
            Debug.LogError("The data doesn't exist in List!");
        }
    }

    public int FindIndexForData(TData data)
    {
        return ListData.IndexOf(data);
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
    protected override bool SetDeltaFunc(int delta)
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
            Set(nextIndex);//（Network）保证调用指定的index
        }
        return isGetNewData;
    }

    /// <summary>
    /// 设置指定序号的数据
    /// </summary>
    /// <param name="index"></param>
    /// <returns>是否成功设置</returns>
    protected override bool SetFunc(int index)
    {
        if (!IsDataVaild(index))//指定Index无效
        {
            onSetInvalid.Invoke(index);
            return false;
        }

        //调用其他Data的Set/Reset方法
        switch (ResetType)
        {
            case DataResetType.ResetPrevious://重置旧Index
                if (IsDataVaild(CurIndex) && (CurIndex != index))
                {
                    ResetDataFunc(ListData[CurIndex], CurIndex);
                }
                break;
            case DataResetType.ResetAll://重置所有
                for (int tempIndex = 0; tempIndex != ListData.Count; tempIndex++)
                {
                    if (IsDataVaild(tempIndex) && (tempIndex != index))
                        ResetDataFunc(ListData[tempIndex], tempIndex);
                }
                break;
            case DataResetType.SetAllPreviousAndResetAllNext:
                for (int tempIndex = 0; tempIndex != ListData.Count; tempIndex++)
                {
                    if (IsDataVaild(tempIndex))
                        if (tempIndex < index)
                        {
                            SetDataFunc(ListData[tempIndex], tempIndex);
                        }
                        else
                        {
                            ResetDataFunc(ListData[tempIndex], tempIndex);
                        }
                }
                break;
        }

        //调用新的Data
        //PS:UnityEvent只针对当前的Data，actionSetData才是针对所有Data
        CurIndex = index;
        TData data = ListData[index];
        onBeforeSet.Invoke(index);
        SetDataFunc(data, index);
        onSet.Invoke(index);

        if (index == 0) //第一位
        {
            onFirst.Invoke();
        }
        if (index == ListData.Count - 1)//最后一位
        {
            onFinish.Invoke();//在后期移除
        }
        return true;
    }

    protected override bool ResetFunc(int index)
    {
        if (!IsDataVaild(index))
        {
            return false;
        }

        onReset.Invoke(index);
        ResetDataFunc(ListData[index], index);
        return true;
    }

    /// <summary>
    /// 处理特定Data
    /// </summary>
    /// <param name="data"></param>
    /// <param name="index"></param>
    protected virtual void SetDataFunc(TData data, int index)
    {
        actionSetData.Execute(index, data);
        actionSetResetData.Execute(index, data, true);
    }

    /// <summary>
    /// 重置特定Data（比如退出界面后重置）
    /// </summary>
    /// <param name="data"></param>
    /// <param name="index"></param>
    protected virtual void ResetDataFunc(TData data, int index)
    {
        actionResetData.Execute(index, data);
        actionSetResetData.Execute(index, data, false);
    }

    protected override void SetCurIndexFunc(int index)
    {
        base.SetCurIndexFunc(index);
        UpdateState();
    }
    #endregion

    #region Interface

    public IEnumerator<TData> GetEnumerator()
    {
        return ListData.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ListData.GetEnumerator();
    }

    #endregion

    #region Utility

    //
    /// <summary>
    /// 【测试中】让用户自行初始化UnityEvent，避免随物体生成或AddComent时UnityEvent未初始化导致报错（https://forum.unity.com/threads/unity-event-is-null-right-after-addcomponent.819402/）
    /// </summary>
    public void InitUnityEvent()
    {
        onBeforeSet = new IntEvent();
        onSet = new IntEvent();
        onReset = new IntEvent();
        onSetInvalid = new IntEvent();
        onFirst = new UnityEvent();
        onFinish = new UnityEvent();
        onComplete = new UnityEvent();
        onCanSetPrevious = new BoolEvent();
        onCanSetNext = new BoolEvent();
        onSetPageNumberText = new StringEvent();
    }

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

            if (listData.Count <= 1)
                canPrevious = canNext = false;
        }
        else
        {
            canPrevious = CurIndex != 0;
            canNext = CurIndex != listData.Count - 1;
        }

        //Bug：Unity2018通过Hierarchy菜单创建会报错，但是整体不影响使用（原因：当Gameobject首次被创建时，onCanSetPrevious等UnityEvent为null）
        onCanSetPrevious?.Invoke(canPrevious);
        onCanSetNext?.Invoke(canNext);
        onSetPageNumberText?.Invoke(StrPageNumber);
    }
    #endregion
}