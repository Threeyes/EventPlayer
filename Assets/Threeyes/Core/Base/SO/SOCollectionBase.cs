using System.Collections;
using UnityEngine;
/// <summary>
/// 
/// </summary>
public abstract class SOCollectionBase : ScriptableObject
{
    //使SODataGroupBase可以使用foreach（PS：遍历时可以通过指定子元素类型进行转换）（PS：Newtonsoft.Json不支持继承IEnumerable的数据类，否则会报错（https://stackoverflow.com/questions/19080326/deserialize-to-ienumerable-class-using-newtonsoft-json-net））
    public abstract IEnumerator GetEnumerator();
}
