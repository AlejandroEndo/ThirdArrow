using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RighHandArrowFx : MonoBehaviour {
    [SerializeField] private Transform fireHole;
    void Update() { transform.LookAt(fireHole.position); }
}
