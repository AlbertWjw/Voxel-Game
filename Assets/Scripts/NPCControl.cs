using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class NPCControl : MonoBehaviour {
    public GameObject player;
    private FsmSystem fsm;
    public Vector3 defaultPoint;  // 默认位置
    public NavMeshAgent navMeshAgent;  // navMeshAgent组件

    public int scope = 10; // 巡逻范围
    public int attackScope = 10; // 攻击范围
    public int attackTime = 2; // 攻击时间间隔
    public float Speed = 10f;

    public void SetTransition(Transition t) {
        //该方法用来改变有限状态机的状体，有限状态机基于当前的状态和通过的过渡状态。
        //如果当前的状态没有用来通过的过度状态，则会抛出错误
        fsm.PerformTransition(t);
    }

    public void Start() {
        MakeFSM();
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

        defaultPoint = transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void FixedUpdate() {
        fsm.CurrentState.Reason(player, gameObject);
        fsm.CurrentState.Update(player, gameObject);
    }

    //建造状态机
    private void MakeFSM()
    {
        InspectionState inspection = new InspectionState();
        inspection.AddTransition(Transition.SawPlayer, StateID.ChasingPlayer);
        inspection.AddTransition(Transition.LostPlayer, StateID.FollowingPath);

        PatrolState follow = new PatrolState(transform.position,scope);
        follow.AddTransition(Transition.SawPlayer, StateID.ChasingPlayer);
        follow.AddTransition(Transition.ArrivePath, StateID.Inspection);

        ChasePlayerState chase = new ChasePlayerState();
        chase.AddTransition(Transition.LostPlayer, StateID.FollowingPath);
        chase.AddTransition(Transition.NearPlayer, StateID.AttackPlayer);

        AttackPlayerState attack = new AttackPlayerState();
        attack.AddTransition(Transition.LostPlayer, StateID.FollowingPath);

        fsm = new FsmSystem();
        fsm.AddState(inspection);//添加状态到状态机，第一个添加的状态将作为初始状态
        fsm.AddState(follow);
        fsm.AddState(chase);
        fsm.AddState(attack);
    }
}



/// <summary>
/// 巡视状态
/// </summary>
public class InspectionState : FSMState {
    public float inspectionTime = 0; // 进入状态时间
    public float changeAnglesTime = 0; // 变更角度时间时间
    public float speed = 5;
    public float angles = 0;
    public float lastAngles = 0;

    //构造函数装填自己
    public InspectionState() {
        stateID = StateID.Inspection;
    }

    public override void DoBeforeEntering() {
        Debug.Log("进入inspectionState状态之前执行--------");
        inspectionTime = Time.realtimeSinceStartup;
        angles = Random.Range(0f,1f);
        Debug.Log("angles:" + angles);
    }

    public override void DoBeforeLeaving() {
        Debug.Log("离开inspectionState状态之前执行 ---------");
    }

    public override void Reason(GameObject player, GameObject npc) {
        RaycastHit hit;
        Debug.DrawRay(npc.transform.position, npc.transform.forward, Color.red);
        if (Vector3.Distance(npc.transform.position, player.transform.position) <= 35f) {
            Debug.Log("与玩家的距离少于35");
            if (Physics.Raycast(npc.transform.position, player.transform.position, out hit, 35F) && hit.transform.gameObject.tag == "Player") {
                Debug.Log("看到玩家 转换状态");
                npc.GetComponent<NPCControl>().SetTransition(Transition.SawPlayer);
            }
        }
        if (Time.realtimeSinceStartup - inspectionTime >= 15f)
            npc.GetComponent<NPCControl>().SetTransition(Transition.LostPlayer);
    }

    public override void Update(GameObject player, GameObject npc) {
        if (Time.realtimeSinceStartup - inspectionTime < 2) return;
        npc.GetComponent<Rigidbody>().velocity = Vector3.zero;

        if (changeAnglesTime == 0 && npc.transform.rotation.y > angles - speed * Time.deltaTime && npc.transform.rotation.y < angles + speed * Time.deltaTime)
            changeAnglesTime = Time.realtimeSinceStartup;
        else {
            Quaternion quaternion = new Quaternion(npc.transform.rotation.x, angles, npc.transform.rotation.z, npc.transform.rotation.w);
            npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, quaternion, speed * Time.deltaTime);
            npc.transform.eulerAngles = new Vector3(0, npc.transform.eulerAngles.y, 0);
        }
        if (changeAnglesTime!= 0 && Time.realtimeSinceStartup - changeAnglesTime > 5) {
            angles = Random.Range(0f, 1f);
            changeAnglesTime = 0;
            Debug.Log("angles:" + angles);
        }


    }

}


/// <summary>
/// 巡逻状态
/// </summary>
public class PatrolState : FSMState {

    public Vector3 nextPoint;
    public NavMeshAgent navMeshAgent = null; // npc的自动巡逻组件
    public float speed = 5;
    public int scope = 0;

    //构造函数装填自己
    public PatrolState(Vector3 v3,int scope) {
        stateID = StateID.FollowingPath;//别忘设置自己的StateID
        nextPoint = v3;
        this.scope = scope;
    }

    public override void DoBeforeEntering() {
        Debug.Log("进入PatrolState状态之前执行--------");
        nextPoint = new Vector3(Random.Range(-scope, scope), 1.5f, Random.Range(-scope, scope));
    }

