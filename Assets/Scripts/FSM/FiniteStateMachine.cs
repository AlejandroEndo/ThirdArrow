using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.FSM {
    public class FiniteStateMachine : MonoBehaviour {

        [SerializeField] private AbstractFSMState startingState;
        private AbstractFSMState currentState;

        private void Awake() {
            currentState = null;
        }

        private void Start() {
            if (startingState != null)
                EnterState(startingState);
        }

        private void Update() {
            if(currentState != null) {
                currentState.UpdateState();
            }
        }

        private void EnterState(AbstractFSMState nextState) {
            if (nextState == null) return;

            currentState = nextState;
            currentState.EnterState();
        }
    }
}