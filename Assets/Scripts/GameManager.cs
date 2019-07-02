using Net.Proto;
using System.Collections;
using System.Collections.Generic;
using ToolSet;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform PlayerParent = null;
    public GameObject PlayerModel = null;

    public static Dictionary<int, Vector3> playerGoList = new Dictionary<int, Vector3>();  // 局内玩家列表
    public Queue<string[]> moveQueue = new Queue<string[]>();  // 移动消息队列

    void Start()
    {
        PlayerParent = GameObject.Find("PlayerList").transform;
        NetEvent.AddEvent(list, netEventEnum.List);
        NetEvent.AddEvent(leave, netEventEnum.Leave);
        NetEvent.AddEvent(move, netEventEnum.Move);
        GetListProto glp = new GetListProto();
        glp.Timestamp = Tool.GetTimestamp();
        string str = JsonUtility.ToJson(glp);
        NetManager.Instance.Send(netEventEnum.GetList, str);
        

    }

    // 销毁
    private void OnDisable() {
        NetEvent.DelEvent(list, netEventEnum.List);
        NetEvent.DelEvent(leave, netEventEnum.Leave);
        NetEvent.DelEvent(move, netEventEnum.Move);
    }

    // 列表下发处理
    public void list(params object[] objs) {
        string[] args = new string[objs.Length];
        objs.CopyTo(args, 0);
        //string[] str = args[0].Split(';');
        string[] str = args;
        foreach (var item in str) {
            if (item == null || item == "")
                continue;
            string[] i = item.Split(',');
            if (!playerGoList.ContainsKey(int.Parse(i[0]))) {
                playerGoList.Add(int.Parse(i[0]), new Vector3(int.Parse(i[1])/10000, int.Parse(i[2]) / 10000, int.Parse(i[3]) / 10000));
            }
        }

        // 玩家列表下发后刷新
        if (PlayerParent.childCount - 1 < playerGoList.Count) {
            int[] keys = new int[playerGoList.Keys.Count];
            playerGoList.Keys.CopyTo(keys, 0);
            foreach (var i in keys) {
                Transform child = PlayerParent.Find(i.ToString());
                if (child != null) {
                    continue;
                }
                GameObject go = null;
                if (i == NetDispose.id) {
                    go = GameObject.Find("player7"); // todo
                    if(go == null)
                        go = GameObject.Find(i.ToString()); // todo
                } else {
                    go = Instantiate(PlayerModel.transform, PlayerParent).gameObject;
                }
                go.name = i.ToString();
                if (playerGoList.ContainsKey(i) && playerGoList[i] != null) {
                    go.transform.position = playerGoList[i];
                } else {
                    go.transform.position = Vector3.zero;
                }
                go.SetActive(true);
            }
        }

    }

    // 玩家离开
    public void leave(params object[] args) {
        int leaveId = int.Parse(args[0].ToString());
        if (playerGoList.ContainsKey(leaveId)) {
            playerGoList.Remove(leaveId);
        }
        Transform child = PlayerParent.Find(leaveId.ToString());
        if (child != null) {
            Destroy(child.gameObject);
        }
    }
    
    // 移动处理
    public void move(params object[] objs) {
        string[] args = new string[objs.Length];
        objs.CopyTo(args, 0);
        lock (moveQueue) {
            moveQueue.Enqueue(args);
        }

        // 移动处理
        if (moveQueue.Count > 0) {
            string[] item = null;
            lock (moveQueue) {
                item = moveQueue.Peek();
            }
            if (item == null) return;
            if (item.Length == 4) {
                int id = int.Parse(item[0]);
                Vector3 v3 = new Vector3(int.Parse(item[1])/10000, int.Parse(item[2])/10000, int.Parse(item[3])/10000);
                if (playerGoList.ContainsKey(id)) {
                    playerGoList[id] = v3;
                    Transform tr = PlayerParent.Find(id.ToString());
                    if (tr == null) return;
                    tr.position = v3;
                }
            }
            lock (moveQueue) {
                moveQueue.Dequeue();
            }
        }
    }

}
