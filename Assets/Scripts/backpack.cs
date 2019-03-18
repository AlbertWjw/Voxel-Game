using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 玩家背包
/// </summary>
public class backpack : MonoBehaviour
{
    public static int[][] weapon;

    public static List<Item> items;

    // Start is called before the first frame update
    void Start()
    {
        items = new List<Item> { };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

/// <summary>
/// 物品类
/// </summary>
public class Item {
    public int ID { get; set; }
    public string Title { get; set; }
    public int Count { get; set; }

    public Item(int id, string title, int count) {
        this.ID = id;
        this.Title = title;
        this.Count = count;
    }
}
