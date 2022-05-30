using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour{

    [SerializeField] private Transform player;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;
    private Vector3 velocity= Vector3.one;
    private Vector3 toPos;

    //left side of screen goes in x right side of screen goes in y
    public Vector2 xBounds;

    private void Awake() {
        toPos = player.position + offset;
    }

    private void FixedUpdate() {
        if (xBounds.x < player.position.x && xBounds.y > player.position.x) {
            toPos = player.position + offset;
        }
        Vector3 smoothPos = Vector3.SmoothDamp(transform.position,toPos,ref velocity,smoothSpeed);
        transform.position = smoothPos;
    }
}
