using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyController : MonoBehaviour
{
    public GameObject player;  // 玩家位置

    public int maxHP = 100;  // 最大hp
    public int hp = 100;  // 当前hp
    public int moveTime = 10; // 移动时间间隔
    public int rotateTime = 5; // 转向时间间隔
    public int attackTime = 2; // 攻击时间间隔

    public GameObject firt;  // 枪口火焰
    public GameObject hurtParticle;  // 受到伤害出血效果
    public GameObject deadParticle;  // 死亡效果

    private NavMeshAgent navMeshAgent;  // navMeshAgent组件
    private Transform weapon;  // 武器位置

    private float lastMoveTime = 0; // 上次移动时间
    private float lastRotateTime = 0; // 上次转向时间
    public float lastAttackTime = 0; // 上次攻击时间

    void Start()
    {
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        weapon = transform.Find("weapon").transform;
    }

    void Update()
    {

        // 鼠标射线检测
        Ray ray;
        RaycastHit hit;
        GameObject obj = null;
        ray = new Ray(weapon.position, player.transform.position - weapon.position);
        Debug.DrawRay(weapon.position, player.transform.position - weapon.position, Color.red);
        if (Physics.Raycast(ray, out hit))
        {
            // Debug.Log(hit.collider.gameObject.name);
            obj = hit.collider.gameObject;

            // 没找到玩家或者超出范围，随机移动
            if (obj.name != "player" || (obj.name == "player" && distance(transform.position, obj.transform.position) >= 50)) {
                if (Time.time - lastMoveTime >= moveTime)
                {
                    // TODO 移动
                    Vector3Int move = new Vector3Int(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                    transform.LookAt(move);
                    navMeshAgent.SetDestination(move);
                    lastMoveTime = Time.time;
                }
                if (Time.time - lastRotateTime >= rotateTime)
                {
                    // TODO 转向
                    transform.LookAt(new Vector3Int(Random.Range(-10, 10), 0, Random.Range(-10, 10)));
                    lastRotateTime = Time.time;
                }
            }
            // 范围内，向玩家移动
            else if (obj.name == "player" && distance(transform.position, obj.transform.position) >= 50 && distance(transform.position, obj.transform.position) <= 200)
            {
                transform.LookAt(obj.transform.position);
                navMeshAgent.SetDestination(obj.transform.position);
            }
            // 范围内攻击玩家
            else if (obj.name == "player" && (Time.time - lastAttackTime >= attackTime) && distance(transform.position, obj.transform.position) <= 50) {
                transform.LookAt(obj.transform.position);
                Ray rayAttack;
                RaycastHit hitAttack;
                GameObject objAttack = null;
                float h = Random.Range(-1, 3);
                float v = Random.Range(-2, 2);
                rayAttack = new Ray(transform.Find("weapon").transform.position, player.transform.position+new Vector3(v,h,0) - transform.Find("weapon").transform.position);
                Debug.DrawRay(transform.Find("weapon").transform.position, player.transform.position + new Vector3(v, h, 0) - transform.Find("weapon").transform.position, Color.red);
                if (Physics.Raycast(rayAttack, out hitAttack))
                {
                    Debug.Log(hit.collider.gameObject.name);
                    objAttack = hitAttack.collider.gameObject;
                    Debug.Log(objAttack.tag);

                    GameObject go = Instantiate(firt, transform.Find("weapon").transform.position, transform.transform.rotation);
                    lastAttackTime = Time.time;
                    if (objAttack.transform.parent && objAttack.transform.parent.tag == "Player")
                    {
                        Debug.Log("attack hurt");
                        // TODO 攻击
                        objAttack.transform.parent.GetComponent<playerController>().hurt(Random.Range(20,40));
                        
                    }
                }
            }
        }
    }

    // TODO 伤害扣除
    public void hurt(int num) {
        hp -= num;
        Instantiate(hurtParticle, transform.position, transform.rotation);  // 播放出血效果   
        if (hp <= 0) {
            GameObject go = Instantiate(deadParticle);  // 播放死亡效果
            go.transform.position = transform.position;
            go.transform.parent = transform.parent;
            Destroy(gameObject);
        }
    }

    // 计算两点间的距离
    private double distance(Vector3 v1, Vector3 v2)
    {
        double num = 0;
        num = System.Math.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.z - v2.z) * (v1.z - v2.z));
        return num;
    }
}
