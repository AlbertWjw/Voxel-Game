using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 玩家背包
/// </summary>
public class backpack : MonoBehaviour
{
    public static Weapon[] weapons;

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

    public void addItem(Item item) {
        Item last = null;
        items.Find(i => {
            if (i.ID == item.ID) {
                last = i;
                items.Remove(i);
            }
            return true;
                
        });
        if (last != null)
        {
            last.Count++;
        }
        else {
            last = item;
        }
        items.Add(last);
        foreach (Item i in items) {
            Debug.Log(i.ID + "," + i.Title + "," + i.Count);
        }
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

/// <summary>
/// 武器类
/// </summary>
public class Weapon : Item {
    public Weapon(int id, string title, int count):base(id,title,count) {

    }
}
