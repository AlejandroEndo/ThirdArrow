using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum NPCType {
    PAWN, // Peon
    ROOK, // Torre
    KNIGHT, // Caballo
    BISHOP, // Alfil
    QUEEN, // Reina
    KING, // Rey
};

[CreateAssetMenu(fileName = "NPCStats", menuName = "NPC/NPC Stats", order = 1)]
public class NPCStats : ScriptableObject {
    public NPCType type;

    public float detectionRange;
    public float patrolRange;
    public float idleDuration;
    public float patrolSpeed;
    public float chaseSpeed;
    public float ChaseRange;
    public float attackRange;
    public float waitBeforeAttack;
    public float waitAfterAttack;
    public float damage;
    public float scapeDistance;
    [Range(1f, 2f)]
    public float criticalMultiplier;
    [Range(0f,1f)]
    public float criticalChance;
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
