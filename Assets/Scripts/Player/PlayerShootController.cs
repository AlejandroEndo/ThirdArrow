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
            Transform lookAt = movementController.aimingTo.transform;
            GameObject arrow = objectPooler.SpawnFromPool("Arrow", firePoint.position, lookAt.rotation);
            ///arrow.transform.LookAt(new Vector3(lookAt.x, firePoint.position.y, lookAt.y));
            arrow.GetComponent<ArrowProjectile>().Fire();
            animationController.Fire();
        }
    }
}
