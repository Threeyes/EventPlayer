using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine.Events;

/// <summary>
/// Ref: Cinemachine
/// </summary>
public static class ReflectionTool
{
	public static Type[] GetGenericInterfaceArgumentTypes(Type objType, Type genericInterfaceType)
	{
		Type[] arrArgument = new Type[] { };
		Type interfaceType = GetGenericInterfaceType(objType, genericInterfaceType);
		if (interfaceType != null)
		{
			arrArgument = interfaceType.GetGenericArguments();
		}
		return arrArgument;
	}
	public static Type GetGenericInterfaceType(Type objType, Type genericInterfaceType)
	{
		if (objType == null)
			return null;

		return objType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterfaceType);
	}

	/// <summary>
	/// 获取继承IList（如Array、IList<>）的对应元素类型
	/// </summary>
	/// <param name="type">type that inheric from IList</param>
	/// <returns></returns>
	public static Type GetCollectionElementType(Type type)
	{
		if (type.IsInherit(typeof(IList)))
		{
			if (type.IsArray)
				return type.GetElementType();
			else
			{
				Type genericIListType = ReflectionTool.GetGenericInterfaceType(type, typeof(IList<>));
				if (genericIListType != null)
				{
					return genericIListType.GetGenericArguments().FirstOrDefault();
				}
			}
		}
		return null;
	}

	/// <summary>Copy the fields from one object to another</summary>
	/// <param name="src">The source object to copy from</param>
	/// <param name="dst">The destination object to copy to</param>
	/// <param name="bindingAttr">The mask to filter the attributes.
	/// <paramref name="funcCopyFilter"/>Decide whethre the field should copy<paramref name="funcCopyFilter"/>
	/// Only those fields that get caught in the filter will be copied</param>
	public static void CopyFields(object src, object dst, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Func<Type, MemberInfo, bool> funcCopyFilter = null)
	{
		if (src != null && dst != null)
		{
			Type type = src.GetType();
			FieldInfo[] fields = type.GetFields(bindingAttr);

			Func<Type, MemberInfo, bool> customFilter = null;
			if (dst is ICopyFilter copyFilter)//自定义的筛选器
				customFilter = copyFilter.ShouldCopy;

			for (int i = 0; i < fields.Length; ++i)
				if (!fields[i].IsStatic)
				{
					if (customFilter != null)//自定义的筛选器
						if (!customFilter(type, fields[i]))
							continue;

					if (funcCopyFilter != null && !funcCopyFilter(type, fields[i]))//忽略不能复制的
						continue;

					fields[i].SetValue(dst, fields[i].GetValue(src));
				}
		}
	}

	#region Get Value
	public static object GetFieldOrPropertyValue(object obj, string fieldOrPropertyName, Type targetType = null, object defaultValue = null)
	{
		object value = GetFieldValue(obj, fieldOrPropertyName, targetType, defaultValue);
		if (value == null)
			value = GetPropertyValue(obj, fieldOrPropertyName, targetType, defaultValue);
		return value;
	}
	public static T GetFieldValue<T>(object obj, string propertyName, T defaultValue = null)
	   where T : class
	{
		return GetFieldValue(obj, propertyName, typeof(T), defaultValue) as T;
	}
	public static object GetFieldValue(object obj, string fieldName, Type targetType = null, object defaultValue = null)
	{
		//ToAdd:针对是否需要Instance字段进行处理（如GetValue时传入的值、
		object result = defaultValue;
		if (obj == null || fieldName.IsNullOrEmpty())
			return result;

		Type objType = obj.GetType();
		FieldInfo fieldInfo = objType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
		if (fieldInfo != null)
		{
			if (!(targetType != null && !fieldInfo.FieldType.IsInherit(targetType)))//ToTest
				result = fieldInfo.GetValue(obj);
		}
		return result != null ? result : defaultValue;
	}

	public static T GetPropertyValue<T>(object obj, string propertyName, T defaultValue = null)
		 where T : class
	{
		return GetPropertyValue(obj, propertyName, typeof(T), defaultValue) as T;
	}

	/// <summary>
	/// 获取Property值，支持引用类型（因为IsInherit）及值类型
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="propertyName"></param>
	/// <param name="targetType">确认</param>
	/// <param name="defaultValue"></param>
	/// <returns></returns>
	public static object GetPropertyValue(object obj, string propertyName, Type targetType = null, object defaultValue = null)
	{
		object result = defaultValue;
		if (obj == null || propertyName.IsNullOrEmpty())
			return result;

		//ToAdd
		Type objType = obj.GetType();
		PropertyInfo propertyInfo = objType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
		if (propertyInfo != null && propertyInfo.CanRead)
		{
			if (propertyInfo.PropertyType.IsValueType)//值类型：直接获取对应值
			{
				if (!(targetType != null && propertyInfo.PropertyType != targetType))//如果propertyType不为空，则确认类型是否相同
					result = propertyInfo.GetValue(obj);
			}
			else
			{
				if (propertyInfo.PropertyType.IsInherit(targetType))//引用类型：确认是否为子类
				{
					if (!(targetType != null && !propertyInfo.PropertyType.IsInherit(targetType)))
						result = propertyInfo.GetValue(obj);
				}
			}
		}
		return result != null ? result : defaultValue;
	}

	#endregion

	#region GetInfo
	/// <summary>
	/// Get Unique path for fieldInfo (Mainly for marking the fieldInfo from nested classes)
	/// </summary>
	/// <param name="parentChain"></param>
	/// <param name="fieldInfo"></param>
	/// <param name="indexInCollectionValue">the index in list (if in list)</param>
	/// <returns></returns>
	public static string GetFieldPathChain(string parentChain, FieldInfo fieldInfo, int? indexInCollectionValue = null)
	{
		string result = "";
		if (parentChain.NotNullOrEmpty())
			result = parentChain + ".";
		result += fieldInfo.Name;
		if (indexInCollectionValue != null)
		{
			result += $"[{indexInCollectionValue.Value}]";
		}
		return result;
	}
	#endregion

	/// <summary>Search the assembly for all types that match a predicate</summary>
	/// <param name="assembly">The assembly to search</param>
	/// <param name="predicate">The type to look for</param>
	/// <returns>A list of types found in the assembly that inherit from the predicate</returns>
	public static IEnumerable<Type> GetTypesInAssembly(Assembly assembly, Predicate<Type> predicate)
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

	/// <summary>
	///  Get a type from a name
	///  仅匹配名字，不匹配全名（容易导致混淆，获取到错误的重名）
	/// </summary>
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

	private static List<Type> GetSelfAndBaseTypes(object target)
	{
		List<Type> types = new List<Type>() { target.GetType() };
		while (types.Last().BaseType != null)
		{
			types.Add(types.Last().BaseType);
		}

		return types;
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


	/// <summary>
	/// Create instance for given type
	/// </summary>
	/// <typeparam name="TValue"></typeparam>
	/// <returns></returns>
	public static TValue CreateInstance<TValue>()
	{
		return (TValue)CreateInstance(typeof(TValue));
	}
	public static object CreateInstance(Type type)
	{
		object valueInst = null;
		if (type != null)
		{
			try
			{

				if (type == typeof(string))
					valueInst = "";
				else
					valueInst = Activator.CreateInstance(type);//创建一个新的非空值，适用于VaueType及ClassType
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogError($"CreateInstance for type [{type}] failed!\r\n" + e);
			}
		}
		return valueInst;
	}



	//DeepCopy （https://stackoverflow.com/questions/39092168/c-sharp-copying-unityevent-information-using-reflection）

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

		// Unity Object：返回原引用，避免引用丢失
		else if (type.IsInherit(typeof(UnityEngine.Object)))//UnityEngine.Object是type的基类
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

       
    //public static void CopyPropertiesTo<T, TU>(this T source, TU dest)
    //{
    //    var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
    //    var destProps = typeof(TU).GetProperties()
    //            .Where(x => x.CanWrite)
    //            .ToList();

    //    foreach (var sourceProp in sourceProps)
    //    {
    //        if (destProps.Any(x => x.Name == sourceProp.Name))
    //        {
    //            var p = destProps.First(x => x.Name == sourceProp.Name);
    //            if (p.CanWrite)
    //            { // check if the property can be set or no.
    //                p.SetValue(dest, sourceProp.GetValue(source, null), null);
    //            }
    //        }
    //    }
    //}
#endif
}

