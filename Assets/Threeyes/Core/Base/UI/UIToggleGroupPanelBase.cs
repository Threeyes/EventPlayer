using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 通过ToggleGroup管理的Toggle元素
/// Bug:Unity暂不公开toggleGroup中的Toggle列表，只能自己手动引用
/// </summary>
public abstract class UIToggleGroupPanelBase<TUIElement, TData> : UIContentPanelBase<TUIElement, TData>
    where TUIElement : ElementBase<TData>, IUIToggleComponent
    where TData : class
{
    public bool enableReInvokeToggle = true;//允许代码重复调用Toggle，适用于：（如重置时需要重新调用首位元素）

    #region ToggleGroup

    public ToggleGroup toggleGroup;//待绑定的组件
    public ToggleGroup ToggleGroup
    {
        get
        {
            if (!toggleGroup)
                toggleGroup = GetCompFunc();
            return toggleGroup;
        }
        set
        {
            toggleGroup = value;
        }
    }

    protected virtual ToggleGroup GetCompFunc()
    {
        if (this)//避免物体被销毁
            return this.GetComponent<ToggleGroup>();
        return default(ToggleGroup);
    }

    #endregion

    protected abstract Toggle GetToggleAt(TUIElement tUIElement);


    protected override void SetDataFunc(TData data, int index)
    {
        base.SetDataFunc(data, index);
        SetUIToggleState(data, index);
    }

    protected virtual void SetUIToggleState(TData data, int index)
    {
        TUIElement tUIElement = GetUIElementAt(index);
        if (!tUIElement)
            return;
        Toggle toggle = GetToggleAt(tUIElement);
        if (toggle)
        {
            //针对需要重新调用的方法（如重置后默认选中默认的选项)
            if (toggle.isOn)
                toggle.isOn = false;
            toggle.isOn = true;
        }
        else
        {
            Debug.LogError("无法获取对应的Toggle！");
        }
    }
}
public interface IUIToggleComponent
{
    Toggle Toggle { get; }
}