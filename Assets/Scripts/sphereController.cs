using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 模拟掉落物
/// </summary>
public class sphereController : MonoBehaviour
{
    public int itemId = 0;  // 可拾取物的物品id

    private float outlineTime = 0;  // 描边开启时间
    private Material material;  // 当前物体的材质球

    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        itemId = Random.Range(1, 11);  // 模拟；生成拾取物品的物品id
    }

    // Update is called once per frame
    void Update() {

        // 0.2秒后关闭描边
        if (Time.time - outlineTime > 0.2f) {
            GetComponent<MeshRenderer>().material.SetFloat("_OutlineFactor", 0);
            outlineTime = 0;
        }
    }

    // 设置描边大小
    public void setOutlineFactor(float num) {
        material.SetFloat("_OutlineFactor", num);
        outlineTime = Time.time;
    }
}
