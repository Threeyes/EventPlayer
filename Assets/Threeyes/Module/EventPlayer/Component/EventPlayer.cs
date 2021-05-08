using UnityEngine;
using UnityEngine.Events;
using System.Text;
using System.Collections.Generic;
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
    /// </summary>
    public class EventPlayer : MonoBehaviour, IHierarchyInfo
    {
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
        public bool IsActive { get { return isActive; } set { onActiveDeactive.Invoke(value); isActive = value; } }
        public bool CanPlay { get { return canPlay; } set { canPlay = value; } }
        public bool CanStop { get { return canStop; } set { canStop = value; } }
        public bool IsPlayOnAwake { get { return isPlayOnAwake; } set { isPlayOnAwake = value; } }
        public bool IsPlayOnce { get { return isPlayOnce; } set { isPlayOnce = value; } }
        public bool IsReverse { get { return isReverse; } set { isReverse = value; } }

        //[Header("Basic Setting")]
        [SerializeField]
        [Tooltip("Is this EP avaliable")]
        protected bool isActive = true;
        [SerializeField]
        protected bool canPlay = true;
        [SerializeField]
        protected bool canStop = true;

        [SerializeField]
        [Tooltip("Invoke Play Method on Awake")]
        protected bool isPlayOnAwake = false;
        [SerializeField]
        [Tooltip("The Play Method can only be Invoked once")]
        protected bool isPlayOnce = false;
        [SerializeField]
        [Tooltip("Reverse the Play/Stop behaviour")]
        protected bool isReverse = false;


        //——Group Setting——
        /// <summary>
        /// returns the relate EventPlayerGroup's IsActive state
        /// </summary>
        public bool IsGroupActive
        {
            get
            {
                bool isGroupActive = true;
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
                                    isGroupActive = false;
                                }
                            }
                            else
                            {
                                isGroupActive = false;
                            }
                        }
                    }
                return isGroupActive;
            }
        }
        public bool IsGroup { get { return isGroup; } set { isGroup = value; } }
        public bool IsIncludeHide { get { return isIncludeHide; } set { isIncludeHide = value; } }
        public bool IsRecursive { get { return isRecursive; } set { isRecursive = value; } }

        //[Header("Group Setting")]
        [SerializeField]
        protected bool isGroup = false;
        [SerializeField]
        [Tooltip("Is Invoke the child component evenif the GameObject is deActive in Hierarchy")]
        protected bool isIncludeHide = true;
        [SerializeField]
        [Tooltip("Is recursive find the child component")]
        protected bool isRecursive = false;


        //——ID Setting——
        public string ID { get { return id; } set { id = value; } }
        public bool IsInvokeByID { get { return isInvokeByID; } set { isInvokeByID = value; } }
        public TargetIDLocationType TargetIDLocation { get { return targetIDLocation; } set { targetIDLocation = value; } }
        public string TargetID { get { return targetId; } set { targetId = value; } }

        //[Header("ID Setting")]
        //Warning&&Bug：多个带UnityEvent的EventPlayer挂在同一个物体上，链接自身的方法时会出现只能连接第一个EventPlayer(https://forum.unity.com/threads/solved-how-to-use-unityevents-for-a-gameobject-that-has-multiple-components-of-the-same-type.368150/)
        //如果：2个EventPlayer挂在同一物体下，都要作为事件中转类，当需要调用自身的PlayByID时，建议调用带参数的方法，而不是无参的方法
        [SerializeField]
        [Tooltip("ID for this Component")]
        protected string id = "";
        [SerializeField]
        [Tooltip("Invoke other EventPlayer by TargetID")]
        protected bool isInvokeByID = false;
        [SerializeField]
        [Tooltip("Desire Location for EventPlayer that has the target ID")]
        protected TargetIDLocationType targetIDLocation = TargetIDLocationType.Global;
        [SerializeField]
        [Tooltip("ID for target Component")]
        protected string targetId = "";


        //——Debug Setting——
        public virtual bool IsPlayed { get { return isPlayed; } }
        public bool IsLogOnPlay { get { return isLogOnPlay; } set { isLogOnPlay = value; } }
        public bool IsLogOnStop { get { return isLogOnStop; } set { isLogOnStop = value; } }
        public EventPlayer_State State { get { return state; } set { state = value; } }

        //[Header("Debug Setting")]
        [SerializeField]
        protected bool isLogOnPlay = false;
        [SerializeField]
        protected bool isLogOnStop = false;
        [SerializeField]
        protected bool isPlayed = false;// Cache the play state
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
            Play(!isPlayed);
        }
        public virtual void Play(bool isPlay)
        {
            if (!IsActive)
                return;

            if (isPlay && !IsReverse || !isPlay && IsReverse)//Actual Play
            {
                if (IsPlayOnce && isPlayed || !CanPlay)
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
        //——Play/Stop by ID——
        public void PlaybyID()
        {
            PlayByID(TargetID, true);
        }
        public void StopbyID()
        {
            PlayByID(TargetID, false);
        }
        public void PlaybyID(bool isPlay)
        {
            PlayByID(TargetID, isPlay);
        }
        public void PlaybyID(string targetID)
        {
            PlayByID(targetID, true);
        }
        public void StopbyID(string targetID)
        {
            PlayByID(targetID, false);
        }
        public void PlayByID(string targetID, bool isPlay)
        {
            Play(isPlay);
            ForEachID<EventPlayer>(targetID,
            (ep) =>
            {
                if (ep != null)
                    ep.Play();
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
                    onPlay.Invoke(); onPlayStop.Invoke(true);
                    SetStateFunc(true, EventPlayer_State.Played);
                },
                actionRelate,
                actionRelate,
                () => { if (IsLogOnPlay) Debug.Log(name + " Play!"); });
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
                    SetStateFunc(false, EventPlayer_State.Stoped);
                },
                actionRelate,
                actionRelate,
                () => { if (IsLogOnStop) Debug.Log(name + " Stop!"); });
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


        protected virtual void InvokeRelateEPByIDFunc<T>(string ID, UnityAction<T> action) where T : EventPlayer
        {
            ForEachID(ID, action);
        }

        protected virtual void ForEachID<T>(string ID, UnityAction<T> action) where T : EventPlayer
        {
            if (ID.IsNullOrEmpty())
            {
                Debug.LogError("TargetId is null!");
                return;
            }

            //Find Target EP
            switch (TargetIDLocation)
            {
                case TargetIDLocationType.Global:

                    //(Debug) will not work because OnIDNotify only receive Action at runtime.
                    //#if UNITY_EDITOR
                    //                    if (!Application.isPlaying)
                    //                    {
                    //                        transform.ForEachSceneComponent<EventPlayer>(ep => ep.OnIDNotify(ID, action));
                    //                        break;
                    //                    }
                    //#endif

                    if (actionByID != null)
                        actionByID.Invoke(ID, action as UnityAction<EventPlayer>);//临时转为EventPlayer，在OnIDNotify中转回原类型.Todo:改为其他实现
                    break;
                case TargetIDLocationType.Sibling:
                    if (transform)
                        transform.ForEachSiblingComponent<T>(ep => ep.OnIDNotify(ID, action));
                    break;
                case TargetIDLocationType.RecursiveUp:
                    if (transform)
                        transform.ForEachParentComponent<T>(ep => ep.OnIDNotify(ID, action));
                    break;
                case TargetIDLocationType.RecursiveDown:
                    if (transform)
                        transform.ForEachChildComponent<T>(ep => ep.OnIDNotify(ID, action));
                    break;
            }
        }
        protected void OnIDNotify<T>(string strID, UnityAction<T> action) where T : EventPlayer
        {
            if (strID == ID)//Self
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
        protected virtual void SetStateFunc(bool isPlay, EventPlayer_State state)
        {
            this.isPlayed = IsReverse ? !isPlay : isPlay;
            this.State = state;
        }

        protected virtual void ResetStateFunc()
        {
            SetStateFunc(false, EventPlayer_State.Idle);

            if (IsGroup)
                ForEachChildComponent<EventPlayer>((ep) =>
                {
                    ep.ResetState();
                });
        }

        protected virtual void ForEachChildComponent<T>(UnityAction<T> func) where T : Component
        {

            if (transform)//Check null in case the GameObject has been destroy
                transform.ForEachChildComponent(func, IsIncludeHide, IsRecursive, false); //PS: don't include self, or will get infinity loop
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

        private void OnEnable()
        {
            actionByID += OnIDNotify;
        }

        private void OnDestroy()
        {
            actionByID -= OnIDNotify;
        }

        #endregion

        #region Editor Method
#if UNITY_EDITOR

        //——MenuItem——
        //MenuItem's info
        public const string strRootMenuItem = "GameObject/EventPlayers/";

        public const string strSubCollectionMenuItem = strRootMenuItem + "Collection/";
        protected const string strSubCoroutineMenuItem = strRootMenuItem + "Coroutine/";
        protected const string strParamMenuGroup = strRootMenuItem + "Param/";
        protected const string strExtendMenuGroup = strRootMenuItem + "Extend/";

        public const int intCollectionMenuOrder = 100;
        protected const int intCoroutineMenuOrder = 200;
        protected const int intParamMenuOrder = 300;
        protected const int intExtendMenuOrder = 400;


        const string instName = "EP ";
        [MenuItem(strRootMenuItem + "EventPlayer  %#e", false, 0)]
        public static EventPlayer CreateEventPlayer()
        {
            return EditorTool.CreateGameObject<EventPlayer>(instName);
        }
        [MenuItem(strRootMenuItem + "EventPlayer Child  &#e", false, 1)]
        public static EventPlayer CreateEventPlayerChild()
        {
            return EditorTool.CreateGameObjectAsChild<EventPlayer>(instName);
        }

        static string instGroupName = "EPG ";
        [MenuItem(strSubCollectionMenuItem + "EventPlayerGroup %#g", false, intCollectionMenuOrder + 0)]
        public static void CreateEventPlayerGroup()
        {
            EventPlayer eventPlayer = EditorTool.CreateGameObject<EventPlayer>(instGroupName);
            eventPlayer.IsGroup = true;
        }
        [MenuItem(strSubCollectionMenuItem + "EventPlayerGroup Child &#g", false, intCollectionMenuOrder + 1)]
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
            //ToUpdate:根据类型自动查找并缓存
            //ToDelete
            foreach (System.Reflection.FieldInfo fieldInfo in this.GetType().GetFields())
            {
                if (!fieldInfo.FieldType.IsClass)
                    continue;

                //溯源检查是否继承UnityEventBase
                System.Type typeRoot = fieldInfo.FieldType;
                while (true)
                {
                    typeRoot = typeRoot.BaseType;
                    if (typeRoot == typeof(UnityEventBase))
                        break;
                    else if (typeRoot == typeof(object))
                    {
                        continue;
                    }
                }
                object obj = ReflectionHelper.DoCopy(fieldInfo.GetValue(this));//克隆对象
                dirCacheEvent.Add(fieldInfo, obj);
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
            dirCacheEvent.Clear();
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
            unityEvent.AddPersistentListenerOnce(this, functionName);
            EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// 注册到指定的事件
        /// </summary>
        /// <param name="unityEvent">本EP需要注册的事件（仅支持带bool参数的事件）</param>
        public void RegisterPersistentListenerOnce(UnityEvent<bool> unityEvent)
        {
            string functionName = "Play";//Play(bool)
            unityEvent.AddPersistentListenerOnce(this, functionName, new System.Type[1] { typeof(bool) });
        }

        //——Hierarchy GUI——

        //Hierarchy GUI Format： [ Basic Setting | Goup Setting | ID Setting | SubClass Setting ] EventPlayer_Type 
        protected static StringBuilder sbCache = new StringBuilder();

        /// <summary>
        /// Set the Type of this EventPlayer
        /// </summary>
        /// <returns></returns>
        public virtual void SetHierarchyGUIType(StringBuilder sB)
        {

        }

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
        }
        /// <summary>
        /// RefreshEditorGUI
        /// </summary>
        protected static void RepaintHierarchyWindow()
        {
#if UNITY_EDITOR
            EditorApplication.RepaintHierarchyWindow();
#endif
        }

        //——Inspector GUI——
        public virtual bool IsCustomInspector { get { return true; } }//Custom Inspector display (Set to false if you has ton's of property and tired of drawing them)

        public virtual void SetInspectorGUIUnityEventProperty(GUIPropertyGroup group)
        {
            group.title = "Unity Event";
            group.listProperty.Add(new GUIProperty("onPlayStop", "OnPlayStop"));
            group.listProperty.Add(new GUIProperty("onPlay", "OnPlay"));
            group.listProperty.Add(new GUIProperty("onStop", "OnStop"));
            group.listProperty.Add(new GUIProperty("onActiveDeactive", "OnActiveDeactive"));
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
                EditorDrawerTool.AppendWarningText(sB, "ID can't be the same as TargetID!");
                sB.Append("\r\n");
            }
        }

        /// <summary>
        /// Split Each Content
        /// </summary>
        /// <param name="sB"></param>
        /// <param name="sbOther"></param>
        protected static void AddSplit(StringBuilder sB, StringBuilder sbOther)
        {
            EditorDrawerTool.AddSplit(sB, sbOther);
        }

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
