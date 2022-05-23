using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BulletFly : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 mousePos;
    private Vector3 originPosition;
    private Vector2 forceDir;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        mousePos = Mouse.current.position.ReadValue().normalized;
        originPosition = transform.position;
        forceDir = mousePos - new Vector2 (originPosition.x, originPosition.y);
    }

    private void Start() {
        rb.AddForce(forceDir * 2);
    }
}
