using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// PS:
/// 1.不能在泛型类中声明，否则会因为不能被初始化从而导致空引用报错
/// 2.只能存放通用的Event，个别需要使用插件库的带参Event不能放在此处，必须放在自己的类中（参考VideoEventPlayer）
/// </summary>

[System.Serializable]
public class ObjectEvent : UnityEvent<object>
{
}
[System.Serializable]
public class StringEvent : UnityEvent<string>
{
}

[System.Serializable]
public class BoolEvent : UnityEvent<bool>
{
}



[System.Serializable]
public class IntEvent : UnityEvent<int>
{

}

[System.Serializable]
public class FloatEvent : UnityEvent<float>
{
}

[System.Serializable]
public class ColorEvent : UnityEvent<Color>
{

}

[System.Serializable]
public class ColliderEvent : UnityEvent<Collider>
{

}

[System.Serializable]
public class CollisionEvent : UnityEvent<Collision>
{

}

[System.Serializable]
public class GameObjectEvent : UnityEvent<GameObject>
{

}

[System.Serializable]
public class Vector2Event : UnityEvent<Vector2>
{
}

[System.Serializable]
public class Vector3Event : UnityEvent<Vector3>
{
}
