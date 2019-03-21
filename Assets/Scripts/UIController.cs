using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI控制
/// </summary>
public class UIController : MonoBehaviour
{
    public GameObject mainUI;  // 主UI
    public GameObject backpack;  // 背包页UI
    public GameObject items;  // 背包物品列表UI
    public GameObject itemPrefab;  // 背包物品Prefab

    void Start()
    {
        
    }

    void Update() {
        // Tab键控制tab菜单(背包)打开与关闭
        if ((Input.GetKeyDown(KeyCode.Tab))) {
            playerController.isFollow = !mainUI.activeSelf;
            mainUI.SetActive(!mainUI.activeSelf);
            backpack.SetActive(!backpack.activeSelf);
        }
    }

    // 刷新背包物品列表
    public bool updateBackpack(Item item) {
        foreach (Transform childTransform in items.transform) {
            if (int.Parse(childTransform.Find("id").GetComponent<Text>().text) == item.ID) {
                string count = childTransform.Find("count").GetComponent<Text>().text;
                childTransform.Find("count").GetComponent<Text>().text = int.Parse(count) + 1+"";
                return true;
            }
        }
        GameObject go =  Instantiate(itemPrefab, items.transform);
        go.transform.Find("name").GetComponent<Text>().text = item.Title;
        go.transform.Find("id").GetComponent<Text>().text = item.ID+"";
        // TODO 之后还需要为拾取的物体添加图标
        return true;
    }

}
