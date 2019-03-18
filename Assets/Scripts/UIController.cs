using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// UI控制
/// </summary>
public class UIController : MonoBehaviour
{
    public GameObject backpack;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Tab键控制tab菜单(背包)打开与关闭
        if ((Input.GetKeyDown(KeyCode.Tab)))
        {
            backpack.SetActive(!backpack.activeSelf);
        }
    }
}
