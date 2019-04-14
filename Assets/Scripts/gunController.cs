using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GunController : MonoBehaviour
{

    public GameObject bullet;

    public GameObject firt;  // 枪口火焰

    public float speed = 1/10;  // 射速 发/分
    private float lastTime = 0;  // 上次开枪时间

    public int bulletNum = 30;  // 单价子弹总量
    public int num = 30;  // 子弹数量

    // Start is called before the first frame update
    void Start()
    {
        num = bulletNum;
        EventManager.AddEvent(ChangeMagazine, eventEnum.ChangeMagazine);
    }

    // Update is called once per frame
    void Update()
    {
        // 鼠标左键按下开枪动画
        if ((Input.GetKey(KeyCode.Mouse0)) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (num <= 0) return;
            if (Time.time - lastTime < speed) return;
            // Instantiate(bullet, transform.Find("muzzle").position, transform.Find("muzzle").rotation);
            // 鼠标射线检测
            Ray ray;
            RaycastHit hit;
            GameObject obj = null;
            Vector2 v = Input.mousePosition;
            if(!mainPlayer.isFollow)
                v = new Vector2(Screen.width / 2, Screen.height / 2); //屏幕中心点
            ray = Camera.main.ScreenPointToRay(v);
            if (Physics.Raycast(ray, out hit))
            {
                // Debug.Log(hit.collider.gameObject.name);
                obj = hit.collider.gameObject;
                GameObject go = Instantiate(firt,transform.Find("muzzle").transform.position, transform.Find("muzzle").transform.rotation);
                Debug.Log(obj.name);
                if (obj.tag == "enemy") {
                    // TODO 伤害计算
                    obj.GetComponent<enemyController>().hurt(40);
                }
            }
            num--;
            lastTime = Time.time;
        }

    }

    /// <summary>
    /// 换弹
    /// </summary>
    /// <param name="handler"></param>
    public void ChangeMagazine(params string[] handler) {
        // TODO
        num = bulletNum;
    }
}
