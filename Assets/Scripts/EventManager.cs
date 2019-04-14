using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件枚举
/// </summary>
public enum eventEnum {

    // 角色控制
    move = 1,
    Rotate = 2,  // 旋转角色
    Jump = 3,
    Pickup = 4,
    Fire = 5,
    ChangeMagazine = 6,

    // UI
    UIShow = 10,
    UIHide= 11,

    max = 12,  // event总数，添加enum时需要一同修改
}

/// <summary>
/// 事件管理
/// </summary>
public class EventManager: MonoBehaviour
{
    public delegate void Operation(params string[] handler);
    private static List<Operation>[] operation;

    //构造函数
    public void Start() {
        operation = new List<Operation>[(int)eventEnum.max];
    }

    /// <summary>
    /// 事件订阅
    /// </summary>
    public static void AddEvent(Operation handler, params eventEnum[] events) {
        for (int i = 0; i < events.Length; i++) {
            //Debug.Log("事件订阅  "+i);
            int key = (int)events[i];
            if (i > operation.Length) continue;
            if (operation[key] == null) {
                operation[key] = new List<Operation>();
            }
            operation[key].Add(handler);
        }
    }

    /// <summary>
    /// 事件退订
    /// </summary>
    public static void DelEvent(Operation handler, eventEnum e) {
        //Debug.Log("事件退订  "+(int)e);
        if ((int)e > operation.Length || operation[(int)e] == null) return;
        operation[(int)e].Remove(handler);
    }

    /// <summary>
    /// 事件派发
    /// </summary>
    public static void SendEvent(eventEnum eventId, params string[] obj) {
        //Debug.Log("事件派发  "+(int)eventId);
        if (operation == null || operation[(int)eventId] == null) return;
        for (int i = 0; i < operation[(int)eventId].Count; i++) {
            operation[(int)eventId][i](obj);
        }
    }
}
