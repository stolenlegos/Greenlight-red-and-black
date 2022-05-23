using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform origin;

    public void ShootBullet(InputAction.CallbackContext ctx) {
        if (ctx.performed) {
            Debug.Log("Player - Gun Noises");
            Instantiate(bullet, origin);
        }
    }
}
