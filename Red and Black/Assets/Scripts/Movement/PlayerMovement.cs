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
    //player components
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private PolygonCollider2D playerCollider;
    //playerstates
    [SerializeField] private CharacterState playerState = CharacterState.IDLE;
    //masks
    [SerializeField] private LayerMask playerLayerMask;
    //movement speeds
    private float moveSpeed = 5f;
    private float slideSpeed = 15f;
    private float diveSpeed = 7f;
    private float jumpHeight = 5f;
    //movement directions
    Vector2 moveDirection = Vector2.zero;
    Vector2 jumpDirections = Vector2.zero;
    //bools
    private bool inputsDisabled;
    private bool playerStateChanged;
    //movement bools
    private bool runCheck = false;
    private bool slideCheck = false;
    private bool diveCheck = false;
    private bool jumpCheck = false;
    private bool vaultCheck = false;
    void Update()
    {
        Debug.Log(playerState);
        if (!inputsDisabled)
        {
            
        }
    }

    private void FixedUpdate()
    {
        if (!inputsDisabled)
        {
            if(playerState == CharacterState.IDLE)
            {
                slideSpeed = 15f;
                playerStateChanged = false;
                playerRB.velocity = new Vector2(moveDirection.x, moveDirection.y);
                if (jumpCheck)
                {
                    playerStateChanged = true;
                    playerState = CharacterState.JUMPING;
                }
            }
            if (playerState == CharacterState.RUNNING)
            {
                slideSpeed = 15f;
                playerStateChanged = false;
                playerRB.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y);
                if (runCheck && jumpCheck)
                {
                    playerStateChanged = true;
                    playerState = CharacterState.JUMPING;
                }
                if (slideCheck )
                {
                    playerStateChanged = true;
                    playerState = CharacterState.SLIDING;
                }
                if (slideCheck && runCheck)
                {
                    playerStateChanged = true;
                    playerState = CharacterState.SLIDING;
                }
                else if (!runCheck)
                {
                    playerStateChanged = true;
                    playerState = CharacterState.IDLE;
                }
            }
            if (playerState == CharacterState.JUMPING)
            {
                slideSpeed = 15f;
                playerStateChanged = false;
                if (runCheck)
                {
                    //playerRB.velocity = 
                }
                //playerRB.velocity = new Vector2(moveDirection.x, moveDirection.y);
            }
            if (playerState == CharacterState.VAULTING)
            {
                slideSpeed = 15f;
                playerStateChanged = false;
                playerRB.velocity = Vector2.zero;
            }
            if (playerState == CharacterState.SLIDING)
            {
                playerStateChanged = false;
                slideSpeed = slideSpeed * .95f;
                playerRB.velocity = new Vector2(moveDirection.x * (slideSpeed), moveDirection.y);
                if (!slideCheck && !runCheck)
                {
                    playerStateChanged = true;
                    playerState = CharacterState.IDLE;
                }
                else if (!slideCheck && runCheck)
                {
                    playerStateChanged = true;
                    playerState = CharacterState.RUNNING;
                }
            }
            if (playerState == CharacterState.DIVING)
            {
                slideSpeed = 15f;
                playerStateChanged = false;
                playerRB.velocity = Vector2.zero;
            }
            Debug.Log("onground" + IsGrounded());
        }
    }
    public void MoveAction(InputAction.CallbackContext ctx)
    {
        if (ctx.started && !slideCheck && (playerState == CharacterState.IDLE || playerState == CharacterState.RUNNING))
        {
            moveDirection = ctx.ReadValue<Vector2>();
            playerStateChanged = true;
            runCheck = true;
            playerState = CharacterState.RUNNING;
        }
        else if (ctx.performed && playerState == CharacterState.RUNNING)
        {
            runCheck = false;
            moveDirection = ctx.ReadValue<Vector2>();
            playerStateChanged = true;
            playerState = CharacterState.IDLE;
        }
    }
    public void JumpAction(InputAction.CallbackContext ctx)
    {
        if (ctx.started && (playerState == CharacterState.IDLE || playerState == CharacterState.RUNNING))
        {
            playerRB.velocity = transform.up * jumpHeight;
            playerStateChanged = true;
            playerState = CharacterState.JUMPING;
            Debug.Log("Player - Jumping");
        }
    }
    public void DiveAction(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            Debug.Log("Player - Diving");
        }
    }
    public void VaultAction(InputAction.CallbackContext ctx)
    {
        if (ctx.started && (playerState == CharacterState.IDLE || playerState == CharacterState.RUNNING))
        {
            Debug.Log("Player - Vaulting");
        }
    }
    public void SlideOn(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            slideCheck = true;
            Debug.Log(slideCheck);
        }
    }
    public void SlideOff(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            slideCheck = false;
            Debug.Log(slideCheck);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision != null && IsGrounded())
        {

        }
    }
    private bool IsGrounded()
    {
        float extraHeightText = .012f;
        RaycastHit2D raycastHit = Physics2D.Raycast(playerCollider.bounds.center, Vector2.down, playerCollider.bounds.extents.y + extraHeightText, ~playerLayerMask);
        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(playerCollider.bounds.center, Vector2.down * (playerCollider.bounds.extents.y + extraHeightText));
        //Debug.Log(raycastHit.collider);
        if (raycastHit.collider != null)
        {
            //groundedForDialogue = true;
            return true;
        }
        else { return false; }
        //return raycastHit.collider != null;
    }

}
