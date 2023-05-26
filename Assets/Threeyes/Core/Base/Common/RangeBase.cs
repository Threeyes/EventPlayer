using UnityEngine;

public abstract class RangeBase<TValue> where TValue : struct
{
    public TValue MinValue { get { return minValue; } set { minValue = value; } }
    public TValue MaxValue { get { return maxValue; } set { maxValue = value; } }
    public abstract TValue RandomValue { get; }

    public abstract TValue Range { get; }

    [SerializeField] protected TValue minValue;
    [SerializeField] protected TValue maxValue;

    public RangeBase(TValue min, TValue max)
    {
        minValue = min;
        maxValue = max;
    }
}

[System.Serializable]
public class Range_Float : RangeBase<float>
{
    public override float RandomValue { get { return Random.Range(MinValue, MaxValue); } }
    public override float Range { get { return MaxValue - MinValue; } }

    public Range_Float(float min, float max) : base(min, max) { }
}

[System.Serializable]
public class Range_Int : RangeBase<int>
{
    public override int RandomValue { get { return Random.Range(MinValue, MaxValue + 1); } }//PS: Random.Range Returns a random integer number between min [inclusive] and max [exclusive]£¬ËùÒÔÒª+1
    public override int Range { get { return MaxValue - MinValue; } }

    public Range_Int(int min, int max) : base(min, max) { }
}
[System.Serializable]
public class Range_Vector3 : RangeBase<Vector3>
{
    public override Vector3 RandomValue { get { return new Vector3(Random.Range(MinValue.x, MaxValue.x), Random.Range(MinValue.y, MaxValue.y), Random.Range(MinValue.z, MaxValue.z)); } }
    public override Vector3 Range { get { return MaxValue - MinValue; } }

    public Range_Vector3(Vector3 min, Vector3 max) : base(min, max) { }
}
