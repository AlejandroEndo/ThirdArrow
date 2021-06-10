using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ArrowProjectile : MonoBehaviour {

    public float force;
    public float lifeTime;
    public int damage;
    public GameObject head;

    protected CinemachineImpulseSource source;
    protected Rigidbody rb;
    protected TrailRenderer trail;

    private Transform originPos;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        trail = head.GetComponent<TrailRenderer>();
    }

    private void Start() {
    }

    void Update() {

    }

    private void FixedUpdate() {
        if (rb.velocity.magnitude > force)
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, force);

        if (Vector3.Distance(originPos.position, transform.position) > 20f) {
            OutOfRange();
        }
    }

    public virtual void Fire(Transform firepoint) {
        InitialSetup();
        originPos = firepoint;
        rb.velocity = transform.forward.normalized * force;
        // TODO: Cinemachine Impulse source
    }

    public virtual void InitialSetup() {
        //rb.isKinematic = false;
        //rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        trail.time = 0.2f;
        trail.Clear();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gameObject.SetActive(true);
    }

    protected virtual void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.CompareTag("Player")) {
            OutOfRange();
        }
    }

    public virtual void OutOfRange() {
        trail.time = 0f;
        trail.Clear();
        //rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        //rb.isKinematic = true;
        gameObject.SetActive(false);
    }

}
