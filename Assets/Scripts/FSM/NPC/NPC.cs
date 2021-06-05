using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.FSM {
    public enum FSMStateType {
        IDLE,
        PATROL,
    };

    [RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(FiniteStateMachine))]
    public class NPC : MonoBehaviour {

        NavMeshAgent navMeshAgent;
        //FiniteStateMachine finiteStateMachine;

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

        //public Transform[] PatrolPoints { get { return patrolPoints; } }
        public Vector3 target;

        FSMStateType state;

        private Rigidbody rb;

        private void Awake() {
            navMeshAgent = GetComponent<NavMeshAgent>();
            //finiteStateMachine = GetComponent<FiniteStateMachine>();
        }

        private void Start() {
            initialPos = transform;
            state = FSMStateType.PATROL;

            rb = GetComponent<Rigidbody>();
        }

        private void Update() {
            totalDuration += Time.deltaTime;

            if(rb.velocity.magnitude > 0.1f) {
                rb.velocity *= 0.9f;
            }

            if (totalDuration >= stateDuration) {
                totalDuration = 0f;
                SetFSMState();
            }
            if (state == FSMStateType.PATROL)
                navMeshAgent.SetDestination(transform.position + moveDir * speed);
            /*
            if (originalPos == initialPos) {
                SetPatrolPoints();
            }
            
            if (state == FSMStateType.PATROL) {
                moveDir = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)).normalized;
                navMeshAgent.SetDestination(transform.position + moveDir * speed);
                state = FSMStateType.IDLE;
                totalDuration = idleDuration;
            }
            */
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

        private void SetPatrolPoints() {
            //bool patrolPointsCheck = true;
            originalPos = transform;
            patrolPoints[checkedIndex].position = originalPos.position;
            checkedIndex++;

            while (checkedIndex <= patrolPoints.Length) {
                if (checkedIndex == patrolPoints.Length) {
                    foreach (Transform point in patrolPoints) {
                        Debug.Log("change parent");
                        point.parent = null;// GameObject.FindGameObjectWithTag("MapManager").transform;
                        checkedIndex++;
                    }
                } else {
                    for (int i = checkedIndex; i < patrolPoints.Length; i++) {
                        NavMeshHit hit;
                        Vector3 randomPos = new Vector3(Random.Range(0f, 1f), 0f, Random.Range(0f, 1f));
                        randomPos *= patrolRange;
                        if (NavMesh.SamplePosition(randomPos, out hit, 1f, NavMesh.AllAreas)) {
                            if (hit.hit) {
                                patrolPoints[checkedIndex].position = randomPos;
                                checkedIndex++;

                            }
                        }

                    }
                }
            }
        }

    }
}