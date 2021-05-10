using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

    [SerializeField] private Animator anim;
    [SerializeField] private PlayerController playerController;

    void Start() {
        playerController = GetComponent<PlayerController>();
    }

    void Update() {
        anim.SetFloat("zVel_f", playerController.move.normalized.magnitude);
        anim.SetFloat("yVel_f", playerController.playerVelocity.y);
        anim.SetBool("isGrounded_b", playerController.groundedPlayer);
    }
}
