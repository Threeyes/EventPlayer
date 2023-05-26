using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
public partial class PathTool
{
	#region 常用定义

	/// <summary>
	/// 项目文件夹（Application.dataPath的上层文件夹）
	/// </summary>
	/// <returns></returns>
	public static string ProjectDirPath
	{
		get
		{
			///Application.dataPath的路径：
			///[Unity Editor]:Asset文件夹
			///[Win]:  executablename_Data 文件夹 

			string path = Application.dataPath;
			DirectoryInfo directory = Directory.GetParent(path);
			return directory.FullName;
		}
	}

	#endregion

	public static string GetAbsPath(string parentDir, string filePath)
	{
		if (parentDir.IsNullOrEmpty() && filePath.NotNullOrEmpty())
			return filePath;

		if (IsUriPath(filePath))//Url通常为绝对路径
			return filePath;

		//如果传入值是相对路径，就转为全局路径
		if (parentDir.NotNullOrEmpty() && filePath.NotNullOrEmpty() && !IsRootPath(filePath))
		{
			filePath = Path.Combine(parentDir, filePath);//PS:任意字段为空会报错
		}
		return filePath;
	}

	public static bool IsUriPath(string filePath)
	{
		//ToUpdate：待使用更加通用的方法，兼容Uri的http/ftp等格式
		//Uri uri = new Uri(filePath);
		//uri.Scheme == Uri.UriSchemeHttp||

		//兼容本地文件以及http/https
		if (filePath.NotNullOrEmpty())
			return filePath.StartsWith("file:///") || filePath.StartsWith("http");
		return false;
	}

	/// <summary>
	/// 检查是否为绝对路径
	/// 例子：
	/// C:\Scripts: true
	/// ..\Scripts: false
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public static bool IsRootPath(string path)
	{
		return Path.IsPathRooted(path);//https://social.technet.microsoft.com/forums/windowsserver/en-US/1edfcd49-7104-40d7-bf31-bf731995b775/net-checking-for-absolute-and-relative-paths
	}

