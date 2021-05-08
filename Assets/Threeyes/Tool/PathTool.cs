using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public static class PathTool
{
    /// <summary>
    /// 转换为File可以识别的格式(用\分隔)
    /// </summary>
    /// <param name="rawPath"></param>
    /// <returns></returns>
    public static string ConvertToSystemFormat(string rawPath)
    {
        return rawPath.Replace("/", @"\");
    }
    public static string GetOrCreateDir(string dirPath)
    {
        try
        {
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
        }
        catch (Exception e)
        {
            Debug.LogError("CreateDirectory Failed!\r\n" + e);
        }
        return dirPath;
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
    /// 打开文件所在位置(PS:；路径只能是\\）
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="isSelect"></param>
    public static void OpenFile(string filePath, bool isSelect = true)
    {
        string cmd = "explorer.exe";
        string arg = filePath;
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

}
