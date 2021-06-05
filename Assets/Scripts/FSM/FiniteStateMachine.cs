using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.FSM {
    public class FiniteStateMachine : MonoBehaviour {
/*
        [SerializeField] private AbstractFSMState currentState;

        public AbstractFSMState[] validStates;
        private Dictionary<FSMStateType, AbstractFSMState> fsmStates;

        [SerializeField] private GameObject idleDisplay;
        [SerializeField] private GameObject patrolDisplay;

        private void Awake() {
            currentState = null;
            fsmStates = new Dictionary<FSMStateType, AbstractFSMState>();

            NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
            NPC npc = GetComponent<NPC>();

            foreach (AbstractFSMState state in validStates) {
                //AbstractFSMState fsm = new AbstractFSMState(this, npc, navMeshAgent);
                state.SetExecutingFSM(this);
                state.SetExecutingNPC(npc);
                state.SetNavMeshAgent(navMeshAgent);
                //state.SetNewNPC(npc, this);
                fsmStates.Add(state.StateType, state);
            }
        }

        private void Start() {
            EnterState(FSMStateType.IDLE);

        }

        private void Update() {
            if (currentState != null) {
                currentState.UpdateState();
            }
        }

        public void EnterState(AbstractFSMState nextState) {
            if (nextState == null) return;

            if (currentState != null) currentState.ExitState();

            currentState = nextState;
            currentState.EnterState();
        }

        public void EnterState(FSMStateType stateType) {
            if (fsmStates.ContainsKey(stateType)) {
                AbstractFSMState nextState = fsmStates[stateType];
                EnterState(nextState);

                idleDisplay.SetActive(stateType == FSMStateType.IDLE);
                patrolDisplay.SetActive(stateType == FSMStateType.PATROL);
            }
        }
*/
    }
}