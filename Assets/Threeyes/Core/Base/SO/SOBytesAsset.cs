using System.Text;
using UnityEngine;
/// <summary>
/// Cache bytes via TextAsset(Editor) or FileBytes(Runtime)
/// 
/// Ref:https://forum.unity.com/threads/how-create-a-textasset-from-external-binary-file.1112608/ #7
/// </summary>
[CreateAssetMenu(menuName = "SO/BytesAsset", fileName = "BytesAsset")]
public class SOBytesAsset : ScriptableObject
{
    public byte[] bytes
    {
        get
        {
            return textAsset ? textAsset.bytes : m_Bytes;
        }
    }
    public TextAsset textAsset;
    [HideInInspector] [SerializeField] byte[] m_Bytes;

    //public static SOBytesInfo CreateFromString(string text)
    //{
    //    var asset = CreateInstance<SOBytesInfo>();
    //    asset.m_Bytes = Encoding.UTF8.GetBytes(text);
    //    return asset;
    //}

    /// Warning：
    /// 1.通过读取文件的string转为 TextAsset 时，如果文件后缀不为.bytes，则其读取后的内容可能会被篡改（https://issuetracker.unity3d.com/issues/textasset-dot-bytes-returns-different-bytes-with-different-file-extensions）
    /// 2.如上所述，TextAsset只能用于引用编辑器内部的文件以便访问其bytes，如果从外部加载资源则直接使用bytes进行初始化
    public static SOBytesAsset CreateFromTextAsset(TextAsset text)
    {
        return CreateFromBytes(text.bytes);
    }
    public static SOBytesAsset CreateFromBytes(byte[] bytes)
    {
        var asset = CreateInstance<SOBytesAsset>();
        asset.m_Bytes = (byte[])bytes.Clone();
        return asset;
    }

    public string text => Encoding.UTF8.GetString(m_Bytes);
    public override string ToString()
    {
        return text;
    }

}
