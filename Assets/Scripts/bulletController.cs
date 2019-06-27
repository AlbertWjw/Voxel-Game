using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletController : MonoBehaviour
{
    public int speed = 15;  // 子弹速度

    private Vector3 startPo; // 起始位置
    private bool isTrigger = false;  // 是否已触发

    // Start is called before the first frame update
    void Start()
    {
        startPo = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (distance(startPo, transform.position)<100 && !isTrigger) {
            transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
        }
    }

    // 进入触发器
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log(other.name);
        isTrigger = true;
    }

    // 计算两点间的距离
    double distance(Vector3 v1, Vector3 v2)
    {
        double num = 0;
        num = System.Math.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.z - v2.z) * (v1.z - v2.z));
        return num;
    }
}
