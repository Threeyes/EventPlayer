using System;

/// <summary>
/// 宏定义
/// </summary>
[System.Serializable]
public class DefineSymbol : IEquatable<DefineSymbol>
{
    public string name;//宏定义显示名称
    public string description_en;//英文注释
    public string description_cn;//中文注释
    public string rootFolderName;//根文件夹的名称，通过改名称知道相应插件是否存在
    public string mainFileName;//主要文件的相对路径（可空，主要是避免开发者自己创建的文件夹重名）
    public string asmdefName;//相应的AsmDef文件名



    public bool isUsing = false;//正在使用中
    public DefineSymbol(string name, string description_en, string description_cn, string rootFolderName, string mainFileName = "", string asmdefName = "")
    {
        this.name = name;
        this.description_en = description_en;
        this.description_cn = description_cn;

        this.rootFolderName = rootFolderName;
        this.mainFileName = mainFileName;
        this.asmdefName = asmdefName;
    }

    public bool Equals(DefineSymbol other)
    {
        if (other == null) return false;
        return (this.name.Equals(other.name));
    }
}