public static class ReflectionToolExtension
{
	#region Copy

	public static T DeepCopyReflection<T>(this T other)
	{
		return ReflectionTool.DeepCopy(other);
	}

	#endregion

	#region Type
	public static bool IsInheritOrEqual(this Type objType, Type targetType)
	{
		if (targetType == null)
			return false;
		if (objType == targetType)
			return true;

		return targetType.IsAssignableFrom(objType);
	}
	public static bool IsInherit(this Type objType, Type targetType)
	{
		if (targetType == null)
			return false;
		return targetType.IsAssignableFrom(objType);
	}

	public static FieldInfo GetFieldWithAttribute<TAttribute, TFieldType>(this Type objType, string name)
	where TAttribute : Attribute
	{
		FieldInfo fieldInfo = GetFieldWithAttribute<TAttribute>(objType, name);
		if (fieldInfo == null)
			return null;
		if (fieldInfo.FieldType != typeof(TFieldType))
			return null;
		return fieldInfo;
	}
	public static FieldInfo GetFieldWithAttribute<TAttribute>(this Type objType, string name)
		where TAttribute : Attribute
	{
		if (objType == null)
			return null;

		FieldInfo fieldInfo = objType.GetField(name);
		if (fieldInfo == null)
			return null;

		var targetAttribute = fieldInfo.GetCustomAttribute<TAttribute>();
		if (targetAttribute == null)
			return null;

		return fieldInfo;
	}
	public static FieldInfo GetFieldWithAttribute<TAttribute>(this Type objType)
		   where TAttribute : Attribute
	{
		if (objType == null)
			return null;

		foreach (FieldInfo fi in objType.GetFields())
		{
			if (fi.GetCustomAttribute<TAttribute>() != null)
				return fi;
		}
		return null;
	}

