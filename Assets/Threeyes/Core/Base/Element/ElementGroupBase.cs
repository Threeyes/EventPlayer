using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// 存储相同的一系列数据
/// </summary>
/// <typeparam name="T"></typeparam>
public class ElementGroupBase<T> : MonoBehaviour where T : class
{
    public List<T> listElement = new List<T>();

    public virtual void ResetData()
    {

    }
}

//适用于带
public abstract class ElementGroupBase<TElement, TEleData> : MonoBehaviour
    where TElement : ElementBase<TEleData>
    where TEleData : class
{
    public List<TElement> listElement = new List<TElement>();

    public virtual void ResetData()
    {

    }

    public virtual void InitElement(TEleData eleData)
    {
        TElement inst = InitElementFunc(eleData);
        AddElementToList(inst);
    }

    protected abstract TElement InitElementFunc(TEleData eleData);

    protected virtual void AddElementToList(TElement element)
    {
        listElement.Add(element);
    }

    protected virtual void RemoveElementFromList(TElement element)
    {
        if (listElement.Contains(element))
        {
            listElement.Remove(element);
        }
    }

}

