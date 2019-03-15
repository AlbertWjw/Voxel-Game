using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 临时场景控制脚本
/// </summary>
public class sceneManager : MonoBehaviour
{
    public Material material;
    // Start is called before the first frame update
    void Start()
    {
        // 随机掩体生成
        int random = Random.Range(0, 100);
        for (int i = 0; i < random; i++) {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.parent = this.transform;
            obj.transform.localScale = new Vector3(2, 2, 2);
            obj.transform.position = new Vector3(Random.Range(-100, 100), 1, Random.Range(-100, 100));
            obj.gameObject.AddComponent<BoxCollider>();
        }

        // 随机可拾取物体代替球生成
        int ball = Random.Range(100, 1000);
        for (int i = 0; i < ball; i++)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.parent = this.transform;
            obj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            obj.GetComponent<MeshRenderer>().material = material;
            obj.GetComponent<MeshRenderer>().material.SetColor("_Diffuse",new Color(Random.Range(0,2), Random.Range(0, 2), Random.Range(0, 2), 1));
            obj.GetComponent<MeshRenderer>().material.SetFloat("_OutlineFactor",0);
            obj.transform.position = new Vector3(Random.Range(-100, 100), 0.15f, Random.Range(-100, 100));
            obj.gameObject.AddComponent<SphereCollider>();
            obj.gameObject.AddComponent<sphereController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
