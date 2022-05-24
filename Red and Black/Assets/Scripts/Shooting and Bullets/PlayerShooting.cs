using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform origin;
    [SerializeField] private float bulletSpeed = 2500;

    public bool standingStill = true;
    public float strayFactor = 10;

    private int magSize = 7;
    private int currentMag;

    private void Awake() {
        currentMag = magSize;
        UIObserver.ObserveMag(currentMag);
    }

    private void OnEnable() {
        StateObserver.NotifyState += StateListener;
    }

    private void Update() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x,Mouse.current.position.ReadValue().y,Mathf.Abs(Camera.main.transform.position.z)));
        Vector3 aimDir = (mousePos - origin.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        origin.eulerAngles = new Vector3(0, 0, angle);
    }

    public void ShootBullet(InputAction.CallbackContext ctx) {
        if (ctx.performed && currentMag > 0) {
            Debug.Log("Player - Gun Noises");
            GameObject bullet = Instantiate(bulletPrefab, origin);            
            bullet.GetComponent<BulletFly>().InitializeBullet(bulletSpeed, strayFactor, standingStill);
            currentMag--;
            UIObserver.ObserveMag(currentMag);
        }
    }

    public void Reload(InputAction.CallbackContext ctx) {
        if (ctx.performed) {
            Debug.Log("Reloading");
            currentMag = magSize;
            UIObserver.ObserveMag(currentMag);
        }
    }

    private void StateListener(string state) {
        if (state != "Idle") {
            standingStill = false;
        } else {
            standingStill = true;
        }
    }

    private void OnDisable() {
        StateObserver.NotifyState -= StateListener;
    }
}
