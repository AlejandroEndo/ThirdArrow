using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour {

    [SerializeField] private Animator anim;

    [SerializeField] private PlayerMovementController playerMovementController;
    [SerializeField] private float aimAnimationSpeed;

    private float aimWeight = 0f;

    void Start() {

    }

    void Update() {
        float normalizedSpeed = Mathf.Abs(playerMovementController.playerMove.magnitude) / playerMovementController.sprintSpeed;
        anim.SetFloat("zVel_f", normalizedSpeed);
        Aiming();
    }

    void Aiming() {
        anim.SetBool("isAiming_b", playerMovementController.isAiming);
        anim.SetLayerWeight(1, aimWeight);
        if (playerMovementController.isAiming) {
            aimWeight = aimWeight >= 1 ? 1 : aimWeight + Time.deltaTime * aimAnimationSpeed;
        } else {
            aimWeight = aimWeight <= 0 ? 0 : aimWeight - Time.deltaTime * aimAnimationSpeed;
        }
    }

    public void Fire() {
        anim.SetTrigger("shoot_t");
    }
}
