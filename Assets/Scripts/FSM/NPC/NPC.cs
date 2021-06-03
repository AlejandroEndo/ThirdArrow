using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.FSM {
    [RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(FiniteStateMachine))]
    public class NPC : MonoBehaviour {

        NavMeshAgent navMeshAgent;
        FiniteStateMachine finiteStateMachine;

        [SerializeField] private Transform[] patrolPoints;

        public Transform[] PatrolPoints { get { return patrolPoints; } }

        private void Awake() {
            navMeshAgent = GetComponent<NavMeshAgent>();
            finiteStateMachine = GetComponent<FiniteStateMachine>();
        }

        private void Start() {

        }

        private void Update() {
        }
    }
}