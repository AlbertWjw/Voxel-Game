using Net.Proto;
using System.Collections;
using System.Collections.Generic;
using ToolSet;
using UnityEngine;

public class mainPlayer : MonoBehaviour
{
    public UIController gameUI;  // 游戏UI脚本
    public backpack playerBackpack;  // 游戏UI脚本
    public GameObject hurtParticle;  // 受到伤害出血效果

    public int maxHP = 100;  // 最大hp
    public int hp = 100;  // 当前hp

    public int mv_speed = 3;  //前后移动速度
    public int mh_speed = 3;  //左右移动速度
    public int r_speed = 3;  //左右旋转速度
    public float jumpHeight = 3;  // 跳跃高度
    public int pickupScope = 5;  // 拾取范围
    public GameObject playerBody;  // 要旋转（弯腰）的骨骼

    private Transform thisTransform;  // 当前物体transform组件
    private Animator animator;  // 当前物体的animator组件
    private BoxCollider trigger;  // 当前物体的脚踩触发器组件

    private float moY; // 角色抬枪旋转增加量

    public bool isTrigger  = true;  // 是否触发脚踩触发器
    private bool isUpdateMove = false;  // 是否移动了

    // Start is called before the first frame update
    void Start() {
        thisTransform = this.transform;
        animator = thisTransform.GetComponent<Animator>();
        trigger = thisTransform.GetComponent<BoxCollider>();

        EventManager.AddEvent(move, eventEnum.move);
        EventManager.AddEvent(Rotate, eventEnum.Rotate);
        EventManager.AddEvent(Jump, eventEnum.Jump);
        EventManager.AddEvent(Pickup, eventEnum.Pickup);
    }

