using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public float vMoveSpeed = 10.0F;  // 前后移动速度
    public float hMoveSpeed = 5.0F;  // 左右移动速度
    public float rotateSpeed = 5.0F;  // 左右旋转速度
    public float jumpHeight = 3;  // 跳跃高度
    public static bool isFollow = true;  // 是否开启相机跟随鼠标
    public int pickupScope = 5;  // 拾取范围
    public GameObject playerBody;  // 要旋转（弯腰）的骨骼

    private Transform thisTransform;  // 当前物体transform组件
    private Animator animator;  // 当前物体的animator组件

    private float moY; // 角色抬枪旋转增加量

    // Start is called before the first frame update
    void Start() {
        Debug.Log("Start");
        thisTransform = this.transform;
        animator = thisTransform.GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;  // 默认设置鼠标隐藏
    }

    // Update is called once per frame
    void Update() {
        // 波浪线~键开启/关闭相机跟随鼠标，隐藏鼠标
        if (Input.GetKeyDown(KeyCode.BackQuote)){
            isFollow = !isFollow;
            if(isFollow)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
        }

        // 鼠标射线检测
        Ray ray;
        RaycastHit hit;
        GameObject obj = null;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            // Debug.Log(hit.collider.gameObject.name);
            obj = hit.collider.gameObject;
            // 射线碰撞的球开启描边
            if (obj.name == "Sphere") {
                obj.GetComponent<sphereController>().setOutlineFactor(0.02f);
            }
        }

        // F键按下拾取物品动画，一定范围内
        if (Input.GetKeyDown(KeyCode.F)) {
            if (obj && obj.name == "Sphere" && distance(obj.transform.position, transform.position) <= pickupScope) {
                animator.SetTrigger("pickup");  // 将人物的动画改为拾取状态
                Item item = new Item(obj.GetComponents<sphereController>()[0].itemId, "物品1", 1);// 拾取到的物品的物品id
                backpack.items.Add(item);  // TODO 在背包中添加背拾取物，之后应该改成同种物体在达到物品上线前合并为一项
                // 销毁可拾取物
                Destroy(obj);
            }

        }
        // WSAD移动动画
        if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.D))) {
            animator.SetBool("walk", true);  // 将人物的动画改为移动状态
        }
        else {
            animator.SetBool("walk", false);  // 取消人物移动动画
        }

        // Q按下/抬起 开/关持枪动画
        if ((Input.GetKeyDown(KeyCode.Q)))
        {
            animator.SetBool("isGun", true);  
        }
        if ((Input.GetKeyUp(KeyCode.Q)))
        {
            animator.SetBool("isGun", false); 
        }
        // 鼠标左键按下开枪动画
        // TODO 在非持枪状态下，不可开枪，之后需要添加修复
        if ((Input.GetKeyDown(KeyCode.Mouse0)))
        {
            animator.SetBool("shoot",true);  // 将人物的动画改为射击状态
        }
        // 鼠标左键抬起时关闭开枪动画
        if ((Input.GetKeyUp(KeyCode.Mouse0)))
        {
            animator.SetBool("shoot", false);  // 将人物的动画改为射击状态
        }

        // 空格键跳跃动画
        // TODO 1、之后需要添加一个角色触碰地面（支撑物）的判断
        //      2、让跳跃动作顺畅
        if ((Input.GetKeyDown(KeyCode.Space)))  
        {
            animator.SetTrigger("jump");  // 将人物的动画改为射击状态
            thisTransform.Translate(0, jumpHeight, 0);
        }

        // 移动
        float vertical = Input.GetAxis("Vertical") * - vMoveSpeed * Time.deltaTime; // 前后
        float horizontal = Input.GetAxis("Horizontal") * - hMoveSpeed * Time.deltaTime; // 左右
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("jump"))
            thisTransform.Translate(horizontal, 0, vertical);
        //transform.Rotate(0, rotation, 0);
        // 角色跟随鼠标左右旋转
        if (isFollow) {
            float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;  // 获得鼠标当前位置的X
            thisTransform.Rotate(0, mouseX, 0);  // 旋转角色
        }

    }

    void LateUpdate () {
        // 角色跟随鼠标上下抬头低头
        // TODO 拾取物品时动画弯腰和视角的弯腰叠加了，之后需要修改
        if (isFollow) {
            float mouseY = Input.GetAxis("Mouse Y");  // 获得鼠标当前位置的Y
            moY += mouseY;
            playerBody.transform.Rotate(new Vector3(moY, 0, 0), Space.Self);
        }
    }

    // 计算两点间的距离
    double distance(Vector3 v1,Vector3 v2) {
        double num=0;
        num = System.Math.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.z - v2.z) * (v1.z - v2.z));
        return num;
    }
}
