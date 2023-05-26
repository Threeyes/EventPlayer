using UnityEngine;
using UnityEngine.Events;
using Threeyes.Pool;
/// <summary>
/// 存储数据，其中TData可以是普通数据类XXXData，或者是继承于ScriptableObject的SOXXXInfo
/// 
/// 提示：如果需要使用多个参数初始化元素，可以将其包在一个XXData数据类中，作为T传送
/// </summary>
public class ElementBase<TData> : MonoBehaviour, IPoolableHandler
	where TData : class
{
	public TData data;
	public UnityEvent onAfterInit;
	public UnityEvent onBeforeDestroy;//BeforeDestroy
	public virtual void Init(TData tempData)
	{
		InitFunc(tempData);
		onAfterInit?.Invoke();
	}

	/// <summary>
	/// 使用已有的数据及参数进行初始化
	/// </summary>
	//[ContextMenu("Init")]
	public virtual void Init()
	{
		Init(data);
	}

	public virtual void InitFunc(TData data)
	{
		this.data = data;
	}

	public virtual void ResetData()
	{

	}

	public virtual void OnBeforeDestroy()
	{
		onBeforeDestroy?.Invoke();//调用相关事件
	}
	protected virtual void OnDestroy()
	{
		OnDespawnFunc();// PS: OnDestroy也可以调用此方法，可避免重复代码
	}

	#region Pool Callback
	/// <summary>
	/// 监听Pool Get时调用的事件
	/// 
	/// PS:
	/// 1.此方法非必须实现，因为已经有Init作为初始化入口
	/// </summary>
	public virtual void OnSpawn()
	{
		OnSpawnFunc();
	}

	/// <summary>
	/// 监听Pool Release时调用的事件
	/// 
	/// PS:
	/// 1.只有使用了对象池时，才需要实现该方法
	/// 2.主要用于：重置数值，去掉引用和委托监听
	/// </summary>
	public virtual void OnDespawn()
	{
		OnDespawnFunc();
	}
	protected virtual void OnSpawnFunc()
	{
	}
	protected virtual void OnDespawnFunc()
	{
	}
	#endregion

	#region Obsolete
	[System.Obsolete("Use onBeforeDestroy Instead")] [HideInInspector] public UnityEvent onDestroy;
#if UNITY_EDITOR
	void OnValidate()
	{
#pragma warning disable CS0618
		Threeyes.Editor.EditorVersionUpdateTool.TransferUnityEvent(this, ref onDestroy, ref onBeforeDestroy);
#pragma warning restore CS0618
	}
#endif
	#endregion

}