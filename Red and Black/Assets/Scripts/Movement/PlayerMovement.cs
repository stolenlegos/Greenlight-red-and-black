using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CharacterState
{
    IDLE,
    WALKING,
    RUNNING,
    JUMPING,
    VAULTING,
    SLIDING,
    DIVING,
    DEAD
}

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private InputAction playerActions;
    //movements
    private Vector2 movementDirection = Vector2.zero;
    //movement speeds
    private float moveSpeed;
    private float slideSpeed;
    private float diveSpeed;
    private float jumpHeight;
    //bools
    private bool inputsDisabled;

    private void OnEnable()
    {
        playerActions.Enable();
    }
    private void OnDisable()
    {
        playerActions.Disable();
    }
    void Start()
    {
        
    }
    void Update()
    {
        movementDirection = playerActions.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        playerRB.velocity = Vector2.zero;
    }
}