	public static bool IsProjectDir(string targetDir)
	{
		return IsSubDir(targetDir, Application.dataPath);
	}
	/// <summary>
	/// 检查目标路径是否为给定父文件夹的子文件夹
	/// </summary>
	/// <param name="targetDir"></param>
	/// <param name="parentDir"></param>
	/// <returns></returns>
	public static bool IsSubDir(string targetDir, string parentDir)
	{
		
		return Path.GetFullPath(targetDir).Contains(Path.GetFullPath(parentDir));//优化：能够识别../等特殊字符
		//return ConvertToSystemFormat(targetDir).Contains(ConvertToSystemFormat(parentDir));
	}
	public static string GetFileName(string filePath)
	{
		if (filePath.IsNullOrEmpty())
			return null;
		try
		{
			FileInfo fileInfo = new FileInfo(filePath);
			return fileInfo.Name;
		}
		catch (Exception e)
		{
			Debug.LogError("GetFileName Failed！\r\n" + e);
		}
		return "";
	}
	public static string GetDirectoryName(string dirPath)
	{
		try
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
			return directoryInfo.Name;
		}
		catch (Exception e)
		{
			///SecurityException
			///NotSupportedException
			///IOException
			///ArgumentException
			Debug.LogError("GetDirectoryName Failed！\r\n" + e);
		}
		return "";
	}
	public static string GetOrCreateFileParentDir(string filePath)
	{
		DirectoryInfo directoryInfo = GetParentDirectory(filePath);
		try
		{
			//PS:Android端尝试创建会报错；
			if (!IsAndroidStreamingAssetsPath(filePath))
				if (!directoryInfo.Exists)
					directoryInfo.Create();
		}
		catch (Exception e)
		{
			Debug.LogError("GetOrCreateFileParentDir failed with error:\r\n" + e);
		}
		return directoryInfo.FullName;
	}

	/// <summary>
	/// 创建文件夹
	/// </summary>
	/// <param name="dirPath"></param>
	/// <returns></returns>
	public static string GetOrCreateDir(string dirPath)
	{
		try
		{
			if (!IsAndroidStreamingAssetsPath(dirPath))
				if (!Directory.Exists(dirPath))
					Directory.CreateDirectory(dirPath);
		}
		catch (Exception e)
		{
			Debug.LogError("CreateDirectory Failed!\r\n" + e);
		}
		return dirPath;
	}

	//PS：安卓端StreamingAssets路径只读（因为在压缩包中）
	public static bool IsAndroidStreamingAssetsPath(string filePath)
	{
		if (!Application.isEditor && Application.platform == RuntimePlatform.Android)
		{
			return filePath.StartsWith("jar:file://");
		}
		return false;
	}

	public static void DirectoryDelete(string dirName, bool isIncludeSelf = false)
	{
		try
		{
			// Get the subdirectories for the specified directory.
			DirectoryInfo dirParent = new DirectoryInfo(dirName);
			Debug.Log("开始删除文件夹中的文件！\r\n" + dirName);

			if (!dirParent.Exists)
			{
				return;
			}
			foreach (var subDir in dirParent.GetDirectories())
			{
				subDir.Delete(true);
			}
			foreach (var subFile in dirParent.GetFiles())
			{
				subFile.Delete();
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Delete Dir Error:" + e);
		}
	}

	public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
	{
		try
		{
			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDirName);
			}

			DirectoryInfo[] dirs = dir.GetDirectories();
			// If the destination directory doesn't exist, create it.
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string temppath = Path.Combine(destDirName, file.Name);
				Debug.Log("Copy " + file.FullName + " to \r\n" + temppath);
				file.CopyTo(temppath, true);
			}

			// If copying subdirectories, copy them and their contents to new location.
			if (copySubDirs)
			{
				foreach (DirectoryInfo subdir in dirs)
				{
					string temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Copy Dir Error:" + e);
		}
	}


	/// <summary>
	/// 获取安卓设备的根目录
	/// RootPath= /storage/emulated/0/
	/// </summary>
	/// <returns></returns>
	public static string GetAndroidRootPath()
	{
		string path = "";
#if UNITY_ANDROID
        try
        {
            string[] srcs = Application.persistentDataPath.Split(new string[1] { "Android" }, System.StringSplitOptions.None);
            if (srcs.Length > 0)
            {
                string result = srcs[0];
                path = result.Substring(0, result.Length - 1);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("设置根目录出错：\r\n" + e);
        }
#endif

		return path;
	}
	/// <summary>
	/// 获取父文件夹路径
	/// </summary>
	/// <param name="filePath"></param>
	/// <returns></returns>
	public static DirectoryInfo GetParentDirectory(string filePath)
	{
		return Directory.GetParent(filePath);
	}

	//——ReadOnly——

	/// <summary>
	///UnitySDK的格式 (用/分隔)
	/// </summary>
	/// <param name="rawPath"></param>
	/// <returns></returns>
	public static string ConvertToUnityFormat(string rawPath)
	{
		return rawPath.Replace(@"\", "/");
	}

	/// <summary>
	/// 转换为File可以识别的格式(用\分隔)
	/// （UnitySDK的路径通常使用/分隔，需要使用该方法转换，如Application.dataPath）
	/// </summary>
	/// <param name="rawPath"></param>
	/// <returns></returns>
	public static string ConvertToSystemFormat(string rawPath)
	{
		return rawPath.Replace("/", @"\");
	}
	/// <summary>
	/// 获取特定深度下的子文件夹
	/// </summary>
	/// <param name="dIParent"></param>
	/// <param name="depth">搜索深度,0代表首层， -1代表无穷</param>
	/// <returns></returns>
	public static List<DirectoryInfo> GetSubDirectories(DirectoryInfo dIParent, int depth = -1)
	{
		List<DirectoryInfo> listDI = new List<DirectoryInfo>();

		foreach (var di in dIParent.GetDirectories())
		{
			listDI.Add(di);

			if (depth == 0)//结束
			{

			}
			else if (depth > 0)
			{
				listDI.AddRange(GetSubDirectories(di, depth - 1));
			}
			else if (depth < 0)//一直遍历到最底层
			{
				listDI.AddRange(GetSubDirectories(di, -1));
			}
		}
		return listDI;
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="dIParent"></param>
	/// <param name="depth"></param>
	/// <param name="searchPattern">(eg:*.dat)</param>
	/// <returns></returns>
	public static List<FileInfo> GetSubFiles(DirectoryInfo dIParent, int depth = -1, string searchPattern = "")
	{
		List<DirectoryInfo> listDI = GetSubDirectories(dIParent, depth);
		List<FileInfo> listFI = new List<FileInfo>();

		listFI.AddRange(searchPattern.IsNullOrEmpty() ? dIParent.GetFiles() : dIParent.GetFiles(searchPattern));
		foreach (var di in listDI)
		{
			listFI.AddRange(searchPattern.IsNullOrEmpty() ? di.GetFiles() : di.GetFiles(searchPattern));
		}
		return listFI;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="externalFileLocation"></param>
	/// <param name="subPath"></param>
	/// <param name="withFilePrefix"></param>
	/// <returns></returns>
	public static string GetPath(ExternalFileLocation externalFileLocation, string subPath, bool withFilePrefix = true)
	{
		string resultPath = "";
		//对应路径：https://www.cnblogs.com/vsirWaiter/p/5340284.html
		switch (externalFileLocation)
		{
			case ExternalFileLocation.CustomPath://完全自定义的路径
				if (Application.platform == RuntimePlatform.Android && withFilePrefix)
					resultPath = "file://";
				resultPath += subPath;
				break;
			case ExternalFileLocation.StreamingAsset:
				//PC/Android 通用路径
				//if (Application.platform != RuntimePlatform.Android && withFilePrefix)
				//{
				//    resultPath = "file://";
				//    //Android的streamingAssets路径= jar:file:///data/app/xxx.xxx.xxx.apk/!/assets //PS:安卓已经有该前缀，所以不用加file://
				//}
				resultPath += Application.streamingAssetsPath + "/" + subPath;
				break;
			case ExternalFileLocation.PersistentDataPath:
				//Android注意：
				//-本地持久化的目录，不是安卓apk包里面的东西，所以不需要加jar:
				//-Play on awake defaults to true. Set it to false to avoid the url set below to auto-start playback since we're in Start().
				if (Application.platform == RuntimePlatform.Android && withFilePrefix)
					resultPath = "file://";
				resultPath += Application.persistentDataPath + "/" + subPath;
				break;
			case ExternalFileLocation.AndroidRoot:
				resultPath = "file://" + GetAndroidRootPath() + subPath;
				break;

			case ExternalFileLocation.CustomData:
				if (Application.platform == RuntimePlatform.Android && withFilePrefix)
					resultPath = "file://";
				resultPath += PathDefinition.dataFolderPath + "/" + subPath;
				break;
		}
		return resultPath;
	}

	/// <summary>
	/// 打开文件所在位置
	/// </summary>
	/// <param name="filePath"></param>
	/// <param name="isSelect"></param>
	public static void OpenFile(string filePath, bool isSelect = true)
	{
		string cmd = "explorer.exe";
		string arg = ConvertToSystemFormat(filePath); //(PS:；路径只能是\\）
		if (isSelect)
			arg = "/select, " + arg;
		Debug.Log(filePath);
		System.Diagnostics.Process.Start(cmd, arg);
	}

	/// <summary>
	/// 打开文件夹
	/// </summary>
	/// <param name="folderPath"></param>
	public static void OpenFolder(string folderPath)
	{
		System.Diagnostics.Process.Start(folderPath);
	}
	/// <summary>
	/// 打开文件所在文件夹
	/// </summary>
	/// <param name="filePath"></param>
	public static void OpenFileFolder(string filePath)
	{
		System.Diagnostics.Process.Start(new FileInfo(filePath).Directory.FullName);
	}

	/// <summary>
	/// 获取快捷方式.lnk的路径
	/// 参考：https://www.kittell.net/code/windows-shortcut-lnk-details/
	/// Bug：不能识别中文
	/// </summary>
	/// <param name="file"></param>
	/// <returns></returns>
	public static string GetShortcutTarget(string file)
	{
		try
		{
			if (System.IO.Path.GetExtension(file).ToLower() != ".lnk")
			{
				throw new Exception("Supplied file must be a .LNK file");
			}

			FileStream fileStream = File.Open(file, FileMode.Open, FileAccess.Read);
			using (System.IO.BinaryReader fileReader = new BinaryReader(fileStream, System.Text.Encoding.Default))
			{
				fileStream.Seek(0x14, SeekOrigin.Begin);     // Seek to flags
				uint flags = fileReader.ReadUInt32();        // Read flags
				if ((flags & 1) == 1)
				{                      // Bit 1 set means we have to
									   // skip the shell item ID list
					fileStream.Seek(0x4c, SeekOrigin.Begin); // Seek to the end of the header
					uint offset = fileReader.ReadUInt16();   // Read the length of the Shell item ID list
					fileStream.Seek(offset, SeekOrigin.Current); // Seek past it (to the file locator info)
				}

				long fileInfoStartsAt = fileStream.Position; // Store the offset where the file info
															 // structure begins
				uint totalStructLength = fileReader.ReadUInt32(); // read the length of the whole struct
				fileStream.Seek(0xc, SeekOrigin.Current); // seek to offset to base pathname
				uint fileOffset = fileReader.ReadUInt32(); // read offset to base pathname
														   // the offset is from the beginning of the file info struct (fileInfoStartsAt)
				fileStream.Seek((fileInfoStartsAt + fileOffset), SeekOrigin.Begin); // Seek to beginning of
																					// base pathname (target)
				long pathLength = (totalStructLength + fileInfoStartsAt) - fileStream.Position - 2; // read the base pathname. I don't need the 2 terminating nulls.
				char[] linkTarget = fileReader.ReadChars((int)pathLength); // should be unicode safe
				var link = new string(linkTarget);

				int begin = link.IndexOf("\0\0");
				if (begin > -1)
				{
					int end = link.IndexOf("\\\\", begin + 2) + 2;
					end = link.IndexOf('\0', end) + 1;

					string firstPart = link.Substring(0, begin);
					string secondPart = link.Substring(end);

					string result = firstPart + secondPart;

					//string convertText = System.Text.Encoding.GetEncoding("GB2312").GetString(linkTarget);
					return result;
				}
				else
				{
					return link;
				}
			}
		}
		catch (Exception e)
		{
			throw e;
		}
	}

	public static bool TryMoveFile(string sourceFileName, string destFileName)
	{
		if (File.Exists(sourceFileName))
		{
			File.Move(sourceFileName, destFileName);
			return true;
		}
		return false;
	}

	public static bool TryMoveDir(string sourceDirName, string destDirName)
	{
		if (Directory.Exists(sourceDirName))
		{
			Directory.Move(sourceDirName, destDirName);
			return true;
		}
		return false;
	}

	#region Judge

	public static bool IsValidFileName(string fileName)
	{
		string strErrinfo = "";
		bool result = IsValidFileName(fileName, ref strErrinfo);
		return result;
	}
	public static bool IsValidFileName(string fileName, ref string strErrorInfo)
	{
		//PS:当名字长度为0时不返回false，
		if (fileName.Length == 0)
		{
			strErrorInfo = "The value is empty!";
			return false;
		}

		//#1 判断文件名是否包含无效字符
		//https://stackoverflow.com/questions/17792883/regex-directory-name-validation
		var reserved = Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars());
		foreach (char invalidChar in reserved)
		{
			if (fileName.Contains(invalidChar))
			{
				//提示当前错误字符
				strErrorInfo = $"The value has invalid char: {invalidChar}";
				return false;
			}
		}
		//其他限制：纯空格开头
		if (fileName.StartsWith(" "))//判断是否为空格
		{
			strErrorInfo = $"The value can't start with empty char!";//只提示当前错误字符
			return false;
		}

		return true;
	}

	public static bool IsValidDirName(string fileName)
	{
		string strErrinfo = "";
		bool result = IsValidDirName(fileName, ref strErrinfo);
		return result;
	}
	static List<string> listInvalidDirName = new List<string>() { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };//Windows默认占用文件夹名称 (https://social.technet.microsoft.com/Forums/en-US/e22c021d-d188-4ff2-a4dd-b5d58d979c58/the-specified-device-name-is-invalid?forum=w7itprogeneral)
	/// <summary>
	/// 检查文件名是否可用
	/// PS:忽略名字长度为0的情况,避免Helpbox出现
	/// </summary>
	/// <param name="dirName"></param>
	/// <param name="strErrorInfo"></param>
	/// <param name="absDirName">如果需要判断路径是否存在，可以传入这个</param>
	/// <returns></returns>
	public static bool IsValidDirName(string dirName, ref string strErrorInfo, string absDirName = null)
	{
		//PS:当名字长度为0时不返回false，
		if (dirName.Length == 0)
		{
			strErrorInfo = "The value is empty!";
			return false;
		}

		//#1 判断文件名是否包含无效字符
		//https://stackoverflow.com/questions/17792883/regex-directory-name-validation
		char[] reserved = Path.GetInvalidFileNameChars();
		foreach (char invalidChar in reserved)
		{
			if (dirName.Contains(invalidChar))
			{
				//提示当前错误字符
				strErrorInfo = $"The value has invalid char: {invalidChar}";
				return false;
			}
		}
		////提示所有错误字符 (Bug: 有些文字无法打印）
		//string strInvalid = "";
		//foreach (var c in Path.GetInvalidPathChars())
		//{
		//    strInvalid += @c + " ";//PS:要处理转义字符
		//}
		//strErrorInfo = $"The name can't contain any of these char: {strInvalid}!\r\nIn this case: {@invalidChar}";
		//Debug.Log(strErrorInfo);

		//其他限制：纯空格开头
		if (dirName.StartsWith(" "))//判断是否为空格
		{
			strErrorInfo = $"The value can't start with empty char!";//只提示当前错误字符
			return false;
		}

		//其他限制：特定名称（https://www.csharp411.com/check-valid-file-path-in-c/）
		string matchInvalidName = listInvalidDirName.Find(ele => ele == dirName);
		if (matchInvalidName.NotNullOrEmpty())
		{
			strErrorInfo = $"The specified value is invalid! Try using other name!";
			return false;
		}

		//#2 检查文件夹是否已经存在
		if (absDirName != null)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(absDirName);
			if (directoryInfo.Exists)
			{
				strErrorInfo = $"The directory already exists!";//只提示当前错误字符
				return false;
			}
		}

		return true;
	}
	#endregion

}
