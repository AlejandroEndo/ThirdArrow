using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour {
    public float force;
    public Rigidbody rb;
    Cinemachine.CinemachineImpulseSource source;
    private void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = transform.position;
    }

    void Update() {
        if (!rb.isKinematic && rb.velocity.magnitude > 0.1f)
            transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
    }

    public void Fire() {
        rb.AddForce(transform.forward * (100 * Random.Range(1.3f, 1.7f) * force), ForceMode.Impulse);
        //source = GetComponent<Cinemachine.CinemachineImpulseSource>();

        //source.GenerateImpulse(Camera.main.transform.forward);
    }

    public void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.name != "Player") {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = true;
            StartCoroutine(Countdown());
        }
    }

    IEnumerator Countdown() {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }


}
