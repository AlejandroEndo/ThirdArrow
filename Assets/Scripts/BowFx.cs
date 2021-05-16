using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BowFx : MonoBehaviour {

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform topJoint;
    [SerializeField] private Transform bottomJoint;
    [SerializeField] private Transform middleCord;
    [SerializeField] private Transform rightHand;
    [SerializeField] private GameObject displayArrow;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    private bool holdingArrow;
    private PlayerController playerController;
    private PlayerShooting playerShooting;



    void Start() {
        playerController = GetComponent<PlayerController>();
        playerShooting = GetComponent<PlayerShooting>();
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

    }

    void Update() {
        if (playerController.isShootPressed && playerShooting.nextShoot >= playerShooting.coolDown) {
            holdingArrow = true;
        } else {
            if (holdingArrow && playerShooting.nextShoot >= playerShooting.coolDown)
                impulseSource.GenerateImpulse();
            holdingArrow = false;
        }
        displayArrow.SetActive(holdingArrow);
        lineRenderer.SetPosition(0, topJoint.position);
        lineRenderer.SetPosition(1, playerController.isAiming ? rightHand.position : middleCord.position);
        lineRenderer.SetPosition(2, bottomJoint.position);
    }
}
