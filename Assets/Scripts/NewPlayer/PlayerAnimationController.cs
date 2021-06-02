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
        float a = Mathf.Abs(playerMovementController.playerMove.magnitude) / playerMovementController.sprintSpeed;
        anim.SetFloat("zVel_f", a);
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
        /*
        anim.SetBool("holdingArrow_b", playerShooting.currentShootCharge > 0);
        anim.SetFloat("shootCharge_f", playerShooting.currentShootCharge / playerShooting.chargeShootLimit);

        }*/
    }
}
