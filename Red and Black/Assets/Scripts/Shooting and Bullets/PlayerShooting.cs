using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform origin;
    [SerializeField] private float bulletSpeed;

    public void ShootBullet(InputAction.CallbackContext ctx) {
        if (ctx.performed) {
            Debug.Log("Player - Gun Noises");
            GameObject bullet = Instantiate(bulletPrefab, origin);

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3 (Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, Mathf.Abs(Camera.main.transform.position.z)));
            Vector3 dir = mousePos - origin.transform.position;
            Vector2 dir2D = new Vector2(dir.x,dir.y);
            
            bullet.GetComponent<BulletFly>().InitializeBullet(dir2D.normalized, bulletSpeed);
        }
    }
}
