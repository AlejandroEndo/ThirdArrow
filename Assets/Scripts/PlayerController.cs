using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
    [SerializeField] InputActionReference movementController;
    [SerializeField] InputActionReference jumpController;
    [SerializeField] InputActionReference shootController;
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float rotationSpeed = 4f;
    [SerializeField] private bool jumpEnable = true;
    [SerializeField] private bool aiming = false;
    private CharacterController controller;
    private PlayerShooting shootingScript;
    [SerializeField] private Transform cameraMainTransform;

    public bool groundedPlayer;
    public Vector3 move;
    public Vector3 playerVelocity;

    private void Start() {
        controller = GetComponent<CharacterController>();
        shootingScript = GetComponent<PlayerShooting>();
    }

    void Update() {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }

        Vector2 movement = movementController.action.ReadValue<Vector2>();
        Locomotion(movement);

        if (jumpController.action.triggered && groundedPlayer && jumpEnable)
            Jump();


        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (movement != Vector2.zero)
            PlayerRotation(movement);
        Debug.Log(shootController.action.ReadValue<bool>());
        if(shootController.action.ReadValue<float>() > 0.1 && groundedPlayer) {
            OnAimToggle();
        }
        else if (aiming) {
            OnAimToggle();
        }

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

    public void OnAimToggle() {
        if (aiming) {
            shootingScript.HipsToShlouder();
        } else {
            shootingScript.ShoulderToHips();
        }
        aiming = !aiming;
    }

    private void EnableJump() {
        jumpEnable = true;
    }

    private void OnEnable() {
        movementController.action.Enable();
        jumpController.action.Enable();
    }

    private void OnDisable() {
        movementController.action.Disable();
        jumpController.action.Disable();
    }
}
