using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 功能：不通过直接引用的方式调用场景中的单例
/// </summary>
public abstract class ComponentRemoteHelperBase<TInst> : MonoBehaviour
{
  public  abstract TInst ManagerInstance { get; }
}
