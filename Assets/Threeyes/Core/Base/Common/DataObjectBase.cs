namespace Threeyes.Base
{
    /// <summary>
    /// 普通数据类的基类，带有比较、空检测等功能
    /// </summary>
    public class DataObjectBase
    {
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(DataObjectBase x, DataObjectBase y)
        {
            //可避免空引用：  https://stackoverflow.com/questions/4219261/overriding-operator-how-to-compare-to-null
            if (object.ReferenceEquals(x, null))
            {
                return object.ReferenceEquals(y, null);
            }

            return x.Equals(y);
        }

        public static bool operator !=(DataObjectBase x, DataObjectBase y)
        {
            return !x == y;
        }

        public static implicit operator bool(DataObjectBase exists)
        {
            return !(exists == null);
        }
    }
}