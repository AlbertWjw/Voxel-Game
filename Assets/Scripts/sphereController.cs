using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sphereController : MonoBehaviour
{
    private float outlineTime = 0;
    private Material material;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
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
