using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFly : MonoBehaviour
{
    private Vector2 direction;
    private float forceMult;

    public void InitializeBullet(Vector2 dir, float force) {
        direction = dir;
        forceMult = force;
        GetComponent<Rigidbody2D>().AddForce(direction * forceMult);
        StartCoroutine("TimeOut");
    }

    private IEnumerator TimeOut() {
        yield return new WaitForSeconds(1);
        GameObject.Destroy(gameObject);        
    }
}
