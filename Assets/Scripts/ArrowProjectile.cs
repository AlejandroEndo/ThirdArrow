using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour {
    public float force;
    public Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private GameObject player;
    Cinemachine.CinemachineImpulseSource source;
    [SerializeField] private float lifeTime;
    [SerializeField] private float distance;
    private void Awake() {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb.centerOfMass = transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update() {
        if (!rb.isKinematic && rb.velocity.magnitude > 0.1f)
            transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);

        if(Vector3.Distance(transform.position, player.transform.position) > distance && !rb.isKinematic) {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = true;
            gameObject.SetActive(false);
        }
    }

    public void Fire() {
        if (rb.isKinematic) {
            rb.isKinematic = false;
        }
        rb.AddForce(transform.forward * (100 * Random.Range(1.3f, 1.7f) * force), ForceMode.Impulse);
    }

    public void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Shootable") {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = true;
            capsuleCollider.isTrigger = true;
            StartCoroutine(Countdown());
        }
    }

    IEnumerator Countdown() {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }


}
