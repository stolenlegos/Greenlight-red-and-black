using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform origin;
    [SerializeField] private Transform bulletRotationPoint;
    [SerializeField] private float bulletSpeed = 2500;

    public bool standingStill = true;
    public float strayFactor = 10;

    private int magSize = 7;
    private int currentMag;

    [SerializeField] private bool bottomlessBullets;
    [SerializeField] private bool ricochetBullets;
    [SerializeField] private bool penetratingBullets;

    private void Awake() {
        currentMag = magSize;
        bottomlessBullets = false;
        ricochetBullets = false;
        UIObserver.ObserveMag(currentMag);
    }

    private void OnEnable() {
        StateObserver.NotifyState += StateListener;
    }

    private void Update() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x,Mouse.current.position.ReadValue().y,Mathf.Abs(Camera.main.transform.position.z)));
        Vector3 aimDir = (mousePos - bulletRotationPoint.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        bulletRotationPoint.eulerAngles = new Vector3(0, 0, angle);
    }

    public void ShootBullet(InputAction.CallbackContext ctx) {
        if (ctx.performed && currentMag > 0) {
            Debug.Log("Player - Gun Noises");
            GameObject bullet = Instantiate(bulletPrefab, origin);            
            bullet.GetComponent<BulletFly>().InitializeBullet(bulletSpeed, strayFactor, standingStill, ricochetBullets, penetratingBullets);
            
            if (!bottomlessBullets) {
                currentMag--;
            }
            
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
        if (state != "IDLE") {
            standingStill = false;
        } else {
            standingStill = true;
        }
    }

    private void BonusesListener(CardEffects bonusEffect) { 
        switch (bonusEffect) {
            case CardEffects.bottomless:
                bottomlessBullets = true;
                //start bonus timers in here
                break;
            case CardEffects.ricochet:
                ricochetBullets = true;
                break;
            case CardEffects.penetrate:
                penetratingBullets = true;
                break;
            default:
                break;

        }
    }

    private void OnDisable() {
        StateObserver.NotifyState -= StateListener;
    }
}
