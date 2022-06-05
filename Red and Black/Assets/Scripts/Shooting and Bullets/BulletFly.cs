using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFly : MonoBehaviour {
    private Vector3 lastVel;
    private Rigidbody2D rb;
    private bool ricochetActive;
    private bool penetrateActive;
    public void InitializeBullet(float force, float stray, bool standingStill, bool ricochet, bool penetrate) {
        float forceMult = force;
        ricochetActive = ricochet;
        penetrateActive = penetrate;

        if (penetrateActive) {
            GameObject[] environmentObjs = GameObject.FindGameObjectsWithTag("Environment");
            foreach (GameObject obj in environmentObjs) {
                Physics2D.IgnoreCollision(obj.GetComponent<Collider2D>(),GetComponent<Collider2D>());
            }
        }

        if (!standingStill) {
            float randomZ = Random.Range(stray * -1,stray);
            gameObject.transform.Rotate(0, 0, randomZ);
        }

        transform.parent = null;
        transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * forceMult);
        StartCoroutine("TimeOut");
    }

    private void Update() {
        lastVel = rb.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (ricochetActive && collision.collider.tag != "Player" && collision.collider.tag != "Enemy") {
            var speed = lastVel.magnitude;
            var direction = Vector3.Reflect(lastVel.normalized,collision.contacts[0].normal);
            rb.velocity = direction * Mathf.Max(speed,0f);

        } else if (!penetrateActive) {
            GameObject.Destroy(gameObject);
        }
    }

    private IEnumerator TimeOut() {
        yield return new WaitForSeconds(1);
        GameObject.Destroy(gameObject);        
    }
}
