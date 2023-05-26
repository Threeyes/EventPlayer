using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 数据类读写配置（适用于数据类自行备份）
/// </summary>
public interface ISaveLoadConfig
{
    void SaveConfig();
    void LoadConfig();
}