	public static MethodInfo GetMethod(this Type type, string methodName, BindingFlags flags, bool isGenericMethod = false, Type[] argTypes = null)
	{
		return type.GetMethods(flags).FirstOrDefault(
			mI =>
		 {
			 bool isMatch = mI.Name == methodName;
			 if (isGenericMethod)
				 isMatch &= mI.IsGenericMethod;

			 if (argTypes != null)//判断参数是否相同
			 {
				 var curMethodInfoParams = mI.GetParameters();
				 if (curMethodInfoParams.Length != argTypes.Length)
					 return false;

				 for (int i = 0; i < argTypes.Length; i++)
				 {
					 if (curMethodInfoParams[i].ParameterType != argTypes[i])
						 return false;
				 }
			 }


			 return isMatch;
		 });
	}
	/// <summary>
	/// Gets all fields from an instance and its hierarchy inheritance （Except System.Object).
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="flags">The flags.</param>
	/// <returns>All fields of the type.</returns>
	public static List<FieldInfo> GetAllFields(this Type type, BindingFlags flags)
	{
		return type.GetAllObjectElement(flags, (t, bF) => t.GetFields(bF));
	}

	public static List<PropertyInfo> GetAllPropertys(this Type type, BindingFlags flags)
	{
		return type.GetAllObjectElement(flags, (t, bF) => t.GetProperties(bF));
	}
	public static List<MethodInfo> GetAllMethods(this Type type, BindingFlags flags)
	{
		return type.GetAllObjectElement(flags, (t, bF) => t.GetMethods(bF));
	}
	public static List<MemberInfo> GetAllMembers(this Type type, BindingFlags flags)
	{
		return type.GetAllObjectElement(flags, (t, bF) => t.GetMembers(bF));
	}
	static List<TMemberInfo> GetAllObjectElement<TMemberInfo>(this Type type, BindingFlags flags, Func<Type, BindingFlags, TMemberInfo[]> actGetMembers)
	{
		// Early exit if Object type
		if (type == typeof(System.Object))
		{
			return new List<TMemberInfo>();
		}

		// Recursive call(找到基类所有的指定元素）
		var fields = type.BaseType.GetAllObjectElement(flags, actGetMembers);
		TMemberInfo[] arrFI = actGetMembers(type, flags | BindingFlags.DeclaredOnly);
		fields.AddRange(arrFI);
		return fields;
	}

