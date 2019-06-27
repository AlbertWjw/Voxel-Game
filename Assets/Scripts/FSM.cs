
using System;
using UnityEngine;
using System.Collections.Generic;


/// <summary>  
/// 状态机中的状态类：以字典的形式保存状态的转换  
/// 1. Reason() 决定触发哪个转换  
/// 2. Act() 决定NPC在当前状态下的行为  
/// </summary>  
public abstract class FSMState {
    protected Dictionary<Transition, StateID> map = new Dictionary<Transition, StateID>();  // 状态转换映射  
    protected StateID stateID;                     // 状态ID  
    public StateID ID { get { return stateID; } }  // 获取当前状态ID  

    /// 添加转换  
    public void AddTransition(Transition trans, StateID id) {
        // 空值检验  
        if (trans == Transition.NullTransition) {
            Debug.LogError("FSMState ERROR: 不能添加空转换");
            return;
        }

        if (id == StateID.NullStateID) {
            Debug.LogError("FSMState ERROR: 不能添加空状态");
            return;
        }

        // 检查是否已经有该转换  
        if (map.ContainsKey(trans)) {
            Debug.LogError("FSMState ERROR: 状态 " + stateID.ToString() + " 已经包含转换 " + trans.ToString() + "不可添加另一个状态");
            return;
        }

        map.Add(trans, id);
    }

    /// 删除状态转换  
    public void DeleteTransition(Transition trans) {
        // 空值检验  
        if (trans == Transition.NullTransition) {
            Debug.LogError("FSMState ERROR: 不能删除空转换");
            return;
        }

        // 检验是否有配对的转换  
        if (map.ContainsKey(trans)) {
            map.Remove(trans);
            return;
        }
        Debug.LogError("FSMState ERROR: 转换 " + trans.ToString() + " - 状态 " + stateID.ToString() + " 不存在");
    }

    /// 获取下一个状态  
    public StateID GetOutputState(Transition trans) {
        // 如果存在转换，返回对应状态  
        if (map.ContainsKey(trans)) {
            return map[trans];
        }
        return StateID.NullStateID;
    }

    /// 进入状态之前执行  
    public virtual void DoBeforeEntering() { }

    /// 离开状态之前执行  
    public virtual void DoBeforeLeaving() { }

    /// 状态转换条件  
    public abstract void Reason(GameObject player, GameObject npc);

    /// 控制行为  
    public abstract void Update(GameObject player, GameObject npc);

}




/// <summary>
/// 为过渡加入枚举标签
/// 不要修改第一个标签，NullTransition会在FSMSytem类中使用
/// </summary>
public enum Transition {
    NullTransition = 0, //用这个过度来代表你的系统中不存在的状态
    SawPlayer,  // 发现玩家
    LostPlayer, // 失去追踪
    ArrivePath,  // 到达巡逻点
    NearPlayer, // 靠近玩家
}

/// <summary>
/// 状态id
/// 为状态加入枚举标签
/// 不要修改第一个标签，NullStateID会在FSMSytem中使用 
/// </summary>
public enum StateID {
    NullStateID = 0, //使用这个ID来代表你系统中不存在的状态ID  
     
    Inspection,  // 巡视
    ChasingPlayer,  // 追逐
    FollowingPath,  // 巡逻
    AttackPlayer,  // 攻击玩家

}


/// <summary>
///状态机类：包含状态列表
/// 该类便是有限状态机类
/// 它持有者NPC的状态集合并且有添加，删除状态的方法，以及改变当前正在执行的状态
/// 1. 删除状态  
/// 2. 改变当前状态  
/// </summary>  
public class FsmSystem {
    private List<FSMState> states;  // 状态列表  

    // 在状态机中改变当前状态的唯一途径是通过转换，当前状态不可直接改变  
    private StateID currentStateID;
    public StateID CurrentStateID { get { return currentStateID; } }
    private FSMState currentState;
    public FSMState CurrentState { get { return currentState; } }



    public FsmSystem() {
        states = new List<FSMState>();
    }

    /// 添加状态  
    public void AddState(FSMState s) {
        // 空值检验  
        if (s == null) {
            Debug.LogError("FSM ERROR: 不可添加空状态");
        }

        // 当所添加状态为初始状态  
        if (states.Count == 0) {
            states.Add(s);
            currentState = s;
            currentStateID = s.ID;
            return;
        }

        // 遍历状态列表，若不存在该状态，则添加  
        foreach (FSMState state in states) {
            if (state.ID == s.ID) {
                Debug.LogError("FSM ERROR: 无法添加状态 " + s.ID.ToString() + " 因为该状态已存在");
                return;
            }
        }
        states.Add(s);
    }

    /// 删除状态  
    public void DeleteState(StateID id) {
        // 空值检验  
        if (id == StateID.NullStateID) {
            Debug.LogError("FSM ERROR: 状态ID 不可为空ID");
            return;
        }

        // 遍历并删除状态  
        foreach (FSMState state in states) {
            if (state.ID == id) {
                states.Remove(state);
                return;
            }
        }
        Debug.LogError("FSM ERROR: 无法删除状态 " + id.ToString() + ". 状态列表中不存在");
    }

    /// 执行转换过渡  
    public void PerformTransition(Transition trans) {
        // 空值检验  
        if (trans == Transition.NullTransition) {
            Debug.LogError("FSM ERROR: 转换不可为空");
            return;
        }

        // 获取当前状态ID  
        StateID id = currentState.GetOutputState(trans);
        if (id == StateID.NullStateID) {
            Debug.LogError("FSM ERROR: 状态 " + currentStateID.ToString() + " 不存在目标状态 " +
                           " - 转换： " + trans.ToString());
            return;
        }

        // 更新当前状态ID 与 当前状态        
        currentStateID = id;
        foreach (FSMState state in states) {
            if (state.ID == currentStateID) {
                // 执行当前状态后处理  
                currentState.DoBeforeLeaving();

                currentState = state;

                // 执行当前状态前处理  
                currentState.DoBeforeEntering();
                break;
            }
        }
    }
}