    public override void DoBeforeLeaving() {
        Debug.Log("离开PatrolState状态之前执行---------");
    }

    //重写动机方法
    public override void Reason(GameObject player, GameObject npc) {
        RaycastHit hit;
        Debug.DrawRay(npc.transform.position, npc.transform.forward, Color.red);
        if (Vector3.Distance(npc.transform.position, player.transform.position) <= 35f) {
            Debug.Log("与玩家的距离少于35");
            if (Physics.Raycast(npc.transform.position, player.transform.position, out hit, 35F) && hit.transform.gameObject.tag == "Player") {
                Debug.Log("看到玩家 转换状态");
                npc.GetComponent<NPCControl>().SetTransition(Transition.SawPlayer);
            }
        }
        Vector3 moveDir = nextPoint - npc.transform.position;
        if (moveDir.magnitude < 1) {
            Debug.Log("到达目标点 转换状态");
            npc.GetComponent<NPCControl>().SetTransition(Transition.ArrivePath);
        }
    }

    //重写表现方法
    public override void Update(GameObject player, GameObject npc) {
        Vector3 moveDir = nextPoint - npc.transform.position;
        if(navMeshAgent == null)
            navMeshAgent = npc.GetComponent<NPCControl>().navMeshAgent;
        navMeshAgent.SetDestination(nextPoint);
        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, Quaternion.LookRotation(moveDir), speed * Time.deltaTime);
    }

}


/// <summary>
/// 追逐状态
/// </summary>
public class ChasePlayerState : FSMState {
    //构造函数装填自己
    public ChasePlayerState() {
        stateID = StateID.ChasingPlayer;
    }

    public override void DoBeforeEntering() {
        Debug.Log("进入ChasePlayerState状态之前执行--------");
    }

    public override void DoBeforeLeaving() {
        Debug.Log("离开ChasePlayerState状态之前执行 ---------");
    }

    public override void Reason(GameObject player, GameObject npc) {
        if (Vector3.Distance(npc.GetComponent<NPCControl>().defaultPoint, player.transform.position) >= 35)
            npc.GetComponent<NPCControl>().SetTransition(Transition.LostPlayer);
        if (Vector3.Distance(npc.GetComponent<NPCControl>().defaultPoint, player.transform.position) <= npc.GetComponent<NPCControl>().attackScope)
            npc.GetComponent<NPCControl>().SetTransition(Transition.NearPlayer);
    }

    public override void Update(GameObject player, GameObject npc) {
        //Vector3 vel = npc.GetComponent<Rigidbody>().velocity;
        Vector3 moveDir = player.transform.position - npc.transform.position;
        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, Quaternion.LookRotation(moveDir), 5 * Time.deltaTime);
        npc.transform.eulerAngles = new Vector3(0, npc.transform.eulerAngles.y, 0);
        //vel = moveDir.normalized * 10;

        //npc.GetComponent<Rigidbody>().velocity = vel;
    }

}


/// <summary>
/// 攻击状态
/// </summary>
public class AttackPlayerState : FSMState {
    public float lastAttackTime = 0; // 上次攻击时间

    //构造函数装填自己
    public AttackPlayerState() {
        stateID = StateID.AttackPlayer;
    }

    public override void DoBeforeEntering() {
        Debug.Log("进入AttackPlayerState状态之前执行--------");
    }

    public override void DoBeforeLeaving() {
        Debug.Log("离开AttackPlayerState状态之前执行 ---------");
    }

    public override void Reason(GameObject player, GameObject npc) {
        if (Vector3.Distance(npc.GetComponent<NPCControl>().defaultPoint, player.transform.position) >= 35)
            npc.GetComponent<NPCControl>().SetTransition(Transition.LostPlayer);
    }

    public override void Update(GameObject player, GameObject npc) {
        npc.transform.LookAt(player.transform.position);
        if ((Time.time - lastAttackTime < npc.GetComponent<NPCControl>().attackTime)) return;
        Ray rayAttack;
        RaycastHit hitAttack;
        GameObject objAttack = null;
        float h = Random.Range(-1, 3);
        float v = Random.Range(-2, 2);
        rayAttack = new Ray(npc.transform.Find("weapon").transform.position, player.transform.position + new Vector3(v, h, 0) - npc.transform.Find("weapon").transform.position);
        Debug.DrawRay(npc.transform.Find("weapon").transform.position, player.transform.position + new Vector3(v, h, 0) - npc.transform.Find("weapon").transform.position, Color.red);
        if (Physics.Raycast(rayAttack, out hitAttack)) {
            objAttack = hitAttack.collider.gameObject;
            if (objAttack.transform.parent && (Time.time - lastAttackTime >= npc.GetComponent<NPCControl>().attackTime) && Vector3.Distance(npc.transform.position, objAttack.transform.parent.position) <= npc.GetComponent<NPCControl>().attackScope) { 
                if (objAttack.transform.parent.tag == "Player") {
                    Debug.Log("attack hurt");
                    // TODO 攻击
                    lastAttackTime = Time.time;
                    objAttack.transform.parent.GetComponent<mainPlayer>().hurt(Random.Range(20, 40));

                }
            }
        }
    }

}
