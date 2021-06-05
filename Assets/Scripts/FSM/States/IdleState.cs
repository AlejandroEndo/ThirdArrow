using UnityEngine;

namespace Assets.Scripts.FSM {
    //[CreateAssetMenu(fileName = "IdleState", menuName = "Unity-FSM/States/Idle", order = 1)]
    public class IdleState : AbstractFSMState {
        
        [SerializeField] private float stateTime = 3f;
        public override void OnEnable() {
            base.OnEnable();
            StateType = FSMStateType.IDLE;
        }
        public override bool EnterState() {
            EnteredState = base.EnterState();
            if (EnteredState) {
                npc.SetTimeToNextState(stateTime);
                Debug.Log("ENTERED IDLE STATE");
            }
            return EnteredState;
        }

        public override void UpdateState() {
            if (EnteredState) {
                if (npc.GoToNextStep()) {
                   // fsm.EnterState(FSMStateType.PATROL);
                }
            }
        }

        public override bool ExitState() {
            base.ExitState();
            return true;
        }
    }
}