using System.Collections;
using UnityEngine;

namespace Assets.Scripts.FSM.States {
    [CreateAssetMenu(fileName = "PatrolState", menuName = "Unity-FSM/States/Patrol", order = 2)]
    public class PatrolState : AbstractFSMState {

        private Transform[] patrolPoints;
        private int patrolPointIndex;

        public override void OnEnable() {
            base.OnEnable();
            StateType = FSMStateType.PATROL;
            patrolPointIndex = -1;
        }

        public override bool EnterState() {
            if (base.EnterState()) {
                EnteredState = false;
                patrolPoints = npc.PatrolPoints;
                if (patrolPoints == null || patrolPoints.Length == 0) {
                    Debug.LogError("PatrolState: Failed to grab patrol points from the npc.");
                } else {
                    if (patrolPointIndex < 0) patrolPointIndex = Random.Range(0, patrolPoints.Length);
                    else patrolPointIndex = (patrolPointIndex + 1) % patrolPoints.Length;

                    SetDestination(patrolPoints[patrolPointIndex]);
                    EnteredState = true;
                }
            }
            return EnteredState;
        }

        public override void UpdateState() {
            if (EnteredState) {
                float a = Vector3.Distance(navMeshAgent.transform.position, patrolPoints[patrolPointIndex].transform.position);
                Debug.Log(a);
                if (Vector3.Distance(navMeshAgent.transform.position, patrolPoints[patrolPointIndex].transform.position) <= 1.5f) {
                    fsm.EnterState(FSMStateType.IDLE);
                }
            }
        }

        private void SetDestination(Transform patrolPoint) {
            if (navMeshAgent != null && patrolPoint != null) {
                navMeshAgent.SetDestination(patrolPoint.position);
            }
        }
    }
}