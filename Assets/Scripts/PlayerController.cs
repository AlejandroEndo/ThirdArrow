using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
    [SerializeField] private Transform cameraMainTransform;
    [SerializeField] private float gravityValue = -9.81f;
    [Header("Locomotion")]
    [SerializeField] InputActionReference movementController;
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float rotationSpeed = 4f;
    public Vector3 move;
    public Vector3 playerVelocity;
    [Header("Jump")]
    [SerializeField] InputActionReference jumpController;
    [SerializeField] private float jumpHeight = 1.0f;
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
            //shootingScript.HipsToShlouder();
            isAiming = true;
        };
        aimController.action.canceled += ctx => {
            //shootingScript.ShoulderToHips();
            isAiming = false;
        };
        shootController.action.performed += ctx => {
            isShootPressed = true;
        };
        shootController.action.canceled += ctx => {
            isShootPressed = false;
        };
    }

    private void Start() {
        controller = GetComponent<CharacterController>();
    }

    void Update() {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }

        Vector2 movement = isAiming && groundedPlayer ? Vector2.zero : movementController.action.ReadValue<Vector2>();
        Locomotion(movement);

        if (jumpController.action.triggered && groundedPlayer && jumpEnable)
            Jump();


        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (movement != Vector2.zero || isAiming)
            PlayerRotation(movement);
    }

    private void Locomotion(Vector2 movement) {
        if (groundedPlayer) {
            move = new Vector3(movement.x, 0, movement.y).normalized;
            move = cameraMainTransform.forward * move.z + cameraMainTransform.right * move.x;
            move.y = 0f;
        }
        controller.Move(move * Time.deltaTime * playerSpeed);
    }

    private void Jump() {
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        move *= 0.9f;
        groundedPlayer = false;
        jumpEnable = false;
        Invoke("EnableJump", 2f);
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
    }

    private void OnDisable() {
        movementController.action.Disable();
        jumpController.action.Disable();
        shootController.action.Disable();
        aimController.action.Disable();
    }
}
