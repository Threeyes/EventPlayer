using System;
using System.Reflection;
//Copy Fields
public interface ICopy
{
    /// <summary>
    /// Copy Fields from other
    /// </summary>
    /// <param name="other"></param>
    void Copy(object source, object target);
}

public interface ICopyFilter
{
    bool ShouldCopy(Type objType, MemberInfo memberInfo);
}
