using Assets.Scripts.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ExecutionState {
    NONE,
    ACTIVE,
    COMPLETED,
    TERMINATED,
};

public enum FSMStateType {
    IDLE,
    PATROL,
};

public abstract class AbstractFSMState : ScriptableObject {

    protected NavMeshAgent navMeshAgent;
    protected NPC npc;
    protected FiniteStateMachine fsm;
    public ExecutionState ExecutionState { get; protected set; }
    public FSMStateType StateType { get; protected set; }
    public bool EnteredState { get; protected set; }

    public virtual void OnEnable() {
        ExecutionState = ExecutionState.ACTIVE;
        //return true;
    }

    public virtual bool EnterState() {
        ExecutionState = ExecutionState.ACTIVE;

        bool successNavMesh = navMeshAgent != null;
        bool successNPC = npc != null;
        return successNavMesh && successNPC;
    }

    public abstract void UpdateState();

    public virtual bool ExitState() {
        ExecutionState = ExecutionState.COMPLETED;
        return true;
    }

    public virtual void SetNavMeshAgent(NavMeshAgent _navMeshAgent) {
        if (_navMeshAgent != null) {
            navMeshAgent = _navMeshAgent;
        }
    }

    public virtual void SetExecutingFSM(FiniteStateMachine _fsm) {
        if (_fsm != null) {
            fsm = _fsm;
        }
    }

    public virtual void SetExecutingNPC(NPC _npc) {
        if (_npc != null) {
            npc = _npc;
        }
    }
}
