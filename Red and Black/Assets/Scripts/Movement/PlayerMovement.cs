using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CharacterState
{
    IDLE,
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
    [SerializeField] private CharacterState playerState = CharacterState.IDLE;
    
    //movement speeds
    private float moveSpeed = 5f;
    private float slideSpeed = 7f;
    private float diveSpeed = 7f;
    private float jumpHeight = 5f;
    //movement directions
    Vector2 moveDirection = Vector2.zero;
    Vector2 jumpDirections = Vector2.zero;
    //bools
    private bool inputsDisabled;
    private bool playerStateChanged;
    void Update()
    {
        if (!inputsDisabled)
        {
            //if(playerState )
        }
    }

    private void FixedUpdate()
    {
        if (!inputsDisabled)
        {
            if(playerState == CharacterState.IDLE)
            {
                playerRB.velocity = new Vector2(moveDirection.x, moveDirection.y);
            }
            if (playerState == CharacterState.RUNNING)
            {
                playerStateChanged = false;
                playerRB.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y);
            }
            if (playerState == CharacterState.JUMPING)
            {
                playerRB.velocity = Vector2.zero;
            }
            if (playerState == CharacterState.VAULTING)
            {
                playerRB.velocity = Vector2.zero;
            }
            if (playerState == CharacterState.SLIDING)
            {
                playerRB.velocity = Vector2.zero;
            }
            if (playerState == CharacterState.DIVING)
            {

            }
        }
        //playerRB.velocity = Vector2.zero;
    }
    public void MoveAction(InputAction.CallbackContext ctx)
    {
        if (ctx.started && (playerState == CharacterState.IDLE || playerState == CharacterState.RUNNING)) //|| CharacterState))
        {
            moveDirection = ctx.ReadValue<Vector2>();
            playerStateChanged = true;
            playerState = CharacterState.RUNNING;
            Debug.Log("Player - Moving");
        }
        if (ctx.performed)
        {
            moveDirection = ctx.ReadValue<Vector2>();
            playerStateChanged = true;
            playerState = CharacterState.IDLE;
        }
    }
    public void JumpAction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Player - Jumping");
        }
    }
    public void SlideAction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Player - Sliding");
        }
    }
    public void DiveAction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Player - Diving");
        }
    }
    public void VaultAction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && (playerState == CharacterState.IDLE || playerState == CharacterState.RUNNING))
        {
            Debug.Log("Player - Vaulting");
        }
    }
    
}
