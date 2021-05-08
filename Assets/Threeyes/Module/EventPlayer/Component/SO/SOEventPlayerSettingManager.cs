#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace Threeyes.EventPlayer
{
    [CreateAssetMenu(menuName = "SO/Manager/EventPlayerSettingManager")]
    public class SOEventPlayerSettingManager : SOInstacneBase<SOEventPlayerSettingManager>
    {
        #region Property & Field

        //——Static——
        public static bool ShowPropertyInHierarchy { get { return Instance ? Instance.showPropertyInHierarchy : true; } set { if (Instance) Instance.showPropertyInHierarchy = value; } }
        public static SOEventPlayerSettingManager Instance { get { return GetOrCreateInstance(ref _instance, defaultName, pathInResources); } }
        private static SOEventPlayerSettingManager _instance;
        static string defaultName = "EventPlayerSetting";
        static string pathInResources = "Threeyes";//该Manager在Resources下的路径，默认是Resources根目录


        public string version = "0"; //Last  cache version

        //Display Setting
        public bool showPropertyInHierarchy = true;//Show info of subclass

        //Other Plugin Support Setting
        //[Header("Other Plugin Support")]
        public bool useTimeline = false;
        public bool useVideoPlayer = false;
        static readonly string epRootDir = "EventPlayer";
        static readonly string extendDir = "Extend/";
        public static List<DefineSymbol> listAvaliableDS = new List<DefineSymbol>()
        {
            new DefineSymbol("Threeyes_Timeline", "TimelineEvent", "支持Timeline事件", extendDir+"Timeline",asmdefName:"Unity.Timeline"),
        };

        public Dictionary<DefineSymbol, bool> dictDS
        {
            get
            {
                return new Dictionary<DefineSymbol, bool>
                {
                    { new DefineSymbol("Threeyes_Timeline", "Timeline Event", "支持Timeline", extendDir+"Timeline") ,useTimeline},
                    {  new DefineSymbol("Threeyes_VideoPlayer", "VideoPlayer Event", "支持VideoPlayer", extendDir+"Video"),useVideoPlayer }
                };
            }
        }

        #endregion

        public void UpdateVersion(string currentVersion)
        {
            if (version != currentVersion)
            {
                version = currentVersion;

                //Update Setting
                RefreshDefine();
                Debug.Log("Update EventPlayerSetting to version: " + currentVersion);
            }
        }

        [ContextMenu("RefreshDefine")]
        public void RefreshDefine()
        {
            foreach (var element in dictDS)
            {
                RefreshRelateFiles(element.Key, element.Value);
            }
        }

        void OnValidate()
        {
#if UNITY_EDITOR
            EditorApplication.RepaintHierarchyWindow();
#endif
        }

        #region Utility


        void RefreshRelateFiles(DefineSymbol defineSymbol, bool enable)
        {
            //Todo:通过路径找到文件夹，然后替换代码头
            string targetDir = "";
            List<DirectoryInfo> listDirectoryInfo = PathTool.GetSubDirectories(new DirectoryInfo(Application.dataPath), -1);
            foreach (DirectoryInfo directoryInfo in listDirectoryInfo)
            {
                if (directoryInfo.Name == epRootDir)
                {
                    string dir = directoryInfo.FullName + "/" + defineSymbol.rootFolderName;
                    if (Directory.Exists(dir))
                    {
                        targetDir = dir;
                        break;
                    }
                }
            }

            if (targetDir != "")
            {
                //Debug.Log("Working on Dir: \r\n" + targetDir);
                DirectoryInfo directoryInfo = new DirectoryInfo(targetDir);
                if (directoryInfo.Exists)
                {
                    List<FileInfo> listFileInfo = PathTool.GetSubFiles(directoryInfo, -1, "*.cs").ToList();

                    foreach (FileInfo fi in listFileInfo)
                    {
                        ModifyCSFile(fi, defineSymbol, enable);
                    }
                    AssetDatabase.Refresh();
                }
            }
            else
            {
                Debug.LogError("Can't find dir:\r\n" + targetDir);
            }
        }

        List<int> _LinesToChange = new List<int>();
        /// <summary>
        /// Learn from DoTween, Replace #if true to #if false
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="defineSymbol"></param>
        /// <param name="enable"></param>
        bool ModifyCSFile(FileInfo fileInfo, DefineSymbol defineSymbol, bool enable)
        {
            bool hasModifiedFiles = false;
            string filePath = fileInfo.FullName;
            string marker = defineSymbol.name;

            _LinesToChange.Clear();
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Length; ++i)
            {
                string s = lines[i];

                //初始化
                if (s.Equals("#if " + marker))
                {
                    lines[i] = "#if true // " + marker;
                    s = lines[i];
                }

                if (s.EndsWith(marker) && s.StartsWith("#if") && (enable && s.Contains("false") || !enable && s.Contains("true")))
                {
                    _LinesToChange.Add(i);
                }
            }
            if (_LinesToChange.Count > 0)
            {
                hasModifiedFiles = true;
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    for (int i = 0; i < lines.Length; ++i)
                    {
                        string s = lines[i];
                        if (_LinesToChange.Contains(i))
                        {
                            s = enable ? s.Replace("false", "true") : s.Replace("true", "false");
                        }
                        sw.WriteLine(s);
                    }
                }

                //string localPath= filePath.funa
                //AssetDatabase.ImportAsset(EditorUtils.FullPathToADBPath(filePath), ImportAssetOptions.Default);
                //Debug.Log("Modify file to " + enable + ":\r\n" + fileInfo.FullName);
            }

            return hasModifiedFiles;
        }

        #endregion
    }
}
#endif