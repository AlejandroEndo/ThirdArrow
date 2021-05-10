using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerShooting : MonoBehaviour {
    //[SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private CinemachineFreeLook freeLookCamera;

    [SerializeField] private float aimValue;
    [SerializeField] private float speed;
    private float hipValue;
    [SerializeField] private bool aiming = false;

    List<float> aimValues = new List<float>();
    List<float> hipValues = new List<float>();
    private void Awake() {
    }

    void Start() {
        hipValue = freeLookCamera.m_Lens.FieldOfView;
        for(int i = 0; i < freeLookCamera.m_Orbits.Length; i++) {
            float a = freeLookCamera.m_Orbits[i].m_Radius;
            aimValues.Add(a - aimValue);
            hipValues.Add(a);
        }
    }

    void Update() {
        if (aiming) {
        } else {
        }
    }

    public void HipsToShlouder() {
        //freeLookCamera.m_Lens.FieldOfView = hipValue;
        /*
        aiming = true;
            if (freeLookCamera.m_Lens.FieldOfView > aimValue)
                freeLookCamera.m_Lens.FieldOfView = Mathf.Lerp(freeLookCamera.m_Lens.FieldOfView, aimValue, speed);
            */
        for (int i = 0; i < freeLookCamera.m_Orbits.Length; i++) {
            freeLookCamera.m_Orbits[i].m_Radius = aimValues[i];
        }
    }

    public void ShoulderToHips() {
        /*
        aiming = false;
            if (freeLookCamera.m_Lens.FieldOfView < hipValue)
                freeLookCamera.m_Lens.FieldOfView = Mathf.Lerp(freeLookCamera.m_Lens.FieldOfView, hipValue, speed);
        //freeLookCamera.m_Lens.FieldOfView = aimValue;
        */
        for (int i = 0; i < freeLookCamera.m_Orbits.Length; i++) {
            freeLookCamera.m_Orbits[i].m_Radius = hipValues[i];
        }
    }
}
