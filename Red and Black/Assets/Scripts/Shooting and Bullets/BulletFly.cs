using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFly : MonoBehaviour {
    public void InitializeBullet(float force, float stray, bool standingStill) {
        float forceMult = force;

        if (!standingStill) {
            float randomZ = Random.Range(stray * -1,stray);
            gameObject.transform.Rotate(0, 0, randomZ);
        }

        GetComponent<Rigidbody2D>().AddForce(transform.right * forceMult);
        StartCoroutine("TimeOut");
    }

    private IEnumerator TimeOut() {
        yield return new WaitForSeconds(1);
        GameObject.Destroy(gameObject);        
    }
}
