using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform origin;

    public void ShootBullet(InputAction.CallbackContext ctx) {
        if (ctx.performed) {
            Debug.Log("Player - Gun Noises");
            GameObject bullet = Instantiate(bulletPrefab, origin);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3 (Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, Mathf.Abs(transform.position.z)));

        }
    }
}
