using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    public float moveSpeed = 10.0F;  // 相机移动速度
    public Vector2 rotateScope = new Vector2(0,0);  // 相机上下旋转范围

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        //// 摄像机跟随鼠标上下旋转
        //if (playerController.isFollow) {
        //    float mouseY = Input.GetAxis("Mouse Y") * moveSpeed;  // 获得鼠标当前位置的Y
        //    Camera.main.transform.localRotation *= Quaternion.Euler(-mouseY, 0, 0);// 鼠标在Y轴上的移动号转为摄像机的上下运动，即是绕着X轴反向旋转
        //}
    }

}
