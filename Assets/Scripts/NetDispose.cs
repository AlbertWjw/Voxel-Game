using Net.Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using ToolSet;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///网络操作类
/// </summary>
public class NetDispose : MonoBehaviour
{
    NetManager net = null;

    public Transform PlayerParent = null;
    public Transform cube = null;

    public static Dictionary<int,Vector3> playerGoList = new Dictionary<int,Vector3>();
    public List<Action> moveList = new List<Action>();
    public Queue<string[]> moveQueue = new Queue<string[]>();

    public static int id = 0;

    void Start()
    {
        DontDestroyOnLoad(gameObject); //过场景不销毁
        // 网络
        net = NetManager.Instance;
        //net.pingPongTime = 3;
        net.Connect("127.0.0.1", 9003);
        //PlayerParent = GameObject.Find("PlayerList").transform;
        //cube = cubeParent.Find("item");
        NetEvent.AddEvent(enter, netEventEnum.Enter);
        //NetEvent.AddEvent(leave, netEventEnum.Leave);
        //NetEvent.AddEvent(move, netEventEnum.Move);
        //NetEvent.AddEvent(list, netEventEnum.List);

    }

    void Update() {
        if (net != null) {
            NetManager.Update();
        }
        //// 临时按键控制
        //if (Input.GetKeyDown(KeyCode.X)) {
        //    net.Close();
        //}
        //if (Input.GetKeyDown(KeyCode.A)) {
        //    var mp = new MoveProto();
        //    System.Random ran = new System.Random();
        //    mp.x = ran.Next(-10,10);
        //    mp.id = id;
        //    mp.Timestamp = Tool.GetTimestamp();
        //    string str = JsonUtility.ToJson(mp);
        //    net.Send(netEventEnum.Move,str);
        //}
    }

    // 销毁
    private void OnDisable() {
        NetEvent.DelEvent(enter, netEventEnum.Enter);
        //NetEvent.DelEvent(leave, netEventEnum.Leave);
        //NetEvent.DelEvent(move, netEventEnum.Move);
        //NetEvent.DelEvent(list, netEventEnum.List);
        net.Close();
    }

    // 连接完成处理
    public void enter(params object[] args) {
        id = int.Parse(args[0].ToString());
        if (SceneManager.GetActiveScene().name == "GameStart") {
            SceneManager.LoadScene("Main");
        } else if (SceneManager.GetActiveScene().name == "Game") {
            GameStartProto gsp = new GameStartProto();
            gsp.id = NetDispose.id;
            string str = JsonUtility.ToJson(gsp);
            NetManager.Instance.Send(netEventEnum.GameStart, str);
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
            if (item.Count() == 4) {
                int id = int.Parse(item[0]);
                Vector3 v3 = new Vector3(float.Parse(item[1]), float.Parse(item[2]), float.Parse(item[3]));
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
