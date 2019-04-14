using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public static bool isFollowMouse = true;  // 相机是否默认跟随鼠标

    //按键
    /// <summary>
    /// 设置界面
    /// </summary>
    public static KeyCode SettingUI = KeyCode.Escape;
    /// <summary>
    /// 跳跃
    /// </summary>
    public static KeyCode Jump = KeyCode.Space;
    /// <summary>
    /// 拾取
    /// </summary>
    public static KeyCode Pickup = KeyCode.F;
    /// <summary>
    /// 开火
    /// </summary>
    public static KeyCode Fire = KeyCode.Mouse0;
    /// <summary>
    /// 换弹
    /// </summary>
    public static KeyCode ChangeMagazine = KeyCode.R;
    /// <summary>
    /// 背包打开/关闭
    /// </summary>
    public static KeyCode Bag = KeyCode.Tab;

    void Start() {
        // 默认设置鼠标隐藏
        if(isFollowMouse) Cursor.lockState = CursorLockMode.Locked;  
    }
}
