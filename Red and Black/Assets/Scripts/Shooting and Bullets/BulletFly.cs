using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFly : MonoBehaviour
{
    private Vector2 direction;
    private Vector2 forceMult;

    private void Start() {
        GetComponent<Rigidbody2D>().AddForce(direction * forceMult);
    }

    public void InitializeBullet(Vector2 dir, Vector2 force) {
        direction = dir;
        forceMult = force;
    }
}
