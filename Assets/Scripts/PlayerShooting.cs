using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerShooting : MonoBehaviour {
    //[SerializeField] private CinemachineVirtualCamera cam;
    [Header("Aiming")]
    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] private GameObject lookAt;
    [SerializeField] private GameObject followAt;
    [SerializeField] private GameObject crossHair;
    [SerializeField] private float aimValue;
    [Range(0.1f, 5f)]
    public float aimSpeed;
    private Vector3 followAimPos;
    private Vector3 followHipPos;
    private Vector3 lookAimPos;
    private Vector3 lookHipPos;

    [Header("Arrow")]
    [SerializeField] private GameObject fireHole;
    [SerializeField] private GameObject arrowPrefab;
    [Range(0.1f, 2f)]
    public float coolDown;
    public float nextShoot = 0f;
    [Range(0.5f, 3f)]
    public float chargeShootLimit;
    public float currentShootCharge = 0f;

    [Header("Debug")]
    [SerializeField] private GameObject collidePrefab;
    private PlayerController playerController;
    [SerializeField] private Animator anim;
    [Range(1f, 1000f)]
    [SerializeField] private float distance;
    private RaycastHit hit;

    List<float> aimValues = new List<float>();
    List<float> hipValues = new List<float>();
    private Vector3 target = new Vector3();


    private void Awake() {
    }

    void Start() {
        playerController = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();

        followAimPos = followAt.transform.localPosition;
        followHipPos = new Vector3(0f, followAt.transform.localPosition.y, followAt.transform.localPosition.z);
        lookAimPos = lookAt.transform.localPosition;
        lookHipPos = new Vector3(0f, lookAt.transform.localPosition.y, lookAt.transform.localPosition.z);

        for (int i = 0; i < freeLookCamera.m_Orbits.Length; i++) {
            float a = freeLookCamera.m_Orbits[i].m_Radius;
            aimValues.Add(a - aimValue);
            hipValues.Add(a);
        }
    }

    void Update() {
        if (nextShoot < coolDown) nextShoot += Time.deltaTime;

        //if (playerController.isAiming) {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distance)) {
            if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Player"))
                target = hit.point;
        } else {
            target = Camera.main.transform.position + Camera.main.transform.forward * distance;
        }
        collidePrefab.transform.position = target;
        //}
        //Debug.Log(Camera.main.transform.forward);
        if (playerController.isAiming && playerController.groundedPlayer) {
            HipsToShlouder();
            crossHair.SetActive(true);
            if (playerController.isShootPressed) {
                if (currentShootCharge < chargeShootLimit)
                    currentShootCharge += Time.deltaTime;
            } else if (currentShootCharge > 0f) {
                if (nextShoot >= coolDown) Shoot(target, currentShootCharge / chargeShootLimit);
                currentShootCharge = 0f;
            }
        } else {
            ShoulderToHips();
            crossHair.SetActive(false);
        }
    }

    private void OnAnimatorIK(int layerIndex) {
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, target);
        // anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        // anim.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.LookRotation(Camera.main.transform.forward));
    }

    private void Shoot(Vector3 target, float force) {
        nextShoot = 0f;
        var arrow = Instantiate(arrowPrefab, fireHole.transform.position, Quaternion.LookRotation(target));
        arrow.transform.LookAt(target);
        var arrowProyectile = arrow.GetComponent<ArrowProjectile>();
        arrowProyectile.force *= force;
        arrowProyectile.Fire();
    }

    public void HipsToShlouder() {
        lookAt.transform.localPosition = Vector3.Lerp(lookAt.transform.localPosition, lookAimPos, Time.deltaTime * aimSpeed);
        followAt.transform.localPosition = Vector3.Lerp(followAt.transform.localPosition, followAimPos, Time.deltaTime * aimSpeed);
        for (int i = 0; i < freeLookCamera.m_Orbits.Length; i++) {
            if (freeLookCamera.m_Orbits[i].m_Radius > aimValues[i]) {
                freeLookCamera.m_Orbits[i].m_Radius -= Time.deltaTime * aimSpeed;
            }
        }
    }

    public void ShoulderToHips() {
        lookAt.transform.localPosition = Vector3.Lerp(lookAt.transform.localPosition, lookHipPos, Time.deltaTime * aimSpeed);
        followAt.transform.localPosition = Vector3.Lerp(followAt.transform.localPosition, followHipPos, Time.deltaTime * aimSpeed);
        for (int i = 0; i < freeLookCamera.m_Orbits.Length; i++) {
            if (freeLookCamera.m_Orbits[i].m_Radius < hipValues[i]) {
                freeLookCamera.m_Orbits[i].m_Radius += Time.deltaTime * aimSpeed;
            }
        }
    }
}
