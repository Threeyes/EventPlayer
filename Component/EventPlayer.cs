using UnityEngine;
using UnityEngine.Events;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Threeyes.Core;
using Threeyes.Core.Editor;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// Base Component that support common unityevent
    /// 
    /// Naming specification：
    /// XX_ID
    /// XX_Target ID
    /// Todo:隐藏该BoolEvent
    /// 
    /// Todo：多ID可以参考Enum，用| &等来表示如何匹配传入的ID，其中,与|功能相同
    /// </summary>
    [AddComponentMenu(EditorDefinition_EventPlayer.AssetMenuPrefix_EventPlayer + "EventPlayer", -100)]
    public class EventPlayer : MonoBehaviour, IHierarchyViewInfo
    {
        #region Networking

        public bool IsCommandMode { get { return isCommandMode; } set { isCommandMode = value; } }//Set to true to invoke Action instead
        private bool isCommandMode = false;
        public UnityAction<bool> actionCommandPlay;
        public UnityAction<bool> actionCommandSetIsActive;

        public UnityAction<bool> actionRealPlay;
        public UnityAction<bool> actionRealSetIsActive;

        #endregion

        #region Property & Field

        /// <summary>
        /// Global Play/Stop callback by targetID [ID, targetEventPlayer]
        /// </summary>
        protected static UnityAction<string, UnityAction<EventPlayer>> actionByID;

        //——UnityEvent——
        /// <summary>
        /// Emitted when Play/Stop Method is Invoked
        /// </summary>
        public BoolEvent onPlayStop;

        /// <summary>
        /// Emitted when Play Method is Invoked
        /// </summary>
        public UnityEvent onPlay;
        /// <summary>
        /// Emitted when Stop Method is Invoked
        /// </summary>
        public UnityEvent onStop;

        public BoolEvent onActiveDeactive;

        //——Basic Setting——
        public virtual bool IsPlayed { get { return isPlayed; } /*protected*/ set { isPlayed = value; RepaintHierarchyWindow(); } }//取消protected set以便反序列化时还原状态
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (IsCommandMode)
                    actionCommandSetIsActive.Execute(value);
                else
                {
                    actionRealSetIsActive.Execute(value);

                    //Warning:Don't Sent event to relate EP Child
                    onActiveDeactive.Invoke(value);
                    isActive = value;
                    NotifyListener<EventPlayerListener>((epl) => epl.OnActiveDeactive(value));
                    RepaintHierarchyWindow();
                }
            }
        }
        public bool CanPlay { get { return canPlay; } set { canPlay = value; RepaintHierarchyWindow(); } }
        public bool CanStop { get { return canStop; } set { canStop = value; RepaintHierarchyWindow(); } }
        public bool IsPlayOnAwake { get { return isPlayOnAwake; } set { isPlayOnAwake = value; RepaintHierarchyWindow(); } }
        public bool IsPlayOnce { get { return isPlayOnce; } set { isPlayOnce = value; RepaintHierarchyWindow(); } }
        public bool IsReverse { get { return isReverse; } set { isReverse = value; RepaintHierarchyWindow(); } }
        public List<EventPlayerListener> ListListener { get { return listListener; } set { listListener = value; } }

        [SerializeField] protected bool isPlayed = false;// Cache the play state
        [SerializeField] protected bool isActive = true;
        [SerializeField] protected bool canPlay = true;
        [SerializeField] protected bool canStop = true;

        [SerializeField] protected bool isPlayOnAwake = false;
        [SerializeField] protected bool isPlayOnce = false;
        [SerializeField] protected bool isReverse = false;

        [SerializeField] private List<EventPlayerListener> listListener = new List<EventPlayerListener>();//For those who don't want to drag Events or inherit from EP, you can use these to receive events

        //——Group Setting——
        /// <summary>
        /// returns the relate EventPlayerGroup's IsActive state
        /// </summary>
        public bool IsGroupActive
        {
            get
            {
                foreach (EventPlayer epgParent in transform.GetComponentsInParent<EventPlayer>())
                    if (epgParent && epgParent.IsGroup)
                    {
                        bool isManager = epgParent.IsRecursive || (!epgParent.IsRecursive && epgParent.transform == transform.parent);//所获得的EPG是否为该EP的管理员
                        if (isManager)
                        {
                            if (epgParent.IsActive)
                            {
                                if (!epgParent.IsIncludeHide && !gameObject.activeInHierarchy)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                return true;
            }
        }
        public bool IsGroup { get { return isGroup; } set { isGroup = value; RepaintHierarchyWindow(); } }
        public bool IsIncludeHide { get { return isIncludeHide; } set { isIncludeHide = value; RepaintHierarchyWindow(); } }
        public bool IsRecursive { get { return isRecursive; } set { isRecursive = value; RepaintHierarchyWindow(); } }

        [SerializeField] protected bool isGroup = false;
        [SerializeField] protected bool isIncludeHide = true;
        [SerializeField] protected bool isRecursive = false;


        //——ID Setting——
        public string ID { get { return id; } set { id = value; RepaintHierarchyWindow(); } }
        public bool IsInvokeByID { get { return isInvokeByID; } set { isInvokeByID = value; RepaintHierarchyWindow(); } }
        public TargetIDLocationType TargetIDLocation { get { return targetIDLocation; } set { targetIDLocation = value; RepaintHierarchyWindow(); } }
        public string TargetID { get { return targetId; } set { targetId = value; RepaintHierarchyWindow(); } }

        //[Header("ID Setting")]
        //Warning&&Bug：多个带UnityEvent的EventPlayer挂在同一个物体上，链接自身的方法时会出现只能连接第一个EventPlayer(https://forum.unity.com/threads/solved-how-to-use-unityevents-for-a-gameobject-that-has-multiple-components-of-the-same-type.368150/)
        //如果：2个EventPlayer挂在同一物体下，都要作为事件中转类，当需要调用自身的PlayByID时，建议调用带参数的方法，而不是无参的方法

        [Tooltip("ID for this Component,，Separate by ','")]
        [SerializeField] protected string id = "";
        [Tooltip("Also Invoke other EventPlayer by TargetID")]
        [SerializeField] protected bool isInvokeByID = false;
        [Tooltip("Desire Location for EventPlayer that has the target ID")]
        [SerializeField] protected TargetIDLocationType targetIDLocation = TargetIDLocationType.Global;
        [Tooltip("ID for target Component")]
        [SerializeField] protected string targetId = "";

        //——Debug Setting——
        public bool IsLogOnPlay { get { return isLogOnPlay; } set { isLogOnPlay = value; RepaintHierarchyWindow(); } }
        public bool IsLogOnStop { get { return isLogOnStop; } set { isLogOnStop = value; RepaintHierarchyWindow(); } }
        public EventPlayer_State State { get { return state; } set { state = value; RepaintHierarchyWindow(); } }

        //[Header("Debug Setting")]
        [SerializeField]
        protected bool isLogOnPlay = false;
        [SerializeField]
        protected bool isLogOnStop = false;
        [SerializeField]
        protected EventPlayer_State state = EventPlayer_State.Idle;


        #endregion

        #region Public Method

        //——Play/Stop ——
        public virtual void Play()
        {
            Play(true);
        }
        public virtual void Stop()
        {
            Play(false);
        }
        public void TogglePlay()
        {
            Play(IsReverse ? isPlayed : !isPlayed);
        }

        public virtual void Play(bool isPlay)
        {
            //Let remove target decide which one to call
            if (IsCommandMode)
            {
                actionCommandPlay.Execute(isPlay);
                return;
            }
            else
            {
                actionRealPlay.Execute(isPlay);
            }

            if (!IsActive)
                return;

            if (isPlay && !IsReverse || !isPlay && IsReverse)//Actual Play
            {
                if (IsPlayOnce && IsPlayed || !CanPlay)
                    return;
                PlayFunc();
            }
            else if (!isPlay && !IsReverse || isPlay && IsReverse)//Actual Stop
            {
                if (!CanStop)
                    return;
                StopFunc();
            }
        }
        //——Play/Stop by TargetID (Mainly called by Event or Method)——

        public void PlayByID()
        {
            PlayByID(TargetID, true);
        }
        public void StopByID()
        {
            PlayByID(TargetID, false);
        }
        public void PlayByID(bool isPlay)
        {
            PlayByID(TargetID, isPlay);
        }
        public void PlayByID(string targetID)
        {
            PlayByID(targetID, true);
        }
        public void StopByID(string targetID)
        {
            PlayByID(targetID, false);
        }

        public void PlayByID(string targetID, bool isPlay)
        {
            //Play(isPlay);//Warning：user may use Play to invoke PlayByID and case dead loop error, so it should not call the Play method
            ForEachID<EventPlayer>(targetID,
            (ep) =>
            {
                if (ep != null)
                {
                    ep.Play(isPlay);
                }
            });
        }

        //——Active ——
        public void ActiveByID()
        {
            ActiveByID(TargetID, true);
        }
        public void DeActiveByID()
        {
            ActiveByID(TargetID, false);
        }
        public void ActiveByID(bool isActive)
        {
            ActiveByID(TargetID, isActive);
        }
        public void ActiveByID(string targetID)
        {
            ActiveByID(targetID, true);
        }
        public void DeActiveByID(string targetID)
        {
            ActiveByID(targetID, false);
        }
        public void ActiveByID(string targetID, bool isActive)
        {
            ForEachID<EventPlayer>(targetID, (ep) => ep.IsActive = isActive);
        }
        public static void ActiveByID(string targetID, bool isActive, EventPlayer eventPlayer)
        {

        }

        //——Reset ——
        [ContextMenu("ResetState")]
        public void ResetState()
        {
            ResetStateFunc();
        }


        #endregion

        #region Inner Method

        protected virtual void PlayFunc()
        {
            UnityAction<EventPlayer> actionRelate =
            (ep) =>
            {
                if (ep != null)
                    ep.Play(true);
            };

            InvokeFunc(
                () =>
                {
                    //ToAdd:SafeMode,在SOInstance中增加该选项，然后全部通过try来调用Event
                    onPlay.Invoke(); onPlayStop.Invoke(true);
                    SetStateFunc(EventPlayer_State.Played);
                },
                actionRelate,
                actionRelate,
                () => { if (IsLogOnPlay) Debug.Log(name + " Play!"); });

            NotifyListener<EventPlayerListener>((epl) => { epl.OnPlay(); epl.OnPlayStop(true); });
        }

        protected virtual void StopFunc()
        {
            UnityAction<EventPlayer> actionRelate =
                (ep) =>
               {
                   if (ep != null)
                       ep.Play(false);
               };

            InvokeFunc(
                () =>
                {
                    onStop.Invoke(); onPlayStop.Invoke(false);
                    SetStateFunc(EventPlayer_State.Stoped);
                },
                actionRelate,
                actionRelate,
                () => { if (IsLogOnStop) Debug.Log(name + " Stop!"); });

            NotifyListener<EventPlayerListener>((epl) => { epl.OnStop(); epl.OnPlayStop(false); });
        }

        /// <summary>
        /// Invoke Custom
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actionSelf">Invoke Event, Set Data, Update State etc</param>
        /// <param name="actionRelateEPByGroup"></param>
        /// <param name="actionRelateEPByID"></param>
        /// <param name="actionEditor">Log Info or update Editor Info</param>
        protected virtual void InvokeFunc<T>(UnityAction actionSelf, UnityAction<T> actionRelateEPByGroup, UnityAction<T> actionRelateEPByID, UnityAction actionEditor = null) where T : EventPlayer
        {
            actionSelf.Execute();

            if (IsGroup)
                InvokeRelateEPByGroupFunc(actionRelateEPByGroup);
            if (IsInvokeByID)
                InvokeRelateEPByIDFunc(TargetID, actionRelateEPByID);

#if UNITY_EDITOR
            actionEditor.Execute();
            RepaintHierarchyWindow();
#endif
        }


        //——Generic——

        protected virtual void InvokeRelateEPByGroupFunc<T>(UnityAction<T> action) where T : EventPlayer
        {
            ForEachChildComponent<T>(action);
        }


        protected virtual void InvokeRelateEPByIDFunc<T>(string id, UnityAction<T> action) where T : EventPlayer
        {
            ForEachID(id, action);
        }


        protected virtual void NotifyListener<T>(UnityAction<T> action) where T : EventPlayerListener
        {
            if (ListListener.Count == 0)
                return;
            ListListener.ForEach((epl) => { if (epl != null) action.Execute(epl as T); });
        }

        protected virtual void ForEachID<T>(string id, UnityAction<T> action) where T : EventPlayer
        {
            //Warning:Editor will not work because OnIDNotify only receive Action at runtime.
            if (id.IsNullOrEmpty())
            {
                Debug.LogError("TargetId is null!");
                return;
            }

            //Find Target EP
            switch (TargetIDLocation)
            {
                case TargetIDLocationType.Global:
                    if (actionByID != null)
                        actionByID.Invoke(id, action as UnityAction<EventPlayer>);//临时转为EventPlayer，在OnIDNotify中转回原类型.Todo:改为其他实现
                    break;
                case TargetIDLocationType.Sibling:
                    if (transform)
                        transform.ForEachSiblingComponent<T>(ep => ep.OnIDNotify(id, action));
                    break;
                case TargetIDLocationType.RecursiveUp:
                    if (transform)
                        transform.ForEachParentComponent<T>(ep => ep.OnIDNotify(id, action));
                    break;
                case TargetIDLocationType.RecursiveDown:
                    if (transform)
                        transform.ForEachChildComponent<T>(ep => ep.OnIDNotify(id, action));
                    break;
            }
        }
        protected void OnIDNotify<T>(string strID, UnityAction<T> action) where T : EventPlayer
        {
            if (ID.Split(',').Contains(strID))
            {
                try
                {
                    if (this != null)
                    {
                        action.Execute(this as T);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Callback Error！\r\n" + e);
                }
            }
        }

        /// <summary>
        /// Update State
        /// </summary>
        /// <param name="isPlay"></param>
        protected virtual void SetStateFunc(EventPlayer_State state)
        {
            IsPlayed = (state == EventPlayer_State.Played || state == EventPlayer_State.PlayedwithParam) ? true : false;//PS：因为Hierarchy会频繁调用，所以不要改为property
            State = state;
        }

        protected virtual void ResetStateFunc()
        {
            SetStateFunc(EventPlayer_State.Idle);

            if (IsGroup)
                ForEachChildComponent<EventPlayer>((ep) =>
                {
                    ep.ResetState();
                });
        }

        protected virtual void ForEachChildComponent<T>(UnityAction<T> func) where T : Component
        {

            if (transform)//Check null in case the GameObject has been destroy
                transform.ForEachChildComponent(func, IsIncludeHide, false, IsRecursive); //PS: don't include self, or will get infinity loop
        }

        #endregion

        #region Unity Method

        protected virtual void Awake()
        {
            if (IsPlayOnAwake)
            {
                Play();
            }
        }

        bool hasInit = false;
        public void Init()
        {
            if (!hasInit)
            {
                if (ID.NotNullOrEmpty())//避免无效监听
                {
                    actionByID += OnIDNotify;
                    hasInit = true;
                }
            }
        }

        void DeInit()
        {
            if (hasInit)
            {
                actionByID -= OnIDNotify;
            }
        }
        private void OnEnable()
        {
            Init();
        }

        private void OnDestroy()
        {
            DeInit();
        }

        #endregion

        #region Obsolete
        [System.Obsolete("Use PlayByID Instead", true)]
        public void PlaybyID()
        {
            PlayByID(TargetID, true);
        }
        [System.Obsolete("Use StopByID Instead", true)]
        public void StopbyID()
        {
            PlayByID(TargetID, false);
        }
        [System.Obsolete("Use PlayByID Instead", true)]
        public void PlaybyID(bool isPlay)
        {
            PlayByID(TargetID, isPlay);
        }
        [System.Obsolete("Use PlayByID Instead", true)]
        public void PlaybyID(string targetID)
        {
            PlayByID(targetID, true);
        }
        [System.Obsolete("Use StopByID Instead", true)]
        public void StopbyID(string targetID)
        {
            PlayByID(targetID, false);
        }
        #endregion

        #region Editor Method

        /// <summary>
        /// RefreshEditorGUI (Warning: Should be call at runtime)
        ///  Use case: 
        ///     1.Continuously refresh Hierarchy display content: eg: progress
        ///     2.Update Hierarchy display content when component's property is changed by Code/UI
        /// </summary>
        protected static void RepaintHierarchyWindow()
        {
#if UNITY_EDITOR
            EditorApplication.RepaintHierarchyWindow();
#endif
        }

#if UNITY_EDITOR

        //——MenuItem——
        public const string strMenuItem_Root = "GameObject/EventPlayer/";
        public const string strMenuItem_Root_Collection = strMenuItem_Root + "Collection/";
        protected const string strMenuItem_RootCoroutine = strMenuItem_Root + "Coroutine/";
        protected const string strMenuItem_Root_Param = strMenuItem_Root + "Param/";
        public const string strMenuItem_Root_Extend = strMenuItem_Root + "Extend/";

        public const int intCollectionMenuOrder = 100;
        protected const int intCoroutineMenuOrder = 200;
        protected const int intParamMenuOrder = 300;
        protected const int intExtendMenuOrder = 400;


        const string instName = "EP ";
        [MenuItem(strMenuItem_Root + "EventPlayer  %#e", false, 0)]
        public static EventPlayer CreateEventPlayer()
        {
            return CreateEventPlayer(true);
        }
        public static EventPlayer CreateEventPlayer(bool isSelect)
        {
            return EditorTool.CreateGameObject<EventPlayer>(instName, isSelect: isSelect);
        }
        [MenuItem(strMenuItem_Root + "EventPlayer Child  &#e", false, 1)]
        public static EventPlayer CreateEventPlayerChild()
        {
            return EditorTool.CreateGameObjectAsChild<EventPlayer>(instName);
        }

        static string instGroupName = "EPG ";
        [MenuItem(strMenuItem_Root_Collection + "EventPlayerGroup %#g", false, intCollectionMenuOrder + 0)]
        public static void CreateEventPlayerGroup()
        {
            EventPlayer eventPlayer = EditorTool.CreateGameObject<EventPlayer>(instGroupName);
            eventPlayer.IsGroup = true;
        }
        [MenuItem(strMenuItem_Root_Collection + "EventPlayerGroup Child &#g", false, intCollectionMenuOrder + 1)]
        public static void CreateEventPlayerGroupChild()
        {
            EventPlayer eventPlayer = EditorTool.CreateGameObjectAsChild<EventPlayer>(instGroupName);
            eventPlayer.IsGroup = true;
        }

        //——ContextMenu——
        static Dictionary<System.Reflection.FieldInfo, object> dirCacheEvent = new Dictionary<System.Reflection.FieldInfo, object>();
        [ContextMenu("CopyEvent")]
        public virtual void CopyEvent()
        {
            dirCacheEvent.Clear();

            //ToUpdate:根据类型自动查找并缓存
            //ToDelete
            foreach (System.Reflection.FieldInfo fieldInfo in this.GetType().GetFields())
            {
                if (!fieldInfo.FieldType.IsClass)
                    continue;

                System.Type typeRoot = fieldInfo.FieldType;
                try
                {
                    //溯源检查是否继承UnityEventBase,如果是就拷贝
                    if (typeRoot == null)//List等泛型会返回空
                        continue;

                    bool isDesire = false;
                    while (true)
                    {
                        typeRoot = typeRoot.BaseType;
                        if (typeRoot == null)
                            break;
                        else if (typeRoot == typeof(object))
                        {
                            break;
                        }
                        if (typeRoot == typeof(UnityEventBase))
                        {
                            isDesire = true;
                            break;
                        }
                    }
                    if (isDesire)
                    {
                        object obj = ReflectionTool.DoCopy(fieldInfo.GetValue(this));//克隆对象(https://stackoverflow.com/questions/39092168/c-sharp-copying-unityevent-information-using-reflection)
                        dirCacheEvent.Add(fieldInfo, obj);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError(typeRoot + "\r\n" + e);
                }
            }
        }

        [ContextMenu("ParseEvent")]
        public virtual void ParseEvent()
        {
            if (dirCacheEvent.Count == 0)
            {
                Debug.LogError("You should Copy Event First!");
                return;
            }

            Undo.RegisterCompleteObjectUndo(this, "ParseEvent");
            foreach (System.Reflection.FieldInfo fieldInfo in this.GetType().GetFields())
            {
                if (fieldInfo.FieldType.IsSubclassOf(typeof(UnityEventBase)))
                {
                    foreach (var element in dirCacheEvent)
                    {
                        if (element.Key.Name == fieldInfo.Name)
                        {
                            fieldInfo.SetValue(this, element.Value);
                        }
                    }
                }
            }
            //dirCacheEvent.Clear();
            EditorUtility.SetDirty(this);// mark as dirty, so the change will be save into scene file
        }

        //——ModifyEvent——
        /// <summary>
        /// 增加一个占位的空事件，便于在编辑器中设置其引用
        /// </summary>
        public void AddNullOnPlayEvent()
        {
            UnityEventTools.AddVoidPersistentListener(onPlay, NullEvent);
        }
        void NullEvent()
        {
        }

        /// <summary>
        /// 注册到指定的事件
        /// </summary>
        /// <param name="unityEvent">注册方</param>
        /// <param name="eventType">本EP需要注册的事件（仅支持无参数的事件）</param>
        public void RegisterPersistentListenerOnce(UnityEventBase unityEvent, EventPlayer_EventType eventType)
        {
            string functionName = eventType == EventPlayer_EventType.Play ? "Play" : "Stop";//Play或Stop无参数方法
            unityEvent.AddVoidPersistentListenerOnce(this, functionName);
            EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// 注册到指定的事件
        /// </summary>
        /// <param name="unityEvent">本EP需要注册的事件（仅支持带bool参数的事件）</param>
        public void RegisterPersistentListenerOnce(UnityEvent<bool> unityEvent)
        {
            string functionName = "Play";//Play(bool)
            unityEvent.AddPersistentListenerOnce(this, functionName);
        }

        //——Hierarchy GUI——
        protected static StringBuilder sbCache = new StringBuilder();
        public virtual string ShortTypeName { get { return ""; } }

        /// <summary>
        /// Display Property of this EventPlayer
        /// </summary>
        /// <param name="texProperty"></param>
        /// <param name="texEPType"></param>
        public virtual void SetHierarchyGUIProperty(StringBuilder sB)
        {
            //#Basic Setting
            if (IsPlayOnAwake)
                sB.Append("A ");
            if (IsPlayOnce)
                sB.Append("① ");
            if (IsReverse)
                sB.Append("↓");

            //#Group Setting
            if (IsGroup)
            {
                sbCache.Length = 0;
                sbCache.Append("G");
                if (IsIncludeHide)
                    sbCache.Append("H");
                if (IsRecursive)
                    sbCache.Append("R");
                AddSplit(sB, sbCache);
            }

            //#ID Setting
            {
                sbCache.Length = 0;
                if (ID != "")
                    sbCache.Append("\"").Append(ID).Append("\"");//Self ID
                if (IsInvokeByID && TargetID != "")
                    sbCache.Append("->").Append("\"").Append(TargetID).Append("\"");//Target ID
                AddSplit(sB, sbCache);
            }

            //#SEP Setting
            {
                ISequence_EventPlayer eventPlayerSequence = transform.GetComponentInParent<ISequence_EventPlayer>();
                if (eventPlayerSequence != null)
                {
                    sbCache.Length = 0;
                    int index = eventPlayerSequence.FindIndexForDataEditor(this);
                    if (index != -1)
                    {
                        sbCache.Append("<").Append(index).Append(">");
                        AddSplit(sB, sbCache);
                    }
                }
            }
        }

        //——Inspector GUI——
        public virtual bool IsCustomInspector { get { return true; } }//Custom Inspector display (Set to false if you has ton's of property and prefer default Inspector view)

        //Specify the draw order (PS: 0 means don't draw)
        public virtual int InspectorUnityEventContentOrder { get { return 1; } }
        public virtual int InspectorBasicContentOrder { get { return 2; } }
        public virtual int InspectorGroupContentOrder { get { return 3; } }
        public virtual int InspectorIDContentOrder { get { return 4; } }
        public virtual int InspectorDebugContentOrder { get { return 5; } }
        public virtual int InspectorSubContentOrder { get { return 6; } }


        public virtual void SetInspectorGUIUnityEventProperty(GUIPropertyGroup group)
        {
            group.title = "Unity Event";
            group.listProperty.Add(new GUIProperty(nameof(onPlayStop)));
            group.listProperty.Add(new GUIProperty(nameof(onPlay)));
            group.listProperty.Add(new GUIProperty(nameof(onStop)));
            group.listProperty.Add(new GUIProperty(nameof(onActiveDeactive)));
        }

        /// <summary>
        /// Draw Property for sub class
        /// </summary>
        /// <param name="group"></param>
        public virtual void SetInspectorGUISubProperty(GUIPropertyGroup group)
        {

        }

        /// <summary>
        /// Inspector Common Tips
        /// </summary>
        /// <param name="sB"></param>
        public virtual void SetInspectorGUICommonTextArea(StringBuilder sB)
        {
            if (ID != "" && ID == TargetID)
            {
                sB.AppendWarningRichText("ID can't be the same as TargetID!");
                sB.Append("\r\n");
            }
        }

        #region Editor Utility

        /// <summary>
        /// Split Each Content
        /// </summary>
        /// <param name="sB"></param>
        /// <param name="sbOther"></param>
        protected static void AddSplit(StringBuilder sB, StringBuilder sbOther)
        {
            if (sB.Length > 0 && sbOther.Length > 0)
            {
                sB.Append("|");
            }
            sB.Append(sbOther);
        }

        #endregion

#endif
        #endregion
    }

    #region Definition

    /// <summary>
    /// 事件类型
    /// </summary>
    public enum EventPlayer_EventType
    {
        Null,
        Play,
        Stop,
        PlayStop
    }

    public enum EventPlayer_State
    {
        Idle = -1,
        Stoped = 0,
        Played = 1 << 0,
        PlayedwithParam = 1 << 1,
        StopedwithParam = 1 << 2,
    }

    /// <summary>
    /// 通过ID调用的查找方式
    /// </summary>
    [System.Serializable]
    public enum TargetIDLocationType
    {
        None = 0,
        Global = 1,//Global
        Sibling = 2,//Same level
        RecursiveUp = 4,//all the way up
        RecursiveDown = 8,//all the way down
    }

    #endregion
}
