using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public enum FSMStateType {
    IDLE,
    PATROL,
    CHASE,
    ATTACK,
    RETURN,
    DEATH,
};

[RequireComponent(typeof(NavMeshAgent))]
public abstract class NPC : MonoBehaviour {

    public Transform spawnPoint;

    public NPCStats stats;
    public FSMStateType state;
    public GameObject spawnObject;

    protected Vector3 target;

    protected NavMeshAgent agent;
    protected Rigidbody rb;
    protected SphereCollider detectionCollider;

    protected float stateTime = 0f;

    protected Transform playerTransform;


    private void Awake() {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        SetFSMState(FSMStateType.IDLE);
        detectionCollider = GetComponent<SphereCollider>();

        detectionCollider.radius = stats.detectionRange;
        agent.speed = stats.patrolSpeed;
        //agent.stoppingDistance = stats.attackRange;
    }

    private void Start() {

    }

    public virtual void FixedUpdate() {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        StateUpdate();
    }

    private void Update() {
    }

    public virtual void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            playerTransform = other.transform;
            SetFSMState(FSMStateType.CHASE);
        }
    }
    public virtual void OnCollisionEnter(Collision collision) {
        if (collision.transform.CompareTag("Arrow")) {
            Transform arrow = collision.transform;
            ArrowProjectile arrowScript = arrow.GetComponent<ArrowProjectile>();
            DamageDisplay.Create(transform.position + Vector3.up * 2f, arrowScript.damage, DamageType.CRITICAL);
            agent.Move(collision.transform.forward.normalized * 0.1f);
            //Vector3 result = collision.transform.forward + transform.forward * -1;
            //rb.AddForce(result.normalized * 50f, ForceMode.Impulse);
            //rb.AddExplosionForce(50.0f, collision.transform.GetComponent<ArrowProjectile>().head.position, 2f);
        }
    }

    public virtual void StateUpdate() {
        switch (state) {
            case FSMStateType.IDLE:
                stateTime -= Time.fixedDeltaTime;
                if (stateTime <= 0f) {
                    stateTime = Random.Range(stats.idleDuration.x, stats.idleDuration.y);
                    SetFSMState(FSMStateType.PATROL);
                }
                break;
            case FSMStateType.PATROL:
                stateTime -= Time.fixedDeltaTime;
                if (target != null)
                    agent.SetDestination(target);

                if (stateTime <= 0f) {
                    stateTime = Random.Range(stats.idleDuration.x, stats.idleDuration.y);
                    SetFSMState(FSMStateType.IDLE);
                }
                break;
            case FSMStateType.CHASE:
                if (playerTransform != null)
                    agent.SetDestination(playerTransform.position);

                if (Vector3.Distance(transform.position, playerTransform.position) > stats.ChaseRange) {
                    SetFSMState(FSMStateType.RETURN);
                } else if (Vector3.Distance(transform.position, playerTransform.position) < stats.attackRange + agent.radius) {
                    SetFSMState(FSMStateType.ATTACK);
                }
                break;
            case FSMStateType.RETURN:
                playerTransform = null;
                agent.SetDestination(transform.parent.position);

                if (Vector3.Distance(transform.position, spawnPoint.position) < stats.patrolRange * 0.8f) {
                    SetFSMState(FSMStateType.IDLE);
                }
                break;
            case FSMStateType.ATTACK:
                stateTime -= Time.fixedDeltaTime;
                if (stateTime <= 0f) {
                    stateTime = stats.waitAfterAttack;
                    Invoke(nameof(AttackAction), stats.waitBeforeAttack);
                }
                break;
            case FSMStateType.DEATH:
                break;
        }
    }

    public virtual void SetFSMState(FSMStateType newState) {
        state = newState;
        switch (newState) {
            case FSMStateType.IDLE:
                agent.isStopped = true;
                if (agent.speed == stats.chaseSpeed) agent.speed = stats.patrolSpeed;
                break;
            case FSMStateType.PATROL:
                target = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)) * stats.patrolRange + spawnPoint.position;
                agent.isStopped = false;
                break;
            case FSMStateType.CHASE:
                if (agent.isStopped) agent.isStopped = false;
                agent.speed = stats.chaseSpeed;
                //target = playerTransform.position;
                break;
            case FSMStateType.RETURN:
                target = Vector3.zero;
                if (agent.isStopped) agent.isStopped = false;
                break;
            case FSMStateType.ATTACK:
                agent.isStopped = true;
                stateTime = stats.waitBeforeAttack;
                break;
            case FSMStateType.DEATH:
                Destroy(gameObject, 2f);
                break;
        }
    }


    protected void AttackAction() {
        Debug.Log("ATTACK!");
        SetFSMState(FSMStateType.CHASE);
    }

}
