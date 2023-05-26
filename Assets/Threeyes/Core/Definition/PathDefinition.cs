/// <summary>
/// 存放自定义常用目录
/// 
/// </summary>
public class PathDefinition
{

    /// <summary>
    /// 存放外部资源的文件  (Assets同级目录/Data/)
    /// </summary>
    public static string dataFolderPath
    {
        get
        {
            return PathTool.ProjectDirPath + "/" + dataFolderName;
        }
    }

    /// 目录结构
    /// Data
    /// ——DataBase
    /// ——Custom
    /// ——Icon
    public static readonly string dataFolderName = "Data";
    public static readonly string dataBaseFolderName = "DataBase";

    //ToUpdate：删掉后面的/，统一用Path.Combine合并（主要是便于通过DirectoryInfo等进行初始化时不需要裁剪）
    public static string relateCustomFolderPath { get { return dataFolderName + "/Custom/"; } }//Data/Custom （自定义文件）
    public static string relateIconFolderPath { get { return relateCustomFolderPath + "Icon/"; } }//Data/Custom/Icon

    public static readonly string iconPicName = "Icon.png";
    public static readonly string iniConfigName = "Config.Ini";//项目配置文件
}