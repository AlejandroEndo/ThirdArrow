using UnityEngine;

namespace Assets.Scripts.FSM {
    [CreateAssetMenu(fileName = "IdleState", menuName = "Unity-FSM/States/Idle", order = 1)]
    public class IdleState : AbstractFSMState {

        [SerializeField] private float idleDuration = 3f;
        [SerializeField] private float totalDuration = 0f;

        public override void OnEnable() {
            base.OnEnable();
            StateType = FSMStateType.IDLE;
        }
        public override bool EnterState() {
            EnteredState = base.EnterState();
            if (EnteredState) {
                totalDuration = 0f;
                Debug.Log("ENTERED IDLE STATE " + totalDuration);
            }
            return EnteredState;
        }

        public override void UpdateState() {
            if (EnteredState) {
                totalDuration += Time.deltaTime;

                if (totalDuration >= idleDuration) {
                    fsm.EnterState(FSMStateType.PATROL);
                }
            }
        }

        public override bool ExitState() {
            base.ExitState();
            Debug.Log("EXITING IDLE STATE");
            return true;
        }
    }
}