using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ArrowProjectile : MonoBehaviour {

    public float force;
    public float lifeTime;
    protected CinemachineImpulseSource source;
    protected Rigidbody rb;
    private void Awake() {
        rb = GetComponent<Rigidbody>();
        // rb.centerOfMass = transform.position;
    }

    private void Start() {
        force = 100 * Random.Range(1.3f, 1.7f);
    }

    void Update() {
    }

    public virtual void Fire() {
        rb.AddForce(transform.forward * force, ForceMode.Impulse);
        transform.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y, 0f));
        // TODO: Cinemachine Impulse source
    }

    public virtual void InitialSetup() {
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.isKinematic = false;
        transform.parent = null;
    }

    protected virtual void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag != "Player") {
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rb.isKinematic = true;
            transform.parent = collision.gameObject.transform;
            StartCoroutine(Countdown());
        }
    }

    IEnumerator Countdown() {
        yield return new WaitForSeconds(lifeTime);
    }
}
