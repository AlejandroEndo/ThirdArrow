using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

    [SerializeField] private Animator anim;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerShooting playerShooting;

    [SerializeField] private float aimWeight;

    void Start() {
        playerController = GetComponent<PlayerController>();
        playerShooting = GetComponent<PlayerShooting>();
        aimWeight = 0;
    }

    void Update() {
        anim.SetFloat("zVel_f", playerController.move.normalized.magnitude);
        anim.SetFloat("yVel_f", playerController.playerVelocity.y);
        anim.SetBool("isGrounded_b", playerController.groundedPlayer);
        Aiming();
    }

    void Aiming() {
        anim.SetBool("isAiming_b", playerController.isAiming);
        anim.SetBool("holdingArrow_b", playerShooting.currentShootCharge > 0);
        anim.SetLayerWeight(1, aimWeight);
        anim.SetFloat("shootCharge_f", playerShooting.currentShootCharge / playerShooting.chargeShootLimit);

        if (playerController.isAiming && playerController.groundedPlayer) {
            aimWeight = aimWeight >= 1 ? 1 : aimWeight + Time.time * 0.025f;
        } else {
            aimWeight = aimWeight <= 0 ? 0 : aimWeight - Time.time * 0.025f;
        }
    }

    
}
