using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerShooting : MonoBehaviour {
    //[SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject collidePrefab;
    [SerializeField] private GameObject fireHole;
    private PlayerController playerController;


    [SerializeField] private float aimValue;
    [Range(1f, 100f)]
    [SerializeField] private float distance;
    [SerializeField] private float speed;
    [Range(0.1f, 2f)]
    [SerializeField] private float coolDown;
    private float nextShoot = 0f;
    [Range(0.5f, 3f)]
    [SerializeField] private float chargeShootLimit;
    public float currentShootCharge = 0f;
    RaycastHit hit;

    List<float> aimValues = new List<float>();
    List<float> hipValues = new List<float>();


    private void Awake() {
    }

    void Start() {
        playerController = GetComponent<PlayerController>();

        for (int i = 0; i < freeLookCamera.m_Orbits.Length; i++) {
            float a = freeLookCamera.m_Orbits[i].m_Radius;
            aimValues.Add(a - aimValue);
            hipValues.Add(a);
        }
    }

    void Update() {
        if (nextShoot < coolDown) nextShoot += Time.deltaTime;

        if (playerController.isAiming) {
            bool raycastHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distance);
            Vector3 target = raycastHit ? hit.point : Camera.main.transform.forward * distance;
            if (playerController.isShootPressed) {
                if (currentShootCharge < chargeShootLimit)
                    currentShootCharge += Time.deltaTime;
            } else if (currentShootCharge > 0f) {
                if (nextShoot >= coolDown) Shoot(target, currentShootCharge);
                currentShootCharge = 0f;
            }
        }
    }

    private void Shoot(Vector3 target, float force) {
        nextShoot = 0f;
        var arrow = Instantiate(arrowPrefab, fireHole.transform.position, Quaternion.LookRotation(target));
        var arrowProyectile = arrow.GetComponent<ArrowProjectile>();
        arrowProyectile.force *= force;
        arrowProyectile.Fire();
    }

    public void HipsToShlouder() {
        for (int i = 0; i < freeLookCamera.m_Orbits.Length; i++) {
            freeLookCamera.m_Orbits[i].m_Radius = aimValues[i];
        }
    }

    public void ShoulderToHips() {
        for (int i = 0; i < freeLookCamera.m_Orbits.Length; i++) {
            freeLookCamera.m_Orbits[i].m_Radius = hipValues[i];
        }
    }
}
