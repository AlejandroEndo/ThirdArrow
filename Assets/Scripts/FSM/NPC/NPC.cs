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
    protected Vector3 target;

    protected NavMeshAgent agent;
    protected Rigidbody rb;
    protected SphereCollider detectionCollider;

    public NPCStats stats;
    public FSMStateType state;
    public GameObject spawnObject;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        SetFSMState(FSMStateType.IDLE);
        detectionCollider = GetComponent<SphereCollider>();

        detectionCollider.radius = stats.detectionRange;
        agent.speed = stats.patrolSpeed;
        //agent.stoppingDistance = stats.attackRange;
    }

    public virtual void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            //state = FSMStateType.CHASE;
            //target = other.gameObject.transform.position;
        }
    }

    public virtual void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            // agent.SetDestination(target.position);
            //agent.speed = stats.chaseSpeed;
        }
    }

    public virtual void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            //agent.SetDestination(spawnPoint);
            //agent.speed = stats.patrolSpeed;
            //state = FSMStateType.RETURN;
            //target = Vector3.zero;
        }
    }

    public virtual void SetFSMState(FSMStateType newState) {
        state = newState;
        switch (newState) {
            case FSMStateType.IDLE:
                //target = Vector3.zero;
                StartCoroutine(NewPatrolPoint());
                agent.isStopped = true;
                break;
            case FSMStateType.PATROL:
                target = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)) * stats.patrolRange;
                spawnObject.transform.localPosition = target;
                agent.isStopped = false;
                StartCoroutine(NewPatrolPoint());
                break;
            case FSMStateType.CHASE:
                break;
            case FSMStateType.RETURN:
                break;
            case FSMStateType.ATTACK:
                break;
            case FSMStateType.DEATH:
                Destroy(gameObject, 2f);
                break;
        }
    }

    public virtual void FixedUpdate() {
        if (!agent.isStopped) {
            agent.SetDestination(spawnPoint + target);
        }
        // SetFSMState();
        /*
        if (target != null) {
            if (Vector3.Distance(target.position, transform.position) < stats.scapeDistance) {
                Vector3 scapePos = transform.position - target.position;
                agent.SetDestination(scapePos.normalized * stats.attackRange);
                spawnObject.transform.position = transform.position - scapePos;
                agent.stoppingDistance = 0f;
            } else {
                agent.SetDestination(target.position);
                agent.stoppingDistance = stats.attackRange;
            }

        } else if (spawnPoint != null) {
            //spawnObject.transform.position = spawnPoint;
        }
        */

        /*
        if (Vector3.Distance(target.position, transform.position) < stats.detectionRange) {
            agent.SetDestination(target.position);
        } else {
            target = null;
        }
        */
    }

    private IEnumerator NewPatrolPoint() {
        yield return new WaitForSeconds(stats.idleDuration);
        FSMStateType newState = state == FSMStateType.PATROL ? FSMStateType.IDLE : FSMStateType.PATROL;
        SetFSMState(newState);
    }
}
