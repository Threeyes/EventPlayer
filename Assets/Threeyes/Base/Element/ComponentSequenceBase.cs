using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Sequence For Component
/// </summary>
/// <typeparam name="TComp"></typeparam>
public class ComponentSequenceBase<TComp> : SequenceBase<TComp>
    where TComp : Component
{
    #region Property & Field

    protected bool IsLoadChildOnAwake { get { return isLoadChildOnAwake; } set { isLoadChildOnAwake = value; } }

    public override List<TComp> ListData
    {
        get
        {
            return listData;
        }

        set
        {
            base.ListData = value;
        }
    }


    [SerializeField] protected bool isLoadChildOnAwake = true;//Auto load child's Comp

    #endregion

    #region Inner Method

    /// <summary>
    /// GetComponentFromChild
    /// </summary>
    /// <returns></returns>
    protected virtual List<TComp> GetComponentFromChild()
    {
        List<TComp> tempList = new List<TComp>();
        //Only get the first layer by default, including hiding object
        foreach (Transform tfChild in transform)
        {
            var comp = tfChild.GetComponent<TComp>();
            if (comp)
            {
                tempList.Add(comp);
            }
        }
        return tempList;
    }

    #endregion


    #region Unity Method

    protected virtual void Awake()
    {
        if (IsLoadChildOnAwake)
        {
            listData.AddRange(GetComponentFromChild());
            UpdateState();
        }

    }
    #endregion

}
