using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI控制
/// </summary>
public class UIController : MonoBehaviour
{
    public static UIController _uiController;
    public GameObject mainUI;  // 主UI
    public GameObject backpack;  // 背包页UI
    public GameObject items;  // 背包物品列表UI
    public GameObject itemPrefab;  // 背包物品Prefab

    public static bool isShowSettingUI = false;
    public GameObject settingPrefab;  // 设置UIPrefab
    public GameObject bagPrefab;  // 背包UIPrefab

    public GameObject newWeapon;  // 当前武器
    public Text newWeaponNum;  // 当前武器弹药数量

    public Slider hpSlider;  // 当前hp

    void Start()
    {
        EventManager.AddEvent(UIShow, eventEnum.UIShow);
        EventManager.AddEvent(UIHide, eventEnum.UIHide);

        _uiController = this;

        newWeapon = mainUI.transform.Find("weapons/weapon1").gameObject;
        newWeaponNum = newWeapon.transform.Find("Text").gameObject.GetComponent<Text>();
        hpSlider = mainUI.transform.Find("Panel/hp").gameObject.GetComponent<Slider>();
    }

    void Update() {
        //// Tab键控制tab菜单(背包)打开与关闭
        //if ((Input.GetKeyDown(KeyCode.Tab))) {
        //    mainPlayer.isFollow = !mainUI.activeSelf;
        //    mainUI.SetActive(!mainUI.activeSelf);
        //    backpack.SetActive(!backpack.activeSelf);
        //}
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

    public void UIShow(params string[] handler) {
        GameObject go = null;
        bool isHas = false;
        for(int i = 0; i< transform.childCount; i++) {
            if(transform.GetChild(i) != null) {
                go = transform.GetChild(i).gameObject;
                if (go.name == handler[0]) {
                    isHas = true;
                    go.SetActive(true);
                    isShowSettingUI = true;
                }else {
                    go.SetActive(false);
                }
            }
        }
        if (!isHas) {
            switch (handler[0]) {
                case "SettingUI":
                    go = Instantiate(settingPrefab, transform);
                    break;
                case "BagUI":
                    go = Instantiate(bagPrefab, transform);
                    break;
                default:
                    break;
            }
            go.name = handler[0];
            go.SetActive(true);
            isShowSettingUI = true;
        }
    }

    public void UIHide(params string[] handler) {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i) != null && transform.GetChild(i).gameObject.name == handler[0]) {
                transform.GetChild(i).gameObject.SetActive(false);
                isShowSettingUI = false;
            }
        }
    }

    /// <summary>
    /// 刷新当前武器子弹数量
    /// </summary>
    /// <param name="num"></param>
    public void ShowBulletNum(string num) {
        if (newWeaponNum != null) {
            newWeaponNum.text = num;
        }
    }

    /// <summary>
    /// 刷新玩家hp
    /// </summary>
    public void showPlayerHP(int hp, int maxHp) {
        if(hpSlider)
            hpSlider.value = (float)hp/maxHp;
    }
}
