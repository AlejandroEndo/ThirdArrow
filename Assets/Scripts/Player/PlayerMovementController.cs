using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerMovementController : MonoBehaviour {

    [Header("Config")]
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float staminaRecoverSpeed;
    public GameObject aimingTo;

    [Header("Movement")]
    [SerializeField] private InputActionReference movementController;
    [SerializeField] private float speed;
    [SerializeField] private float basePlayerSpeed;
    public bool isTired = false;
    public Vector3 playerMove;
    public Vector2 movementInput;

    [Header("Sprint")]
    [SerializeField] private InputActionReference sprintController;
    [SerializeField] private bool onSprint;
    public float sprintSpeed;
    public float totalStamina;
    public float currentStamina = 0f;
    [SerializeField] private float temple;

    public float PlayerSpeed { get { return isTired ? basePlayerSpeed * 0.75f : basePlayerSpeed; } }
    private float StaminaRecover { get { return movementInput.magnitude < 0.1f ? staminaRecoverSpeed : staminaRecoverSpeed * 0.5f; } }

    [Header("Shooting")]
    [SerializeField] private InputActionReference aimController;
    [SerializeField] private InputActionReference shootController;
    public bool isAiming;
    public bool isShootPressed;
    public Vector3 lookAt;
    [Range(0.1f, 1f)]
    [SerializeField] private float totalFireRate;
    public float fireRate;

    public bool TriggerShoot {
        get {
            if (isShootPressed) {
                isShootPressed = false;
                fireRate = totalFireRate;
                return true;
            }
            return isShootPressed;
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
        shootController.action.started += ctx => {
            isShootPressed = fireRate <= 0 && isAiming;
        };
        shootController.action.canceled += ctx => {
            isShootPressed = false;
        };
    }

    private void Update() {
        if (fireRate > 0f) fireRate -= Time.deltaTime;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        movementInput = isAiming ? Vector2.zero : movementController.action.ReadValue<Vector2>();
        if (isTired && currentStamina >= totalStamina) {
            isTired = false;
        }

        if (currentStamina < 0) {
            isTired = true;
        }

        if ((!onSprint || movementInput.magnitude < 0.1f) && currentStamina < totalStamina) {
            currentStamina += StaminaRecover * Time.deltaTime;
        }
        if (isAiming) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Aim"))) {
                //Transform objectHit = hit.transform;
                //Vector3 newPoint = Vec
                //lookAt = new Vector2(hit.point.x, hit.point.z).normalized;
                //hit.point.y = 1.3f;
                //lookAt = hit.point;
                aimingTo.transform.position = hit.point;
                //transform.r();
                PlayerRotation(new Vector2(aimingTo.transform.position.x - transform.position.x, aimingTo.transform.position.z - transform.position.z).normalized);
            }
        } else if (movementInput != Vector2.zero)
            PlayerRotation(movementInput);
    }

    void FixedUpdate() {
        Locomotion(movementInput);


        if (isAiming) {
            //Vector3 mousePos = Input.mousePosition;
            //mousePos.z = 25f;
            //Vector3 point = Camera.main.ScreenToWorldPoint(mousePos);
            //point.y = 1.3f;
            //lookAt = point;//new Vector3(point.x, 0f, point.z) + transform.position;
            //aimingTo.transform.position = lookAt;
            //PlayerRotation(lookAt);
            //☺Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            /*
            if (Physics.Raycast(ray, out RaycastHit hit, LayerMask.GetMask("Aim"))) {
                //Transform objectHit = hit.transform;
                //Vector3 newPoint = Vec
                lookAt = new Vector2(hit.point.x, hit.point.z).normalized;
                //hit.point.y = 1.3f;
                aimingTo.transform.position = hit.point;
                PlayerRotation(lookAt);
            }
             */
        } else if (movementInput != Vector2.zero)
            PlayerRotation(movementInput);
    }

    private void Locomotion(Vector3 movement) {
        playerMove = new Vector3(movement.x, 0f, movement.y).normalized;
        playerMove = Camera.main.transform.forward.normalized * 2 * playerMove.z + Camera.main.transform.right.normalized * playerMove.x;
        playerMove.y = 0f;
        if (onSprint && !isTired && movement.magnitude > 0.1f) {
            playerMove *= sprintSpeed;
            currentStamina -= temple * Time.deltaTime;
        } else {
            playerMove *= PlayerSpeed;
        }
        //playerMove *= onSprint ? sprintSpeed : playerSpeed;
        Vector3 newPos = transform.position + playerMove.normalized;
        if (NavMesh.SamplePosition(newPos, out NavMeshHit hit, 0.3f, NavMesh.AllAreas)) {
            if ((transform.position - hit.position).magnitude >= 0.02f) {
                navMeshAgent.Move(playerMove * Time.fixedDeltaTime);
            }
        } else {
            Vector3 bipasedMove = new Vector3();
            Vector3 hMove = playerMove.x * Vector3.right;
            Vector3 vMove = playerMove.z * Vector3.forward;

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
        shootController.action.Enable();
        /*
        jumpController.action.Enable();
        */
    }

    private void OnDisable() {
        movementController.action.Disable();
        sprintController.action.Disable();
        aimController.action.Disable();
        shootController.action.Disable();
        /*
        jumpController.action.Disable();
        */
    }
}
