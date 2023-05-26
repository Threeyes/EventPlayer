/// <summary>
/// 外部读取的文件存放目录
/// </summary>
public enum ExternalFileLocation
{
    CustomPath = -1,//Custom dir
    StreamingAsset = 0,//Application.streamingAssetsPath 
    PersistentDataPath = 1,//Application.persistentDataPath
    AndroidRoot = 2,//Android Root dir

    CustomData = 101,//PathDefinition.dataFolderPath
}


/// <summary>
/// Which update method to call
/// </summary>
public enum UpdateMethodType
{
    Default = 1,
    Late = 2,
    Fixed = 3,
    Manual = 4
}