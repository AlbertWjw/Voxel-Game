using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class gunController : MonoBehaviour
{

    public GameObject bullet;

    public GameObject firt;  // 枪口火焰

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 鼠标左键按下开枪动画
        if ((Input.GetKeyDown(KeyCode.Mouse0)) && !EventSystem.current.IsPointerOverGameObject())
        {
            // Instantiate(bullet, transform.Find("muzzle").position, transform.Find("muzzle").rotation);
            // 鼠标射线检测
            Ray ray;
            RaycastHit hit;
            GameObject obj = null;
            Vector2 v = Input.mousePosition;
            if(!playerController.isFollow)
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
        }
    }
}
