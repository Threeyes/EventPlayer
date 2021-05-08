using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

/// <summary>
/// Ref: Cinemachine
/// 
/// An ad-hoc collection of helpers for reflection, used by Cinemachine or its editor tools in various places
/// </summary>
public static class ReflectionHelper
{
    /// <summary>Copy the fields from one object to another</summary>
    /// <param name="src">The source object to copy from</param>
    /// <param name="dst">The destination object to copy to</param>
    /// <param name="bindingAttr">The mask to filter the attributes.
    /// Only those fields that get caught in the filter will be copied</param>
    public static void CopyFields(
        System.Object src, System.Object dst,
        System.Reflection.BindingFlags bindingAttr
            = System.Reflection.BindingFlags.Public
            | System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Instance)
    {
        if (src != null && dst != null)
        {
            Type type = src.GetType();
            FieldInfo[] fields = type.GetFields(bindingAttr);
            for (int i = 0; i < fields.Length; ++i)
                if (!fields[i].IsStatic)
                    fields[i].SetValue(dst, fields[i].GetValue(src));
        }
    }

    /// <summary>Search the assembly for all types that match a predicate</summary>
    /// <param name="assembly">The assembly to search</param>
    /// <param name="predicate">The type to look for</param>
    /// <returns>A list of types found in the assembly that inherit from the predicate</returns>
    public static IEnumerable<Type> GetTypesInAssembly(
        Assembly assembly, Predicate<Type> predicate)
    {
        if (assembly == null)
            return null;

        Type[] types = new Type[0];
        try
        {
            types = assembly.GetTypes();
        }
        catch (Exception)
        {
            // Can't load the types in this assembly
        }
        types = (from t in types
                 where t != null && predicate(t)
                 select t).ToArray();
        return types;
    }

    /// <summary>Get a type from a name</summary>
    /// <param name="typeName">The name of the type to search for</param>
    /// <returns>The type matching the name, or null if not found</returns>
    public static Type GetTypeInAllLoadedAssemblies(string typeName)
    {
        foreach (Type type in GetTypesInAllLoadedAssemblies(t => t.Name == typeName))
            return type;
        return null;
    }

    /// <summary>Search all assemblies for all types that match a predicate</summary>
    /// <param name="predicate">The type to look for</param>
    /// <returns>A list of types found in the assembly that inherit from the predicate</returns>
    public static IEnumerable<Type> GetTypesInAllLoadedAssemblies(Predicate<Type> predicate)
    {
        Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        List<Type> foundTypes = new List<Type>(100);
        foreach (Assembly assembly in assemblies)
        {
            foreach (Type foundType in GetTypesInAssembly(assembly, predicate))
                foundTypes.Add(foundType);
        }
        return foundTypes;
    }

    /// <summary>call GetTypesInAssembly() for all assemblies that match a predicate</summary>
    /// <param name="assemblyPredicate">Which assemblies to search</param>
    /// <param name="predicate">What type to look for</param>
    public static IEnumerable<Type> GetTypesInLoadedAssemblies(
        Predicate<Assembly> assemblyPredicate, Predicate<Type> predicate)
    {
        Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        assemblies = assemblies.Where((Assembly assembly)
                =>
        { return assemblyPredicate(assembly); }).OrderBy((Assembly ass)
     =>
        { return ass.FullName; }).ToArray();

        List<Type> foundTypes = new List<Type>(100);
        foreach (Assembly assembly in assemblies)
        {
            foreach (Type foundType in GetTypesInAssembly(assembly, predicate))
                foundTypes.Add(foundType);
        }

        return foundTypes;
    }

    /// <summary>Is a type defined and visible</summary>
    /// <param name="fullname">Fullly-qualified type name</param>
    /// <returns>true if the type exists</returns>
    public static bool TypeIsDefined(string fullname)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            try
            {
                foreach (var type in assembly.GetTypes())
                    if (type.FullName == fullname)
                        return true;
            }
            catch (System.Exception) { } // Just skip uncooperative assemblies
        }
        return false;
    }

    /// <summary>Cheater extension to access internal field of an object</summary>
    /// <param name="type">The type of the field</param>
    /// <param name="obj">The object to access</param>
    /// <param name="memberName">The string name of the field to access</param>
    /// <returns>The value of the field in the objects</returns>
    public static T AccessInternalField<T>(this Type type, object obj, string memberName)
    {
        if (string.IsNullOrEmpty(memberName) || (type == null))
            return default(T);

        System.Reflection.BindingFlags bindingFlags = System.Reflection.BindingFlags.NonPublic;
        if (obj != null)
            bindingFlags |= System.Reflection.BindingFlags.Instance;
        else
            bindingFlags |= System.Reflection.BindingFlags.Static;

        FieldInfo field = type.GetField(memberName, bindingFlags);
        if ((field != null) && (field.FieldType == typeof(T)))
            return (T)field.GetValue(obj);
        else
            return default(T);
    }

