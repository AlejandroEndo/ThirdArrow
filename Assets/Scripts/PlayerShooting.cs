using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerShooting : MonoBehaviour {
    //[SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] private GameObject collidePrefab;
    [SerializeField] private GameObject fireHole;
    private PlayerController playerController;


    [SerializeField] private float aimValue;
    [SerializeField] private float speed;
    [SerializeField] private bool aiming = false;
    [Range(0.1f, 2f)]
    [SerializeField] private float coolDown;
    private float nextShoot = 0f;
    RaycastHit hit;

    List<float> aimValues = new List<float>();
    List<float> hipValues = new List<float>();


    private void Awake() {
    }

    void Start() {
        playerController = GetComponent<PlayerController>();

        for(int i = 0; i < freeLookCamera.m_Orbits.Length; i++) {
            float a = freeLookCamera.m_Orbits[i].m_Radius;
            aimValues.Add(a - aimValue);
            hipValues.Add(a);
        }
    }

    void Update() {
        if (nextShoot < coolDown) nextShoot += Time.deltaTime;
        if (playerController.isShootPressed && playerController.isAiming && nextShoot >= coolDown) {
            nextShoot = 0f;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100.0f)) {
                Vector3 dis = hit.point - fireHole.transform.position;
                
                var arrow = Instantiate(collidePrefab, fireHole.transform.position, Quaternion.LookRotation(dis));
                arrow.GetComponent<ArrowProjectile>().Fire();
            }
        }
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
