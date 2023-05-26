using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// PS:
/// 1.不能在泛型类中声明，否则会因为不能被初始化从而导致空引用报错
/// 2.只能存放通用的Event，个别需要使用插件库的带参Event不能放在此处，必须放在自己的类中（参考VideoEventPlayer）
/// </summary>

//——System——

[System.Serializable]
public class EnumEvent : UnityEvent<System.Enum> { }

[System.Serializable]
public class StringEvent : UnityEvent<string> { }

[System.Serializable]
public class BoolEvent : UnityEvent<bool> { }

[System.Serializable]
public class BytesEvent : UnityEvent<byte[]> { }

[System.Serializable]
public class IntEvent : UnityEvent<int> { }

[System.Serializable]
public class FloatEvent : UnityEvent<float> { }

[System.Serializable]
public class ObjectEvent : UnityEvent<object> { }

//——Unity——

[System.Serializable]
public class UnityObjectEvent : UnityEvent<UnityEngine.Object> { }

[System.Serializable]
public class ColorEvent : UnityEvent<Color> { }

[System.Serializable]
public class ColliderEvent : UnityEvent<Collider> { }

[System.Serializable]
public class CollisionEvent : UnityEvent<Collision> { }

[System.Serializable]
public class GameObjectEvent : UnityEvent<GameObject> { }

[System.Serializable]
public class Vector2Event : UnityEvent<Vector2> { }

[System.Serializable]
public class Vector3Event : UnityEvent<Vector3> { }

[System.Serializable]
public class GradientEvent : UnityEvent<Gradient> { }


//——Assets——

[System.Serializable]
public class TextAssetEvent : UnityEvent<TextAsset> { }

[System.Serializable]
public class TextureEvent : UnityEvent<Texture> { }
[System.Serializable]
public class Texture2DEvent : UnityEvent<Texture2D> { }

[System.Serializable]
public class ScriptableObjectEvent : UnityEvent<ScriptableObject> { }

[System.Serializable]
public class SOBytesAssetEvent : UnityEvent<SOBytesAsset> { }



public delegate TResult CustomFunc<TResult>();
public delegate TResult CustomFunc<T, TResult>(T arg1);
