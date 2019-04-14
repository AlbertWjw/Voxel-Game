using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationManager : MonoBehaviour
{
    public static bool isFollow;  // 是否开启相机跟随鼠标

    void Start() {
        isFollow = SettingManager.isFollowMouse;
    }

    void Update() {

        // 波浪线~键开启/关闭相机跟随鼠标、隐藏鼠标
        if (Input.GetKeyDown(KeyCode.BackQuote)) {
            isFollow = !isFollow;
        }
        if (isFollow) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 设置ui
        if (Input.GetKeyDown(SettingManager.SettingUI)) {
            if(UIController.isShowSettingUI)
                EventManager.SendEvent(eventEnum.UIHide, "SettingUI");
            else
                EventManager.SendEvent(eventEnum.UIShow,"SettingUI");
        }
        // 背包打开与关闭
        if (Input.GetKeyDown(SettingManager.Bag)) {
            if (UIController.isShowSettingUI)
                EventManager.SendEvent(eventEnum.UIHide, "BagUI");
            else
                EventManager.SendEvent(eventEnum.UIShow, "BagUI");
        }

        // 移动
        float horizontal = Input.GetAxis("Horizontal"); //A D 左右
        float vertical = Input.GetAxis("Vertical"); //W S 前 后
        if (horizontal != 0 || vertical != 0) {
            EventManager.SendEvent(eventEnum.move, horizontal.ToString(), vertical.ToString());
        }

        // 角色跟随鼠标左右旋转
        if (isFollow) { 
            float mouseX = Input.GetAxis("Mouse X");  // 获得鼠标当前位置的X
            EventManager.SendEvent(eventEnum.Rotate, mouseX.ToString());
        }

        // 跳跃
        if (Input.GetKeyDown(SettingManager.Jump)) {
            EventManager.SendEvent(eventEnum.Jump);
        }

        // 拾取
        if (Input.GetKeyDown(SettingManager.Pickup)) {
            EventManager.SendEvent(eventEnum.Pickup);
        }

        // 开火
        if (Input.GetKey(SettingManager.Fire)) {
            EventManager.SendEvent(eventEnum.Fire);
        }

        // 更换弹夹
        if (Input.GetKey(SettingManager.ChangeMagazine))
        {
            EventManager.SendEvent(eventEnum.ChangeMagazine);
        }
    }
}
