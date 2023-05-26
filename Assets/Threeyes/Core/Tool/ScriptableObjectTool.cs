using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
/// <summary>
/// Bug:
/// 1.如果使用了List<SO>而且本地没有对应的PersistentData，那会因为TargetValue的List没有克隆，从而导致直接修改List中的SO；如果本地有PersistentData那就没有这个问题（因为是修改实时Load的数据）。因此强烈建议不要用嵌套的SO实现！
/// 
/// ToUpdate：针对Gradient进行特殊处理
/// </summary>
public static class ScriptableObjectTool
{
	public static void Copy(ScriptableObject source, ScriptableObject target, Func<Type, MemberInfo, bool> funcCopyFilter = null)
	{
		CopyFields(source, target, funcCopyFilter: funcCopyFilter);
	}

	/// <summary>
	/// Instantiate SO in depth (include sub list's SO）
	/// 克隆包括List中的SO，从而杜绝引用失效
	/// </summary>
	/// <typeparam name="TSO"></typeparam>
	/// <param name="original"></param>
	/// <returns></returns>
	public static TSO DeepInstantiate<TSO>(TSO original)
		where TSO : ScriptableObject
	{
		//#1 创建 Clone
		TSO clone = UnityEngine.Object.Instantiate(original);
		DeepInstantiateField(original, clone);
		return clone;
	}
	static void DeepInstantiateField(object original, object clone)
	{
		if (original == null || clone == null)
			return;

		Type objType = original.GetType();
		foreach (FieldInfo fieldInfo in objType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
		{
			if (!IsTypeCopiable(fieldInfo))
				continue;

			object fieldValue = fieldInfo.GetValue(original);
			Type fieldType = fieldInfo.FieldType;

			//主要工作：遍历List<SO>并克隆实例
			if (fieldType.GetInterface(nameof(IList)) != null)//List<T>、T[]等合集
			{
				Type itemType = ReflectionTool.GetCollectionElementType(fieldType);
				if (itemType.IsInherit(ScriptableObjectType))//SO类型
				{
					var srcList = fieldValue as IList;
					if (srcList.IsFixedSize)// Array(固定大小）
					{
						if (fieldType.IsArray)
						{
							if (itemType.IsInherit(ScriptableObjectType))
							{
								var array = fieldValue as Array;
								Array copied = Array.CreateInstance(itemType, array.Length);
								for (int i = 0; i < array.Length; i++)
								{
									var eleValue = array.GetValue(i) as ScriptableObject;
									if (eleValue)
										copied.SetValue(UnityEngine.Object.Instantiate(eleValue), i);
								}
								fieldInfo.SetValue(clone, Convert.ChangeType(copied, fieldValue.GetType()));
							}
						}
					}
					else//List(可变大小）
					{
						var srcListCopy = Activator.CreateInstance(fieldValue.GetType()) as IList;
						for (int i = 0; i < srcList.Count; i++)
						{
							var eleValue = srcList[i] as ScriptableObject;
							if (eleValue)
								srcListCopy.Add(UnityEngine.Object.Instantiate(eleValue));
						}
						fieldInfo.SetValue(clone, Convert.ChangeType(srcListCopy, fieldValue.GetType()));
					}
				}
				else//普通类型
				{
					fieldInfo.SetValue(clone, GetFieldClone(fieldInfo.GetValue(original)));
				}
			}
			else if (fieldType.IsClass)// 自定义Class -> Recursion
			{
				var originInst = fieldInfo.GetValue(original);
				var cloneInst = fieldInfo.GetValue(clone);
				DeepInstantiateField(originInst, cloneInst);
				fieldInfo.SetValue(clone, cloneInst);
			}
		}

	}

	/// <summary>
	/// 复制SO的字段到另一个SO中
	/// 
	/// （忽略UnityObject、Action等）
	/// </summary>
	/// <param name="srcObj"></param>
	/// <param name="dstObj"></param>
	/// <param name="bindingAttr"></param>
	/// <param name="funcCopyFilter"></param>
	/// <param name="maxDepth"></param>
	static void CopyFields(object srcObj, object dstObj, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Func<Type, MemberInfo, bool> funcCopyFilter = null, int maxDepth = 7, bool includeUnityObject = false)
	{
		maxDepth--;
		if (maxDepth == -1)
			return;

		if (srcObj == null || dstObj == null)
			return;


		Type srcType = srcObj.GetType();
		bool isSrcInheritFromSO = srcType.IsInherit(ScriptableObjectType);//确认是否继承SO
		bool isSrcInheritFromUnityObject = srcType.IsInherit(UnityObjectType);//确认是否继承Unity.Object

		//ToDelete
		//Func<Type, MemberInfo, bool> customFilter = null;
		//if (dst is ICopyFilter copyFilter)//自定义的筛选器
		//    customFilter = copyFilter.ShouldCopy;

		//针对子类
		foreach (FieldInfo fieldInfo in srcType.GetAllFields(bindingAttr))
		{
			if (!IsTypeCopiable(fieldInfo))
				continue;

			Type fieldType = fieldInfo.FieldType;

			//if (customFilter != null && !customFilter(type, fieldInfo))//自定义的筛选器
			//    continue;
			if (funcCopyFilter != null && !funcCopyFilter(fieldType, fieldInfo))//忽略不能复制的
				continue;

			if (fieldType.IsInherit(ScriptableObjectType))//SO(强制处理）
			{
				CopyFields(fieldInfo.GetValue(srcObj), fieldInfo.GetValue(dstObj), bindingAttr, funcCopyFilter, maxDepth);
				continue;
			}

			if (fieldType.IsValueType || fieldType == typeof(string) || fieldType.IsInheritOrEqual(UnityObjectType))//Value/string type+UnityEngine.Object的子类：直接克隆原值
			{
				fieldInfo.SetValue(dstObj, GetFieldClone(fieldInfo.GetValue(srcObj), funcCopyFilter, maxDepth, bindingAttr));
			}
			else if (fieldType.GetInterface(nameof(IList)) != null)//List<T>、T[]等合集
			{
				//PS:针对Array、List<SO>进行迭代，而不是直接创建Clone，因此无法直接调用GetFieldClone
				//ToAdd:Array

				//IList<T>
				var srcList = fieldInfo.GetValue(srcObj) as IList;
				var dstList = fieldInfo.GetValue(dstObj) as IList;
				if (srcList != null && dstList != null)//排除List未初始化导致为null的情况
				{
					Type elementType = ReflectionTool.GetCollectionElementType(fieldType);

					if (elementType != null && elementType.IsInherit(ScriptableObjectType)) //SO元素：其作用是为了供程序内部引用，为了避免引用丢失，需要迭代调用本方法复制其字段内容而不是返回原值（前提是SO的具体类型一致）【太复杂，ToDelete】
					{
						if (dstList.Count == srcList.Count)//只有合集长度相同才复制，避免影响原来的List的长度、位置或顺序
						{
							for (int i = 0; i != dstList.Count; i++)
							{
								CopyFields(srcList[i], dstList[i], bindingAttr, funcCopyFilter, maxDepth);
							}
						}
					}
					else//其他元素：直接使用新List覆盖
					{
						//#1 确保元素数量一致
						if (srcList.Count > dstList.Count)//Dst元素过少：填充对应的默认实例
						{
							int startIndex = dstList.Count;
							int totalCount = srcList.Count;
							for (int i = startIndex; i != totalCount; i++)
							{
								object tempElement =
								ReflectionTool.CreateInstance(elementType);
								dstList.Add(tempElement);
							}
						}
						else if (srcList.Count < dstList.Count)//Dst元素过多：删掉多余元素
						{
							while (srcList.Count != dstList.Count)
								dstList.RemoveAt(dstList.Count - 1);//移除最后一位直到数量相同
						}

						//#2 克隆相同元素
						for (int i = 0; i != dstList.Count; i++)
						{
							//ToUpdate：针对Texture，不应该拷贝，否则List<Class>中的Texture会被拷贝（参考CopyFields(srcList[i], dstList[i], bindingAttr, funcCopyFilter, maxDepth);的正确实现）
							if (IsSimpleType(elementType))
								dstList[i] = GetFieldClone(srcList[i], funcCopyFilter, maxDepth, bindingAttr);
							else
								CopyFields(srcList[i], dstList[i], bindingAttr, funcCopyFilter, maxDepth);
						}
						//else//数量不同：直接复制List整体值
						//{
						//	//【Bug】【ToFix】：会直接克隆list字段，导致引用相同；另外List中的部分字段可能是不应该克隆的（如Texture）。应该更新为逐个元素筛选及克隆
						//	fieldInfo.SetValue(dstObj, GetFieldClone(fieldInfo.GetValue(srcObj), funcCopyFilter, maxDepth, bindingAttr));

						//	//for (int i = 0; i != dstList.Count; i++)
						//	//{
						//	//		CopyFields(srcList[i], dstList[i], bindingAttr, funcCopyFilter, maxDepth);
						//	//}
						//}
					}
				}
			}
			else if (fieldType.IsClass)// 自定义Class -> Recursion (克隆并筛选)
			{
				if (SetFieldClone_UnityClass(fieldInfo, srcObj, dstObj))//Unity定制Class:进行特殊Clone
				{

				}
				else
				{
					var srcFieldValue = fieldInfo.GetValue(srcObj);
					if (srcFieldValue != null)
					{
						var dstFieldValue = fieldInfo.GetValue(dstObj);

						//PS:如果dstFieldValue为Null，则为其创建实例（可能原因：更改或新增字段）
						if (dstFieldValue == null)
						{
							try
							{
								dstFieldValue = srcFieldValue.Copy();//使用第三方插件进行类克隆，能够避免克隆出错
								fieldInfo.SetValue(dstObj, dstFieldValue);
							}
							catch (Exception e)
							{
								Debug.LogError("Copy src failed: " + e);
							}
						}
						CopyFields(srcFieldValue, dstFieldValue, bindingAttr, funcCopyFilter, maxDepth);
					}
				}
			}
			else//Others：interface, etc
			{
				//Debug.LogError("Ignore type: " + fieldInfo.Name + " " + fieldType);
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public static bool IsSimpleType(Type type)
	{
		return type.IsValueType || type == typeof(string);
	}
	//void CreateListInstance<TElement>()

	/// <summary>
	/// 获取Field的Clone
	/// </summary>
	/// <param name="srcFieldValue"></param>
	/// <param name="funcCopyFilter"></param>
	/// <param name="maxDepth"></param>
	/// <param name="dst">如果是引用类型，则需要传入这个参数</param>
	/// <returns></returns>
	public static object GetFieldClone(object srcFieldValue, Func<Type, MemberInfo, bool> funcCopyFilter = null, int maxDepth = 7, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
	{
		if (srcFieldValue == null)
			return null;

		maxDepth--;
		var srcType = srcFieldValue.GetType();

		if (srcType.IsValueType || srcType == typeof(string))// Value type
		{
			return srcFieldValue;
		}
		else if (srcType.IsInheritOrEqual(UnityObjectType))//type是UnityEngine.Object的子类：返回原引用，避免丢失（如Asset中的GameObject，Material、AudioSource等）（因为序列化也不支持这些字段，因此没有克隆的必要）
		{
			return srcFieldValue;
		}
		else if (srcType.IsInherit(typeof(IList)))//List<T>、T[]等合集
		{
			if (maxDepth == -1)//跳出循环，直接赋空值
				return null;

			//Ref：RuntimeInspectorNamespace/ArrayField
			///ToUpdate:
			///-如果 srcFieldValue 为null，则需要创建一个长度为空的默认类返回，否则会出现访问错误的问题
			///-兼容复杂List的情况
			if (srcType.IsArray)//固定长度
			{
				var array = srcFieldValue as Array;
				Type elementType = srcType.GetElementType();
				Array copied = Array.CreateInstance(elementType, array != null ? array.Length : 0);//PS:不管原Array是否为null，返回的Clone都不能为null，否则访问会报错
				if (array != null)
					for (int i = 0; i < array.Length; i++)
					{
						copied.SetValue(GetFieldClone(array.GetValue(i), funcCopyFilter, maxDepth, bindingAttr), i);
					}
				return Convert.ChangeType(copied, srcFieldValue.GetType());
			}
			else//IList
			{
				IList copied = Activator.CreateInstance(srcFieldValue.GetType()) as IList;
				var srcList = srcFieldValue as IList;
				if (srcList != null)
					for (int i = 0; i < srcList.Count; i++)
					{
						copied.Add(GetFieldClone(srcList[i], funcCopyFilter, maxDepth, bindingAttr));
					}
				return Convert.ChangeType(copied, srcFieldValue.GetType());
			}

		}
		else if (srcType.IsClass)// 自定义Class -> Recursion
		{
			if (maxDepth == -1)//跳出循环，直接赋空值
				return null;

			//克隆类实例
			var copy = Activator.CreateInstance(srcFieldValue.GetType());
			foreach (FieldInfo fieldInfo in srcType.GetAllFields(bindingAttr))
			{
				if (funcCopyFilter != null && !funcCopyFilter(srcType, fieldInfo))//忽略不能复制的
					continue;
				var subSrcFieldValue = fieldInfo.GetValue(srcFieldValue);
				if (subSrcFieldValue != null)
				{
					var dstFieldValue = GetFieldClone(subSrcFieldValue, funcCopyFilter, maxDepth, bindingAttr);
					fieldInfo.SetValue(copy, dstFieldValue);
				}
			}
			return copy;
		}
		// Fallback
		else
		{
			Debug.LogError("Unknown type:" + srcFieldValue.GetType());
			return null;
		}
	}


	/// <summary>
	/// 创建Array（固定长度）/List的对应列表（如果为空则创建空表），避免srcObj为null时返回null值
	/// </summary>
	/// <param name="collectionType"></param>
	/// <param name="length"></param>
	/// <returns></returns>
	public static object CreateCollectionInstance(Type collectionType, object srcObj)
	{
		object result = null;
		if (collectionType.IsInherit(typeof(IList)))
		{
			if (collectionType.IsArray)
			{
				result = Array.CreateInstance(collectionType, srcObj == null ? 0 : (srcObj as Array).Length);//Array需要在创建时设置固定大小
			}
			else
			{
				result = Activator.CreateInstance(collectionType);
			}
		}
		return result;
	}


	/// <summary>
	/// 获取Unity自定义类的Clone
	/// （eg：Gradient，因为其内部使用IntPtr保存，会导致引用同一个类)
	/// </summary>
	/// <param name="srcObj"></param>
	/// <param name=""></param>
	/// <returns>是否已经拷贝</returns>
	public static bool SetFieldClone_UnityClass(FieldInfo fieldInfo, object srcObj, object dstObj)
	{
		if (fieldInfo == null || srcObj == null || dstObj == null)
			return false;

		if (fieldInfo.FieldType == typeof(Gradient))
		{
			Gradient srcGradient = fieldInfo.GetValue(srcObj) as Gradient;
			if (srcGradient != null)
			{
				//PS：因为该类字段有指针，所有不能直接拷贝实例所有字段，而是创建新实例
				Gradient clone = new Gradient();
				clone.mode = srcGradient.mode;
				clone.SetKeys(srcGradient.colorKeys, srcGradient.alphaKeys);
				fieldInfo.SetValue(dstObj, srcGradient);
				return true;
			}
		}
		//ToUpdate：增加AnimationCurve
		return false;
	}


	static Type ScriptableObjectType { get { return typeof(ScriptableObject); } }
	static Type UnityObjectType { get { return typeof(UnityEngine.Object); } }
	static List<string> listIgnoreFieldTypeName
	  = new List<string>()
	  {
            //忽略delegate（因为委托是与实例绑定的，拷贝后会导致SO的Action被覆盖掉）

            //以下有可能是泛型，因为通过名称判断
            "System.Action",
			"UnityEngine.Events.UnityAction",
			"Dictionary",

		  //event对应EventInfo暂时不考虑，后续可加上
	  };
	static List<Type> listIgnoreFieldType = new List<Type>()
	{
			typeof(IntPtr),//拷贝该字段会导致闪退！PS：AnimationCurve 等会包含该字段
            
            //Unity
            typeof(AnimationCurve),//ToSupport（参考Gradient）
    };
	public static bool IsTypeCopiable(FieldInfo fieldInfo)
	{
		if (fieldInfo == null)
			return false;
		Type fieldType = fieldInfo.FieldType;
		if (fieldInfo.DeclaringType == UnityObjectType)//忽略Object中声明的字段，如name、m_InstanceID等，同时避免拷贝m_InstanceID导致指向同一个实例（PS：不能整合到IsTypeCopiable中，否则会导致DeepInstantiate失败）
			return false;

		return
			!listIgnoreFieldTypeName.Any((s) => fieldType.ToString().Contains(s)) &&
			!listIgnoreFieldType.Any(t => t == fieldType);
	}

	///ToDelete
	/////缺点：
	/////1.如果是List<SO>,那SO就被整个替代，原有的引用也都不存在
	////正对SO的反射拷贝字段：保留SO最初的场景引用
	//public static void CopyFields(ScriptableObject src, ScriptableObject dst, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Func<Type, MemberInfo, bool> funcCopyFilter = null, int maxDepth = 7)
	//{
	//    if (src != null && dst != null)
	//    {
	//        Type type = src.GetType();
	//        FieldInfo[] fields = type.GetFields(bindingAttr);

	//        //ToDelete
	//        //Func<Type, MemberInfo, bool> customFilter = null;
	//        //if (dst is ICopyFilter copyFilter)//自定义的筛选器
	//        //    customFilter = copyFilter.ShouldCopy;

	//        for (int i = 0; i < fields.Length; ++i)
	//        {
	//            FieldInfo fieldInfo = fields[i];

	//            //if (customFilter != null && !customFilter(type, fieldInfo))//自定义的筛选器
	//            //    continue;

	//            if (funcCopyFilter != null && !funcCopyFilter(type, fieldInfo))//忽略不能复制的
	//                continue;

	//            //fieldInfo.SetValue(dst, fieldInfo.GetValue(src));
	//            var fieldValue = fieldInfo.GetValue(src);
	//            if (fieldValue != null)
	//            {
	//                fieldInfo.SetValue(dst, DoCopy(fieldValue, funcCopyFilter, maxDepth: maxDepth));
	//            }
	//        }
	//    }
	//}

	//保留UnityObject的相关引用，穷举复制其他类/值
	//public static object DoCopy(object src, Func<Type, MemberInfo, bool> funcCopyFilter = null, int maxDepth = 7)
	//{
	//	if (src == null)
	//		return null;

	//	maxDepth--;
	//	var srcType = src.GetType();

	//	// Value type
	//	if (srcType.IsValueType || srcType == typeof(string))
	//	{
	//		return src;
	//	}
	//	//type是UnityEngine.Object的子类：返回原引用，避免丢失（如Asset中的GameObject，Material、AudioSource等）
	//	else if (typeof(UnityEngine.Object).IsAssignableFrom(srcType))
	//	{
	//		return src;
	//	}
	//	//ToAdd：List
	//	else if (srcType.IsArray)// Array
	//	{
	//		Type elementType = srcType.GetElementType();
	//		var array = src as Array;
	//		Array copied = Array.CreateInstance(elementType, array.Length);
	//		for (int i = 0; i < array.Length; i++)
	//		{
	//			copied.SetValue(DoCopy(array.GetValue(i), maxDepth: maxDepth), i);
	//		}
	//		return Convert.ChangeType(copied, src.GetType());
	//	}
	//	else if (srcType.IsClass)// 自定义Class -> Recursion
	//	{
	//		////ToDelete
	//		//foreach (MemberInfo memberInfo in type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
	//		//{
	//		//    if (memberInfo.Name == "actionPersistentChanged")
	//		//        Debug.LogError(memberInfo.Name);
	//		//}
	//		if (maxDepth == -1)//跳出循环，直接赋空值
	//			return null;

	//		//PS:因为是调用特定Constructor，因此要先判断
	//		try
	//		{
	//			var copy = Activator.CreateInstance(srcType);
	//			foreach (FieldInfo fieldInfo in srcType.GetAllFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
	//			{
	//				if (funcCopyFilter != null && !funcCopyFilter(srcType, fieldInfo))//忽略不能复制的
	//					continue;

	//				//ToAdd:忽略Action等
	//				if (!IsTypeCopiable(fieldInfo))
	//					continue;


	//				var srcFieldValue = fieldInfo.GetValue(src);
	//				if (srcFieldValue != null)
	//				{
	//					var dstFieldValue = DoCopy(srcFieldValue, maxDepth: maxDepth);
	//					fieldInfo.SetValue(copy, dstFieldValue);
	//				}
	//			}
	//			return copy;
	//		}
	//		catch (Exception e)
	//		{
	//			Debug.LogError($"CreateInstance for {srcType} failed: " + e);
	//		}
	//	}
	//	// Fallback
	//	else
	//	{
	//		throw new ArgumentException("Unknown type");
	//	}
	//	return null;
	//}
}
