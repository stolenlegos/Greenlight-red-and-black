using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform origin;

    void Start() {
        
    }

    void Update() {
        
    }

    public void ShootBullet(InputAction.CallbackContext ctx) {
        if (ctx.performed) {
            Debug.Log("Player - Gun Noises");
            Instantiate(bullet,origin);
        }
    }
}
