﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunController : MonoBehaviour
{

    public GameObject bullet;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 鼠标左键按下开枪动画
        if ((Input.GetKeyDown(KeyCode.Mouse0)))
        {
            Instantiate(bullet, transform.Find("muzzle").position, transform.Find("muzzle").rotation);
        }
    }
}