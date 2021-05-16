using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
    private Transform cameraMainTransform;

    [Header("Locomotion")]
    [SerializeField] InputActionReference movementController;
    [SerializeField] private PhysicMaterial physicMaterial;
    [SerializeField] private float playerSpeed = 2f;
    [SerializeField] private float rotationSpeed = 4f;
    public Vector3 move;
    public Vector3 playerVelocity;

    [Header("Sprint")]
    [SerializeField] InputActionReference sprintController;
    public float sprintSpeed = 4f;
    [SerializeField] private bool onSprint = false;

    [Header("Slopes")]
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float slopeRayLength = 2f;
    [SerializeField] private float slopeForce;
    [SerializeField] private float stepOffset;

    [Header("Jump")]
    [SerializeField] InputActionReference jumpController;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float jumpCoolDown;
    [Range(0.0f, 1f)]
    [SerializeField] private float onAirSpeedModifier;
    [SerializeField] private bool jumpEnable = true;
    public bool groundedPlayer;

    [Header("Aim and Shoot")]
    [SerializeField] InputActionReference shootController;
    [SerializeField] InputActionReference aimController;
    
    private CharacterController controller;
    private PlayerShooting shootingScript;
    public bool isAiming;
    public bool isShootPressed = false;

    private void Awake() {
        shootingScript = GetComponent<PlayerShooting>();
        aimController.action.performed += ctx => {
            isAiming = true;
        };
        aimController.action.canceled += ctx => {
            isAiming = false;
        };
        shootController.action.performed += ctx => {
            isShootPressed = true;
        };
        shootController.action.canceled += ctx => {
            isShootPressed = false;
        };
        sprintController.action.performed += ctx => {
            onSprint = true;
        };
        sprintController.action.canceled += ctx => {
            onSprint = false;
        };
    }

    private void Start() {
        cameraMainTransform = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        controller.material = physicMaterial;
    }

    void Update() {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
            controller.stepOffset = stepOffset;
        }

        Vector2 movement = isAiming && groundedPlayer ? Vector2.zero : movementController.action.ReadValue<Vector2>();
        Locomotion(movement);

        if (jumpController.action.triggered && groundedPlayer && jumpEnable)
            Jump();


        float extraForce = IsGoinDown() && movement != Vector2.zero ? slopeForce : 1;
        playerVelocity.y += gravityValue * Time.deltaTime * extraForce;
        controller.Move(playerVelocity * Time.deltaTime);

        if (movement != Vector2.zero || isAiming)
            PlayerRotation(movement);
    }

    private bool IsGoinDown() {
        if (controller.velocity.y > 0) return false;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, slopeRayLength)) {
            if (hit.normal != Vector3.up) return true;
        }
        return false;
    }

    private void Locomotion(Vector2 movement) {
        if (groundedPlayer) {
            move = new Vector3(movement.x, 0, movement.y).normalized;
            move = cameraMainTransform.forward * move.z + cameraMainTransform.right * move.x;
            move.y = 0f;
            move *= onSprint ? sprintSpeed : playerSpeed;
        }
        controller.Move(move * Time.deltaTime);
    }

    private void Jump() {
        controller.stepOffset = 0f;
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        move *= onAirSpeedModifier;
        groundedPlayer = false;
        jumpEnable = false;
        Invoke(nameof(EnableJump), jumpCoolDown);
    }

    private void PlayerRotation(Vector2 movement) {
        float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + cameraMainTransform.rotation.eulerAngles.y;
        Quaternion angle = Quaternion.Euler(0f, targetAngle, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, angle, Time.deltaTime * rotationSpeed);
    }

    private void EnableJump() {
        jumpEnable = true;
    }

    private void OnEnable() {
        movementController.action.Enable();
        jumpController.action.Enable();
        shootController.action.Enable();
        aimController.action.Enable();
        sprintController.action.Enable();
    }

    private void OnDisable() {
        movementController.action.Disable();
        jumpController.action.Disable();
        shootController.action.Disable();
        aimController.action.Disable();
        sprintController.action.Disable();
    }
}