	#endregion

	#region MemberInfo

	//Ref: http://www.java2s.com/example/csharp/system.reflection/get-actual-type-from-memberinfo.html
	public static Type GetActualType(this MemberInfo methodInfo)//ToTest
	{
		Type rawType;
		if (!(methodInfo is Type))
		{
			rawType = methodInfo.GetVariableType();
		}
		else
		{
			rawType = (Type)methodInfo;
		}

		Type memberType;
		if (rawType.IsArray)
		{
			memberType = rawType.GetElementType();
			if (memberType == null)
			{
				throw new Exception(string.Format("Unable to get Type of Array {0} ({1}).", rawType, methodInfo.GetFullMemberName()));
			}
		}
		else
		{
			memberType = rawType;
		}
		return memberType;
	}
	/// <summary>
	/// Find the real type of MemberInfo, get value's type
	/// </summary>
	/// <param name="methodInfo"></param>
	/// <returns></returns>
	public static Type GetVariableType(this MemberInfo methodInfo)
	{
		if (methodInfo != null)
		{
			if (methodInfo is FieldInfo)
			{
				return ((FieldInfo)methodInfo).FieldType;
			}
			if (methodInfo is PropertyInfo)
			{
				return ((PropertyInfo)methodInfo).PropertyType;
			}
			else
			{
				UnityEngine.Debug.LogError("Can only get VariableType of Fields and Properties!");
			}
		}
		return null;
	}
	public static string GetFullMemberName(this MemberInfo member)//ToTest
	{
		var str = member.DeclaringType.FullName + "." + member.Name;
		if (member is MethodInfo)
		{
			str += "(" + ((MethodInfo)member).GetParameters().Select(param => param.ParameterType.Name).ConnectToString(", ") + ")";
		}
		return str;
	}

	#endregion

	#region MethodInfo

	public static string GetUniqueID(this MethodInfo methodInfo, Type objType)
	{
		//https://stackoverflow.com/questions/11193616/how-to-get-a-unique-id-for-a-method-based-on-its-signature-in-c
		return objType.FullName + "."
			+ methodInfo.Name + " "
			+ methodInfo.ReturnType + "(" +
			methodInfo.GetParameters().ConnectToString(",") + ")";
	}

	#endregion

	#region Utility

	public static void ForEachMember<TAttribute>(this Type type, BindingFlags flags, UnityAction<object> act)
		where TAttribute : Attribute
	{

	}

	public static void ForEachMemberWithInterface<TInterface>(this object obj, UnityAction<TInterface> act, bool isRecursive = true, bool includeSelf = true, int maxDepth = 7)
	{
		if (obj == null)
			return;

		if (includeSelf)
			if (obj is TInterface inst)
				act.Execute(inst);

		if (maxDepth == -1)
			return;
		//PS:功能比较复杂，不适合使用AlgorithmTool.Recursive

		foreach (FieldInfo fieldInfo in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
		{
			if (!fieldInfo.FieldType.IsClass)
				continue;
			object objField = fieldInfo.GetValue(obj);
			if (objField == null)
				continue;

			if (isRecursive)
				objField.ForEachMemberWithInterface(act, true, true, --maxDepth);
			else
			{
				if (objField is TInterface inst)
					act.Execute(inst);
			}
		}
	}

	#endregion
}