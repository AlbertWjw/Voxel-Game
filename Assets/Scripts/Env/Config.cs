using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{
    public static Config Instance = new Config();  // 单例

    public string localInfoPath = Application.dataPath +"/GameInfo";  // 本地存档文件夹路径
    public string[] localInfoFiles = {
        "UserData.cfg",
        "Version.cfg",
    };

    public string versionServerPath = @"http://localhost/lastVersion";  // 最新版本号获取地址

    public string defaultUserName = "";  // 默认用户名

    public int[] localVersionNumber = new int[3] { 0, 1, 0 }; // 本地版本号

}
