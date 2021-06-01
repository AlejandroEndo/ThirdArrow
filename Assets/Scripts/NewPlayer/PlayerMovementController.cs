using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerMovementController : MonoBehaviour {

    [SerializeField] private InputActionReference movementController;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private bool isAiming;
    [SerializeField] private bool onSprint;
    [SerializeField] private float speed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float playerSpeed;

    public Vector3 move;

    void Start() {

    }

    void FixedUpdate() {
        Vector2 movement = isAiming ? Vector2.zero : movementController.action.ReadValue<Vector2>();

        if (movement.sqrMagnitude >= 0.1f) {
            Locomotion(movement);
        }

        if (movement != Vector2.zero || isAiming)
            PlayerRotation(movement);
    }

    private void Locomotion(Vector3 movement) {
        NavMeshHit hit;
        move = new Vector3(movement.x, 0f, movement.y).normalized;
        move = Camera.main.transform.forward.normalized * 2 * move.z + Camera.main.transform.right.normalized * move.x;
        move.y = 0f;
        move *= onSprint ? sprintSpeed : playerSpeed;
        Vector3 newPos = transform.position + move.normalized;
        Vector3 hMove = move.x * Vector3.right;
        Vector3 vMove = move.z * Vector3.forward;
        if (NavMesh.SamplePosition(newPos, out hit, 0.3f, NavMesh.AllAreas)) {
            if ((transform.position - hit.position).magnitude >= 0.02f) {
                navMeshAgent.Move(move * Time.fixedDeltaTime);
            }
        } else {
            Vector3 bipasedMove = new Vector3();

            if (NavMesh.SamplePosition(hMove, out hit, 0.3f, NavMesh.AllAreas)) {
                if ((transform.position - hit.position).magnitude >= 0.02f) {
                    bipasedMove.z = move.z;
                }
            }
            if (NavMesh.SamplePosition(vMove, out hit, 0.3f, NavMesh.AllAreas)) {
                if ((transform.position - hit.position).magnitude >= 0.02f) {
                    bipasedMove.x = move.x;
                }
                navMeshAgent.Move(bipasedMove * Time.fixedDeltaTime);
            }
        }
    }

    private void PlayerRotation(Vector2 movement) {
        float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;
        Quaternion angle = Quaternion.Euler(0f, targetAngle, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, angle, Time.deltaTime * navMeshAgent.angularSpeed);
    }

    private void OnEnable() {
        movementController.action.Enable();
        /*
        jumpController.action.Enable();
        shootController.action.Enable();
        aimController.action.Enable();
        sprintController.action.Enable();
        */
    }

    private void OnDisable() {
        movementController.action.Disable();
        /*
        jumpController.action.Disable();
        shootController.action.Disable();
        aimController.action.Disable();
        sprintController.action.Disable();
        */
    }
}