#if false
        /// <summary>Cheater extension to access internal property of an object</summary>
        /// <param name="type">The type of the field</param>
        /// <param name="obj">The object to access</param>
        /// <param name="memberName">The string name of the field to access</param>
        /// <returns>The value of the field in the objects</returns>
        public static T AccessInternalProperty<T>(this Type type, object obj, string memberName)
        {
            if (string.IsNullOrEmpty(memberName) || (type == null))
                return default(T);

            System.Reflection.BindingFlags bindingFlags = System.Reflection.BindingFlags.NonPublic;
            if (obj != null)
                bindingFlags |= System.Reflection.BindingFlags.Instance;
            else
                bindingFlags |= System.Reflection.BindingFlags.Static;

            PropertyInfo pi = type.GetProperty(memberName, bindingFlags);
            if ((pi != null) && (pi.PropertyType == typeof(T)))
                return (T)pi.GetValue(obj, null);
            else
                return default(T);
        }
#endif

    /// <summary>Get the object owner of a field.  This method processes
    /// the '.' separator to get from the object that owns the compound field
    /// to the object that owns the leaf field</summary>
    /// <param name="path">The name of the field, which may contain '.' separators</param>
    /// <param name="obj">the owner of the compound field</param>
    public static object GetParentObject(string path, object obj)
    {
        var fields = path.Split('.');
        if (fields.Length == 1)
            return obj;

        var info = obj.GetType().GetField(
                fields[0], System.Reflection.BindingFlags.Public
                    | System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Instance);
        obj = info.GetValue(obj);

        return GetParentObject(string.Join(".", fields, 1, fields.Length - 1), obj);
    }

    /// <summary>Returns a string path from an expression - mostly used to retrieve serialized properties
    /// without hardcoding the field path. Safer, and allows for proper refactoring.</summary>
    public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr)
    {
        MemberExpression me;
        switch (expr.Body.NodeType)
        {
            case ExpressionType.MemberAccess:
                me = expr.Body as MemberExpression;
                break;
            default:
                throw new InvalidOperationException();
        }

        var members = new List<string>();
        while (me != null)
        {
            members.Add(me.Member.Name);
            me = me.Expression as MemberExpression;
        }

        var sb = new StringBuilder();
        for (int i = members.Count - 1; i >= 0; i--)
        {
            sb.Append(members[i]);
            if (i > 0) sb.Append('.');
        }
        return sb.ToString();
    }


    //¡ª¡ªDeepCopy¡ª¡ª £¨Ref£ºhttps://stackoverflow.com/questions/39092168/c-sharp-copying-unityevent-information-using-reflection£©

    /// <summary>
    /// Gets all fields from an object and its hierarchy inheritance.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="flags">The flags.</param>
    /// <returns>All fields of the type.</returns>
    public static List<FieldInfo> GetAllFields(this Type type, BindingFlags flags)
    {
        // Early exit if Object type
        if (type == typeof(System.Object))
        {
            return new List<FieldInfo>();
        }

        // Recursive call
        var fields = type.BaseType.GetAllFields(flags);
        fields.AddRange(type.GetFields(flags | BindingFlags.DeclaredOnly));
        return fields;
    }

    /// <summary>
    /// Perform a deep copy of the class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <returns>A deep copy of obj.</returns>
    /// <exception cref="System.ArgumentNullException">Object cannot be null</exception>
    public static T DeepCopy<T>(T obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException("Object cannot be null");
        }
        return (T)DoCopy(obj);
    }


    /// <summary>
    /// Does the copy.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentException">Unknown type</exception>
    public static object DoCopy(object obj)
    {
        if (obj == null)
        {
            return null;
        }

        // Value type
        var type = obj.GetType();
        if (type.IsValueType || type == typeof(string))
        {
            return obj;
        }

        // Array
        else if (type.IsArray)
        {
            Type elementType = type.GetElementType();
            var array = obj as Array;
            Array copied = Array.CreateInstance(elementType, array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                copied.SetValue(DoCopy(array.GetValue(i)), i);
            }
            return Convert.ChangeType(copied, obj.GetType());
        }

        // Unity Object
        else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
        {
            return obj;
        }

        // Class -> Recursion
        else if (type.IsClass)
        {
            var copy = Activator.CreateInstance(obj.GetType());

            var fields = type.GetAllFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                var fieldValue = field.GetValue(obj);
                if (fieldValue != null)
                {
                    field.SetValue(copy, DoCopy(fieldValue));
                }
            }

            return copy;
        }

        // Fallback
        else
        {
            throw new ArgumentException("Unknown type");
        }
    }


}
