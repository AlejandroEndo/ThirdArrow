using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerShooting : MonoBehaviour {
    //[SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private CinemachineFreeLook freeLookCamera;

    [SerializeField] private float aimValue;
    [SerializeField] private float hipValue;

    void Start() {
    }

    void Update() {
        //freeLookCamera.m_Lens.FieldOfView = 10;

    }

    public void HipsToShlouder() {
        freeLookCamera.m_Lens.FieldOfView = Mathf.Lerp(freeLookCamera.m_Lens.FieldOfView, aimValue, Time.deltaTime);
        //cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, aimValue, Time.deltaTime);
    }

    public void ShoulderToHips() {
        freeLookCamera.m_Lens.FieldOfView = Mathf.Lerp(freeLookCamera.m_Lens.FieldOfView, hipValue, Time.deltaTime);
        //cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, hipValue, Time.deltaTime);
    }
}
