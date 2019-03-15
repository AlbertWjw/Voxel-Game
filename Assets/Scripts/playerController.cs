using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public float vMoveSpeed = 10.0F;  // 前后移动速度
    public float hMoveSpeed = 5.0F;  // 左右移动速度
    public float rotateSpeed = 5.0F;  // 左右旋转速度
    public static bool isFollow = true;  // 是否开启相机跟随鼠标

    private Transform thisTransform;  // 当前物体transform组件

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        thisTransform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // 开启关闭相机跟随鼠标
        if (Input.GetKeyDown(KeyCode.BackQuote)){
            isFollow = !isFollow;
            if(isFollow)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
            Debug.Log(isFollow);
        }

        // 鼠标射线检测
        Ray ray;
        RaycastHit hit;
        GameObject obj;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            Debug.DrawRay(transform.position, hit.point);
            Debug.Log(hit.collider.gameObject.name);
            obj = hit.collider.gameObject;
            // 射线碰撞的球开启描边
            if (hit.collider.gameObject.name == "Sphere") {
                obj.GetComponent<sphereController>().setOutlineFactor(0.02f);
            }
        }


        // 移动时改变动画
        if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.D))) {
            thisTransform.GetComponent<Animator>().SetBool("walk", true);  // 将人物的动画改为移动状态
        }
        else {
            thisTransform.GetComponent<Animator>().SetBool("walk", false);  // 取消人物移动动画
        }
        // 鼠标左键点击时改变为开枪动画
        if ((Input.GetKeyDown(KeyCode.Mouse0)))
        {
            thisTransform.GetComponent<Animator>().SetBool("shoot",true);  // 将人物的动画改为射击状态
        }
        // 鼠标左键抬起时关闭开枪动画
        if ((Input.GetKeyUp(KeyCode.Mouse0)))
        {
            thisTransform.GetComponent<Animator>().SetBool("shoot", false);  // 将人物的动画改为射击状态
        }

        // 移动
        float vertical = Input.GetAxis("Vertical") * - vMoveSpeed * Time.deltaTime; // 前后
        float horizontal = Input.GetAxis("Horizontal") * - hMoveSpeed * Time.deltaTime; // 左右
        thisTransform.Translate(horizontal, 0, vertical);
        //transform.Rotate(0, rotation, 0);
        // 角色跟随鼠标左右旋转
        if (isFollow) {
            float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;  // 获得鼠标当前位置的X
            thisTransform.Rotate(0, mouseX, 0);  // 旋转角色
        }

    }
}
