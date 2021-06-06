using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootController : MonoBehaviour {
    [SerializeField] private Transform firePoint;
    [SerializeField] private PlayerMovementController movementController;
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] GameManager objectPooler;
    void Start() {
        objectPooler = GameManager.Instance;
    }

    void Update() {
        if (movementController.TriggerShoot) {
            Vector2 lookAt = movementController.lookAt;
            GameObject arrow = objectPooler.SpawnFromPool("Arrow", firePoint.position, Quaternion.LookRotation(new Vector3(lookAt.x, 0f, lookAt.y)));
            //arrow.GetComponent<ArrowProjectile>().Fire();
            animationController.Fire();
        }
    }
}
