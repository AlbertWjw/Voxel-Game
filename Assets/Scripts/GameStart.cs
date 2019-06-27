using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    public bool isHotUpdate = true;
    public Text stateTextUI;

    void Start() {
        stateTextUI = GameObject.Find("StateText").GetComponent<Text>();
        // 检查存档
        if (Directory.Exists(Config.Instance.localInfoPath)) {
            if (Directory.GetFiles(Config.Instance.localInfoPath).Length > 0) {
                stateTextUI.text = "加载配置中......";
                LoadWithPLayerInfo(Config.Instance.localInfoPath);
            }
        }

        // 检查更新
        if (isHotUpdate) {
            string[] lastVersion = GetLastVersion().Split(',');
            int[] versionNumber = Config.Instance.localVersionNumber;
            if (int.Parse(lastVersion[0]) > versionNumber[0] || int.Parse(lastVersion[1]) > versionNumber[1]) {
                print("请更新客户端");
                stateTextUI.text = "请更新客户端";
            } else if (int.Parse(lastVersion[2]) > versionNumber[2]) {
                stateTextUI.text = "需要热更新";
                // todo
                StartCoroutine(UpDateAsset(loadNextScene));  // 热更新
            } else {
                print("不需要更新");
                stateTextUI.text = "不需要更新";
                loadNextScene();
            }
        }

    }

    /// <summary>
    /// 获取最新版本号
    /// </summary>
    public string GetLastVersion() {
        UnityWebRequest lastVersionRequest = UnityWebRequest.Get(Config.Instance.versionServerPath);
        if (lastVersionRequest.isHttpError || lastVersionRequest.isNetworkError) {
            Debug.Log(lastVersionRequest.error);
        }
        string lastVersion = lastVersionRequest.downloadHandler.text;
        if (string.IsNullOrEmpty(lastVersion)) {
            lastVersion = "0,0,0";
        }
        return lastVersion;
    }

    /// <summary>
    /// 加载存档
    /// </summary>
    /// <param name="path"></param>
    public void LoadWithPLayerInfo(string path) {
        foreach (string fileName in Config.Instance.localInfoFiles) {
            print(fileName);
            if (File.Exists(path + "/" + fileName)) {
                string str = File.ReadAllText(path + "/" + fileName);
                switch (fileName) {
                    case "UserData.cfg":
                        Config.Instance.defaultUserName = str;
                        break;
                    case "Version.cfg":
                        string[] ver = str.Split(',');
                        int[] verInt = new int[3];
                        for (int i = 0; i < 3; i++) {
                            verInt[i] = int.Parse(ver[i]);
                        }
                        Config.Instance.localVersionNumber = verInt;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void loadNextScene() {
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// 更新资源
    /// </summary>
    IEnumerator UpDateAsset(Action callBack) {
        UnityWebRequest filesRequest = UnityWebRequest.Get(@"http://localhost/filesRequest");
        yield return filesRequest.SendWebRequest();
        if (filesRequest.isHttpError || filesRequest.isNetworkError) {
            Debug.Log(filesRequest.error);
            yield break;
        }
        print(filesRequest.downloadHandler.text);
        string[] filesName = filesRequest.downloadHandler.text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        int finishNum = 0;
        Action DownloadAssetFinish = new Action(() => {
            print("下载完成回调");
            finishNum++;
            if (finishNum >= filesName.Length) {
                callBack();
            }
        });  // todo 下载完成回调 下载失败的
        foreach (var i in filesName) {
            if (string.IsNullOrEmpty(i)) break;
            StartCoroutine(DownloadAsset(i, DownloadAssetFinish));  // 下载对应文件
        }
    }

    /// <summary>
    /// 下载更新的文件
    /// </summary>
    /// <param name="uri">文件路径</param>
    IEnumerator DownloadAsset(string uri, Action func) {
        UnityWebRequest fileRequest = UnityWebRequest.Get(@"http://localhost/filesRequest/" + uri);
        yield return fileRequest.SendWebRequest();
        if (fileRequest.isHttpError || fileRequest.isNetworkError) {
            Debug.Log(fileRequest.error);
            StartCoroutine(DownloadAsset(uri, func));
            //failFiles.Add(uri);
            yield break;
        }
        File.WriteAllText(@"Assets/" + uri, fileRequest.downloadHandler.text);
        func();  // 下载完成回调
    }
}
