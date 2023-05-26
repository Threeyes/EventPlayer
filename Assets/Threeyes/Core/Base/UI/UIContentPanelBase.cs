using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Threeyes.Pool;
/// <summary>
/// 管理动态生成的多个内容（如Scroll Panel）
/// </summary>
/// <typeparam name="TUIElement">实例化元素</typeparam>
/// <typeparam name="TData">元素数据</typeparam>
public abstract class UIContentPanelBase<TUIElement, TData> : SequenceBase<TData>, IShowHide
    where TUIElement : ElementBase<TData>
    where TData : class
{
    public virtual Transform TfContentParent { get { return tfContentParent; } }//待实例化元素的父物体
    public virtual GameObject PreUIElement { get { return preUIElement; } set { preUIElement = value; } }//元素预制物
    [HideInInspector] public UnityEvent onInitAllUIElement;//生成所有元素时调用
    [HideInInspector] public UnityEvent onInitUIElement;//生成新元素时调用

    [Header("UI Init")]
    public bool isHideOnStart = true;//开始时隐藏
    //初始化设置（2选1）
    public bool isInitOnStart = true;//是否使用已有数据生成元素  （设置为false可保留已存在的元素）
    public bool isSetupOnStart = true;//是否针对已有元素进行初始化
    public Transform tfContentParent;//元素的父物体
    public GameObject preUIElement;//元素预制物

    [Header("Pool")]
    public bool usePool = false;//Pool支持，默认为false
    public int defaultCapacity = 10;
    public int maxSize = 100;

    [Header("Runtime")]
    public List<TUIElement> listUIElement = new List<TUIElement>();//缓存动态实例化后的元素组件

    protected virtual ObjectPool<GameObject> Pool//PS:子类可更换成自己喜欢的Pool类型
    {
        get
        {
            if (pool == null)
            {
                pool = new GameObjectPool(createFunc: () => Instantiate(preUIElement), defaultCapacity: defaultCapacity, maxSize: maxSize);
            }
            return pool;
        }
    }
    ObjectPool<GameObject> pool;

    #region Unity Method

    protected virtual void Start()
    {
        if (isInitOnStart)//使用已有数据生成元素
        {
            Init();
        }
        else if (isSetupOnStart)//针对已有元素进行初始化
        {
            SetUpExistUI();
        }

        if (isHideOnStart)//（创建以后隐藏）
            Hide();
    }

    private void OnDestroy()
    {
        if (usePool && pool != null)
            pool.Dispose();
    }
    #endregion

    #region Public Method

    /// <summary>
    /// 通过传递的数据，生成UI
    /// </summary>
    /// <param name="tempListData"></param>
    public virtual void Init(List<TData> tempListData)
    {
        ListData = tempListData;
        Init();
    }

    /// <summary>
    /// 使用已有的数据，生成UI
    /// </summary>
    public virtual void Init()
    {
        //重置元素和数据
        ResetData();
        ResetUI();

        //初始化UI
        InitUI();
    }

    /// <summary>
    /// 重设数据（ToUpdate:整合在ResetUI中）
    /// </summary>
    public virtual void ResetData()
    {
        listUIElement.Clear();
    }

    /// <summary>
    /// 删除所有元素
    /// </summary>
    public virtual void ResetUI()
    {
        while (TfContentParent.childCount > 0)//使用While而不是foreach，避免访问越界
        {
            Transform tfSon = TfContentParent.GetChild(0);
            DestroyElementFunc(tfSon.gameObject);//PS:初始化调用时，可能物体只是临时物体，所以不能通过获取TUIElement的方式删除
        }
    }

    /// <summary>
    /// 移除指定UI元素
    /// </summary>
    /// <param name="element"></param>
    public virtual void RemoveElement(TUIElement element)
    {
        if (!element)//元素为空
            return;
        if (ListData.Contains(element.data))
            ListData.Remove(element.data);
        if (listUIElement.Contains(element))
            listUIElement.Remove(element);

        DestroyElementFunc(element.gameObject);
    }


    /// <summary>
    /// 针对已有的元素，使用其自身data进行初始化
    /// </summary>
    public virtual void SetUpExistUI()
    {
        listData.Clear();//清空数据

        listUIElement = TfContentParent.GetComponentsInChildren<TUIElement>(true).ToList();
        for (int i = 0; i != listUIElement.Count; i++)
        {
            TUIElement uIElement = listUIElement[i];
            InitData(uIElement, uIElement.data, i);//使用UIElement已有的数据及参数进行初始化
            listData.Add(uIElement.data);
        }
    }

    /// <summary>
    /// 使用已有数据，初始化全部UI
    /// </summary>
    public virtual void InitUI()
    {
        for (int i = 0; i != ListData.Count; i++)
        {
            TData data = ListData[i];
            if (data == null)
            {
                Debug.LogError("空引用！");
                continue;
            }

            var uIElement = InitElement(PreUIElement, data, i);
            if (uIElement)
            {
                listUIElement.Add(uIElement);
            }
        }
        onInitAllUIElement.Invoke();
    }
    #endregion

    #region Inner Function

    protected virtual TUIElement InitElement(GameObject goPre, TData data, bool isSetData = true)
    {
        int index = listUIElement.Count;
        return InitElement(goPre, data, index, true);
    }

    /// <summary>
    /// 生成元素并初始化
    /// </summary>
    /// <param name="goPre"></param>
    /// <param name="data"></param>
    /// <param name="index">Index</param>
    /// <param name="isSetData"></param>
    protected virtual TUIElement InitElement(GameObject goPre, TData data, int index, bool isSetData = true)
    {
        TUIElement uIElement = CreateElementFunc(goPre);
        if (uIElement && isSetData)
        {
            InitData(uIElement, data, index);
        }
        onInitUIElement.Invoke();
        return uIElement;
    }

    /// <summary>
    /// 生成UI并存储到listUIElement中
    /// </summary>
    /// <param name="goPre"></param>
    /// 
    /// <returns></returns>
    protected virtual TUIElement CreateElementFunc(GameObject goPre)
    {
        GameObject goInst = usePool ? Pool.Get() : Instantiate(goPre);
        goInst.SetupUIInstantiate(TfContentParent);
        TUIElement uIElement = goInst.GetComponent<TUIElement>();
        return uIElement;
    }
    protected virtual void DestroyElementFunc(GameObject go)
    {
        go.GetComponent<TUIElement>()?.OnBeforeDestroy();//手动调用方法

        if (usePool)
            Pool.Release(go);
        else
        {
            go.transform.SetParent(null);//PS：因为物体在下一帧被销毁，因此需要先移出父物体，避免影响新UI生成
            Destroy(go);
        }
    }


    /// <summary>
    /// 传入数据初始化指定Element
    /// </summary>
    /// <param name="uIElement"></param>
    /// <param name="data"></param>
    protected virtual void InitData(TUIElement uIElement, TData data, int index)
    {
        uIElement.Init(data);
    }

    /// <summary>
    /// 获取指定index的元素
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    protected virtual TUIElement GetUIElementAt(int index)
    {
        TUIElement uiElement = listUIElement[index];
        if (uiElement)
            return uiElement;
        else
        {
            Debug.LogError("没有找到相关元素!");
            return null;
        }
    }

    #endregion

    #region Override IShowHideInterface

    public bool IsShowing { get { return isShowing; } set { isShowing = value; } }
    public bool isShowing = false;

    [HideInInspector] public BoolEvent onShowHide;
    [HideInInspector] public UnityEvent onShow;
    [HideInInspector] public UnityEvent onHide;

    public void Show()
    {
        Show(true);
    }
    public void Hide()
    {
        Show(false);
    }
    public void ToggleShow()
    {
        Show(!IsShowing);
    }
    public void Show(bool isShow)
    {
        IsShowing = isShow;

        if (isShow)
            onShow.Invoke();
        else
            onHide.Invoke();
        onShowHide.Invoke(isShow);

        ShowFunc(isShow);
    }
    protected virtual void ShowFunc(bool isShow)
    {
        gameObject.SetActive(isShow);
    }


    #endregion

    #region Obsolete
    [System.Obsolete("Use InitUI instead!", true)]//淘汰原因：取名错误
    public virtual void InitAllUI()
    {
        InitUI();
    }
    #endregion

    #region Editor Method
#if UNITY_EDITOR

    public override void SetInspectorGUIUnityEventProperty(GUIPropertyGroup group)
    {
        base.SetInspectorGUIUnityEventProperty(group);
        group.listProperty.Add(new GUIProperty(nameof(onInitAllUIElement)));
        group.listProperty.Add(new GUIProperty(nameof(onInitUIElement)));
        group.listProperty.Add(new GUIProperty(nameof(onShowHide)));
        group.listProperty.Add(new GUIProperty(nameof(onShow)));
        group.listProperty.Add(new GUIProperty(nameof(onHide)));
    }

#endif
    #endregion
}

/// <summary>
/// 针对ContentElementBase,增加序号以及单例
/// </summary>
/// <typeparam name="TUIManager"></typeparam>
/// <typeparam name="TUIElement"></typeparam>
/// <typeparam name="TData"></typeparam>
public abstract class UIContentPanelBase<TUIManager, TUIElement, TData> : UIContentPanelBase<TUIElement, TData>
    where TUIManager : UIContentPanelBase<TUIElement, TData>
    where TUIElement : ContentElementBase<TUIManager, TUIElement, TData>
    where TData : class
{
    protected override void InitData(TUIElement uIElement, TData data, int index)
    {
        //设置相关引用
        uIElement.Index = index;
        uIElement.UIManager = this as TUIManager;

        base.InitData(uIElement, data, index);
    }
}