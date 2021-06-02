using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerMovementController : MonoBehaviour {

    [SerializeField] private InputActionReference movementController;
    [SerializeField] private InputActionReference sprintController;
    [SerializeField] private InputActionReference aimController;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private bool onSprint;
    [SerializeField] private float speed;
    [SerializeField] private float playerSpeed;
    public bool isAiming;
    public float sprintSpeed;

    [SerializeField] private float totalStamina;
    [SerializeField] private float currentStamina = 0f;
    [SerializeField] private float temple;
    public bool isTired = false;

    public Vector3 playerMove;
    public Vector2 movementInput;

    private float StaminaRecover {
        get {
            return movementInput.magnitude < 0.1f ? 1 : 0.5f;
        }
    }

    void Awake() {
        sprintController.action.performed += ctx => {
            onSprint = !isTired;
        };
        sprintController.action.canceled += ctx => {
            onSprint = false;
        };
        aimController.action.performed += ctx => {
            isAiming = true;
        };
        aimController.action.canceled += ctx => {
            isAiming = false;
        };
    }

    private void Update() {
        movementInput = isAiming ? Vector2.zero : movementController.action.ReadValue<Vector2>();
        if (isTired && currentStamina >= totalStamina) {
            isTired = false;
        }

        if (currentStamina < 0) {
            isTired = true;
        }

        if (!onSprint && currentStamina < totalStamina) {
            currentStamina += StaminaRecover * Time.deltaTime;
        }
    }

    void FixedUpdate() {
        Locomotion(movementInput);


        if (isAiming) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Aim"))) {
                Transform objectHit = hit.transform;
                Vector2 lookAt = new Vector2(hit.point.x - transform.position.x, hit.point.z - transform.position.z);
                PlayerRotation(lookAt);
            }
        } else if (movementInput != Vector2.zero)
            PlayerRotation(movementInput);
    }

    private void Locomotion(Vector3 movement) {
        NavMeshHit hit;
        playerMove = new Vector3(movement.x, 0f, movement.y).normalized;
        playerMove = Camera.main.transform.forward.normalized * 2 * playerMove.z + Camera.main.transform.right.normalized * playerMove.x;
        playerMove.y = 0f;
        if (onSprint && !isTired) {
            playerMove *= sprintSpeed;
            currentStamina -= temple * Time.deltaTime;
        } else {
            playerMove *= playerSpeed;
        }
        //playerMove *= onSprint ? sprintSpeed : playerSpeed;
        Vector3 newPos = transform.position + playerMove.normalized;
        Vector3 hMove = playerMove.x * Vector3.right;
        Vector3 vMove = playerMove.z * Vector3.forward;
        if (NavMesh.SamplePosition(newPos, out hit, 0.3f, NavMesh.AllAreas)) {
            if ((transform.position - hit.position).magnitude >= 0.02f) {
                navMeshAgent.Move(playerMove * Time.fixedDeltaTime);
            }
        } else {
            Vector3 bipasedMove = new Vector3();

            if (NavMesh.SamplePosition(hMove, out hit, 0.3f, NavMesh.AllAreas)) {
                if ((transform.position - hit.position).magnitude >= 0.02f) {
                    bipasedMove.z = playerMove.z;
                }
            }
            if (NavMesh.SamplePosition(vMove, out hit, 0.3f, NavMesh.AllAreas)) {
                if ((transform.position - hit.position).magnitude >= 0.02f) {
                    bipasedMove.x = playerMove.x;
                }
                navMeshAgent.Move(bipasedMove * Time.fixedDeltaTime);
            }
        }
    }

    private void PlayerRotation(Vector2 _movement) {
        float targetAngle = Mathf.Atan2(_movement.x, _movement.y) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;
        Quaternion angle = Quaternion.Euler(0f, targetAngle, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, angle, Time.deltaTime * navMeshAgent.angularSpeed);
    }

    private void OnEnable() {
        movementController.action.Enable();
        sprintController.action.Enable();
        aimController.action.Enable();
        /*
        jumpController.action.Enable();
        shootController.action.Enable();
        */
    }

    private void OnDisable() {
        movementController.action.Disable();
        sprintController.action.Disable();
        aimController.action.Disable();
        /*
        jumpController.action.Disable();
        shootController.action.Disable();
        */
    }
}
