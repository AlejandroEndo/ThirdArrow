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

    public Vector3 spawnPoint;
    protected Transform target;

    protected NavMeshAgent agent;
    protected Rigidbody rb;
    protected SphereCollider detectionCollider;

    public NPCStats stats;
    public FSMStateType state;
    public GameObject spawnObject;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        state = FSMStateType.IDLE;
        detectionCollider = GetComponent<SphereCollider>();

        detectionCollider.radius = stats.detectionRange;
        agent.speed = stats.patrolSpeed;
        agent.stoppingDistance = stats.attackRange;
    }

    public virtual void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            target = other.gameObject.transform;
        }
    }

    public virtual void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            agent.SetDestination(target.position);
            agent.speed = stats.chaseSpeed;
        }
    }

    public virtual void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            agent.SetDestination(spawnPoint);
            agent.speed = stats.patrolSpeed;
            target = null;
        }
    }

    public virtual void FixedUpdate() {
        if (target != null) {
            if (Vector3.Distance(target.position, transform.position) < stats.scapeDistance) {
                Vector3 scapePos = transform.position - target.position;
                agent.SetDestination(scapePos.normalized * stats.attackRange);
                spawnObject.transform.position = transform.position + scapePos;
                agent.stoppingDistance = 0f;
            } else {
                agent.SetDestination(target.position);
                agent.stoppingDistance = stats.attackRange;
            }

        } else if (spawnPoint != null) {
            //spawnObject.transform.position = spawnPoint;
        }
        /*
        if (Vector3.Distance(target.position, transform.position) < stats.detectionRange) {
            agent.SetDestination(target.position);
        } else {
            target = null;
        }
        */
    }


    public virtual void LateUpdate() {
        //if (spawnPoint == null) spawnPoint = transform.position;

        if (state == FSMStateType.DEATH) {
            // TODO: death
            Destroy(gameObject, 2f);
        }
    }

    public virtual void ChaseTarget() {
        if (target == null) return;
    }

}












/*

NavMeshAgent navMeshAgent;

[SerializeField] private Transform[] patrolPoints;

[SerializeField] private Transform originalPos;
[SerializeField] private Transform initialPos;
[SerializeField] private float patrolRange;
[SerializeField] private float patrolOffset;

[SerializeField] private int checkedIndex;

[SerializeField] private float stateDuration = 3f;
[SerializeField] private float totalDuration = 0f;
[SerializeField] private float idleDuration = 0f;
[SerializeField] private float patrolDuration = 0f;
[SerializeField] private Vector2 idleDurationRange;
[SerializeField] private Vector2 patrolDurationRange;
[SerializeField] private float speed = 1f;
[SerializeField] private Vector3 moveDir;

public Vector3 target;

FSMStateType state;

private Rigidbody rb;

private void Awake() {
    navMeshAgent = GetComponent<NavMeshAgent>();
}

private void Start() {
    initialPos = transform;
    state = FSMStateType.PATROL;

    rb = GetComponent<Rigidbody>();
}

private void Update() {
    totalDuration += Time.deltaTime;

    if (rb.velocity.magnitude > 0.1f) {
        rb.velocity *= 0.9f;
    }

    if (totalDuration >= stateDuration) {
        totalDuration = 0f;
        SetFSMState();
    }
    if (state == FSMStateType.PATROL)
        navMeshAgent.SetDestination(transform.position + moveDir * speed);
}

private void SetFSMState() {
    switch (state) {
        case FSMStateType.IDLE:
            state = FSMStateType.PATROL;
            stateDuration = Random.Range(patrolDurationRange.x, patrolDurationRange.y);
            break;
        case FSMStateType.PATROL:
            moveDir = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)).normalized;
            state = FSMStateType.IDLE;
            stateDuration = Random.Range(idleDurationRange.x, idleDurationRange.y);
            break;
    }
}

public bool GoToNextStep() { return totalDuration >= stateDuration; }

public void SetTimeToNextState(float _stateDuration) {
    stateDuration = _stateDuration;
    totalDuration = 0f;
}

 */