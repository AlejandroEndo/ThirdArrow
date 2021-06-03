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
    private PlayerMovementController playerController;
    private PlayerShootController playerShooting;



    void Start() {
        playerController = GetComponent<PlayerMovementController>();
        playerShooting = GetComponent<PlayerShootController>();
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

    }

    void Update() {
        displayArrow.SetActive(playerController.isAiming && playerController.fireRate <= 0f);
        lineRenderer.SetPosition(0, topJoint.position);
        lineRenderer.SetPosition(1, playerController.isAiming ? rightHand.position : middleCord.position);
        lineRenderer.SetPosition(2, bottomJoint.position);
    }
}