    // Update is called once per frame
    void Update() {

        //// 鼠标射线检测
        //Ray ray;
        //RaycastHit hit;
        //GameObject obj = null;
        //ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out hit)) {
        //    // Debug.Log(hit.collider.gameObject.name);
        //    obj = hit.collider.gameObject;
        //    // 射线碰撞的球开启描边
        //    if (obj.name == "Sphere") {
        //        obj.GetComponent<sphereController>().setOutlineFactor(0.02f);
        //    }
        //}

        //// F键按下拾取物品动画，一定范围内
        //if (Input.GetKeyDown(KeyCode.F)) {
        //    if (obj && obj.name == "Sphere" && distance(obj.transform.position, transform.position) <= pickupScope) {
        //        animator.SetTrigger("pickup");  // 将人物的动画改为拾取状态
        //        Item item = new Item(obj.GetComponents<sphereController>()[0].itemId, "物品"+ obj.GetComponents<sphereController>()[0].itemId, 1);//TODO 拾取到的物品的物品id
        //        playerBackpack.addItem(item);  // 在背包中添加背拾取物
        //        bool isTrue = gameUI.updateBackpack(item);
        //        // 销毁可拾取物
        //        if(isTrue)
        //            Destroy(obj);
        //    }

        //}

        // WSAD移动动画
        if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.D))) {
            animator.SetBool("walk", true);  // 将人物的动画改为移动状态
        }
        else {
            animator.SetBool("walk", false);  // 取消人物移动动画
        }

        // TODO临时 Q按下/抬起 开/关持枪动画
        if ((Input.GetKeyDown(KeyCode.Q)))
        {
            animator.SetBool("isGun", true);  
        }
        if ((Input.GetKeyUp(KeyCode.Q)))
        {
            animator.SetBool("isGun", false); 
        }
        // 鼠标左键按下开枪动画
        if ((Input.GetKeyDown(KeyCode.Mouse0)) && animator.GetBool("isGun"))
        {
            animator.SetBool("shoot",true);  // 将人物的动画改为射击状态
        }
        // 鼠标左键抬起时关闭开枪动画
        if ((Input.GetKeyUp(KeyCode.Mouse0)))
        {
            animator.SetBool("shoot", false);  // 将人物的动画改为射击状态
        }

    }

    private void FixedUpdate() {
        if (isUpdateMove) {
            
            MoveProto mp = new MoveProto();
            mp.id = NetDispose.id;
            mp.Timestamp = Tool.GetTimestamp();
            mp.x = (int)(transform.position.x * 10000);
            mp.y = (int)(transform.position.y * 10000);
            mp.z = (int)(transform.position.z * 10000);
            isUpdateMove = false;
            string str = JsonUtility.ToJson(mp);
            NetManager.Instance.Send(netEventEnum.Move, str);
        }
    }

    void LateUpdate () {
        // 角色跟随鼠标上下抬头低头
        // TODO 拾取物品时动画弯腰和视角的弯腰叠加了，之后需要修改
        if (OperationManager.isFollow) {
            float mouseY = Input.GetAxis("Mouse Y");  // 获得鼠标当前位置的Y
            moY += mouseY;
            playerBody.transform.Rotate(new Vector3(moY, 0, 0), Space.Self);
        }
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="handler">移动位置:0-horizontal,1-vertical</param>
    public void move(params string[] handler)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("jump")) return;
        Vector3 v3 = new Vector3(-float.Parse(handler[0]) * mh_speed * Time.deltaTime, 0, -float.Parse(handler[1]) * mv_speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, v3) > 1) {
            isUpdateMove = true;
        }
        transform.Translate(v3);
         
        animator.SetTrigger("walk");  // 将人物的动画改为移动状态
    }

    /// <summary>
    /// 旋转
    /// </summary>
    /// <param name="handler">y轴旋转角度</param>
    public void Rotate(params string[] handler) {
        transform.Rotate(0, float.Parse(handler[0])*r_speed, 0);  // 旋转角色
    }

    /// <summary>
    /// 跳跃
    /// </summary>
    /// <param name="handler">null</param>
    public void Jump(params string[] handler) {
        if (!isTrigger) return;
        animator.SetTrigger("jump");  // 将人物的动画改为跳跃状态
        gameObject.GetComponent<Rigidbody>().velocity += Vector3.up * jumpHeight;
    }

    /// <summary>
    /// 拾取物品，一定范围内
    /// </summary>
    /// <param name="handler"></param>
    public void Pickup(params string[] handler) {

        // 鼠标射线检测
        Ray ray;
        RaycastHit hit;
        GameObject obj = null;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            // Debug.Log(hit.collider.gameObject.name);
            obj = hit.collider.gameObject;
            // 射线碰撞的球开启描边
            if (obj.name == "Sphere")
            {
                obj.GetComponent<sphereController>().setOutlineFactor(0.02f);
            }
        }

        if (obj && obj.name == "Sphere" && distance(obj.transform.position, transform.position) <= pickupScope)
        {
            animator.SetTrigger("pickup");  // 将人物的动画改为拾取状态
            Item item = new Item(obj.GetComponents<sphereController>()[0].itemId, "物品" + obj.GetComponents<sphereController>()[0].itemId, 1);//TODO 拾取到的物品的物品id
            playerBackpack.addItem(item);  // 在背包中添加背拾取物
            bool isTrue = gameUI.updateBackpack(item);
            // 销毁可拾取物
            if (isTrue)
                Destroy(obj);
        }
    }

    // 当进入触发器
    void OnTriggerEnter(Collider collider) {
        isTrigger = true;
    }
    private void OnTriggerStay(Collider other) {
        isTrigger = true;
    }
    // 当退出触发器
    void OnTriggerExit(Collider collider) {
        isTrigger = false;
    }

    /// <summary>
    /// 计算两点间的直线距离
    /// </summary>
    /// <param name="v1">起始点</param>
    /// <param name="v2">结束点</param>
    /// <returns>距离</returns>
    private double distance(Vector3 v1,Vector3 v2) {
        double num=0;
        num = System.Math.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.z - v2.z) * (v1.z - v2.z));
        return num;
    }

    // TODO 伤害扣除
    public void hurt(int num)
    {
        GameObject go = Instantiate(hurtParticle, transform.position, transform.rotation);
        StartCoroutine(DestroyEffect(go));
        hp -= num;
        UIController._uiController.showPlayerHP(hp, maxHP);
        if (hp <= 0)
        {
            Debug.Log("Player Dead");
        }
    }

    IEnumerator DestroyEffect(GameObject go) {
        yield return new WaitForSeconds(2);
        Destroy(go);
    }
}
