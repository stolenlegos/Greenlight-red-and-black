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
    private float diveSpeed = 8f;
    private float diveHeight = 3.5f;
    private float jumpHeight = 5f;
    //movement directions
    Vector2 moveDirection = Vector2.zero;
    Vector2 jumpDirections = Vector2.zero;
    //player bools
    private bool inputsDisabled;
    private bool playerStateChanged;
    private bool isFacingRight;
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
        if (!isFacingRight && moveDirection.x < 0f)
        {
            FlipCharacter();
        }
        else if (isFacingRight && moveDirection.x > 0f)
        {
            FlipCharacter();
        }
    }

    private void FixedUpdate()
    {
        if (!inputsDisabled){
            if(playerState == CharacterState.IDLE){
                slideSpeed = 15f;
                diveSpeed = 8f;
                playerStateChanged = false;
                playerRB.velocity = new Vector2(moveDirection.x, moveDirection.y);
                if (jumpCheck){
                    playerStateChanged = true;
                    playerState = CharacterState.JUMPING;
                }
            }
            if (playerState == CharacterState.RUNNING){
                slideSpeed = 15f;
                diveSpeed = 8f;
                playerStateChanged = false;
                playerRB.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y);
                if (runCheck && slideCheck && jumpCheck){
                    playerStateChanged = true;
                    playerState = CharacterState.DIVING;
                }
                else if (runCheck && jumpCheck && !slideCheck){
                    playerStateChanged = true;
                    playerState = CharacterState.JUMPING;
                }
                else if (slideCheck && runCheck && !jumpCheck){
                    playerStateChanged = true;
                    playerState = CharacterState.SLIDING;
                }
                else if (!runCheck){
                    playerStateChanged = true;
                    playerState = CharacterState.IDLE;
                }
            }
            if (playerState == CharacterState.JUMPING){
                slideSpeed = 15f;
                diveSpeed = 8f;
                playerStateChanged = false;
                if (runCheck){
                    playerRB.velocity = new Vector2(moveDirection.x * moveSpeed, playerRB.velocity.y);
                }
                else if (!runCheck)
                {
                    playerRB.velocity = new Vector2(moveDirection.x, playerRB.velocity.y);
                }
            }
            if (playerState == CharacterState.VAULTING){
                slideSpeed = 15f;
                diveSpeed = 8f;
                playerStateChanged = false;
                playerRB.velocity = Vector2.zero;
            }
            if (playerState == CharacterState.SLIDING){
                diveSpeed = 8f;
                playerStateChanged = false;
                slideSpeed = slideSpeed * .95f;
                playerRB.velocity = new Vector2(moveDirection.x * (slideSpeed), moveDirection.y);
                if (!slideCheck && !runCheck){
                    playerStateChanged = true;
                    playerState = CharacterState.IDLE;
                }
                else if (!slideCheck && runCheck){
                    playerStateChanged = true;
                    playerState = CharacterState.RUNNING;
                }
            }
            if (playerState == CharacterState.DIVING){
                slideSpeed = 15f;
                playerStateChanged = false;
                playerRB.velocity = new Vector2(moveDirection.x * (diveSpeed), playerRB.velocity.y);
            }
            Debug.Log("onground" + IsGrounded());
            WallCheck();
        }
    }
    public void MoveAction(InputAction.CallbackContext ctx){
        if (ctx.started /*&& !slideCheck*/ && (playerState == CharacterState.IDLE || playerState == CharacterState.RUNNING)){
            moveDirection = ctx.ReadValue<Vector2>();
            playerStateChanged = true;
            runCheck = true;
            playerState = CharacterState.RUNNING;
        }
        else if (ctx.started && playerState == CharacterState.JUMPING){
            runCheck = true;
            moveDirection = ctx.ReadValue<Vector2>();
        }
        else if (ctx.performed && (playerState == CharacterState.RUNNING || playerState == CharacterState.JUMPING || playerState == CharacterState.DIVING || playerState == CharacterState.SLIDING)){
            runCheck = false;
            moveDirection = ctx.ReadValue<Vector2>();
        }
    }
    public void JumpAction(InputAction.CallbackContext ctx){
        if (ctx.started && (playerState == CharacterState.IDLE || playerState == CharacterState.RUNNING)){
            playerRB.velocity = transform.up * jumpHeight;
            playerStateChanged = true;
            playerState = CharacterState.JUMPING;
            Debug.Log("Player - Jumping");
        }
        else if (ctx.started && (playerState == CharacterState.SLIDING))
        {
            playerRB.velocity = transform.up * diveHeight;
            playerStateChanged = true;
            playerState = CharacterState.DIVING;
        }
    }
    public void VaultAction(InputAction.CallbackContext ctx){
        if (ctx.started && (playerState == CharacterState.IDLE || playerState == CharacterState.RUNNING)){
            Debug.Log("Player - Vaulting");
        }
    }
    public void SlideOn(InputAction.CallbackContext ctx){
        if (ctx.performed){
            slideCheck = true;
            Debug.Log(slideCheck);
        }
    }
    public void SlideOff(InputAction.CallbackContext ctx){
        if (ctx.performed){
            slideCheck = false;
            Debug.Log(slideCheck);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision){
        if (collision != null){
            Debug.Log("Collided");
            if (!IsGrounded()){

            }
            else if (IsGrounded() && playerState == CharacterState.JUMPING){
                if (runCheck){
                    playerStateChanged = true;
                    playerState = CharacterState.RUNNING;
                }
                else if (!runCheck){
                    playerStateChanged = true;
                    playerState = CharacterState.IDLE;
                }
            }
            else if (IsGrounded() && playerState == CharacterState.DIVING)
            {
                if (runCheck)
                {
                    playerStateChanged = true;
                    playerState = CharacterState.RUNNING;
                    slideCheck = false;
                }
                else if (!runCheck)
                {
                    playerStateChanged = true;
                    playerState = CharacterState.IDLE;
                    slideCheck = false;
                }
            }
        }
    }
    private void FlipCharacter()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
    private bool IsGrounded(){
        float extraHeightText = .02f;
        RaycastHit2D raycastHit = Physics2D.Raycast(playerCollider.bounds.center, Vector2.down, playerCollider.bounds.extents.y + extraHeightText, ~playerLayerMask);
        Color rayColor;
        if (raycastHit.collider != null){
            rayColor = Color.green;
        }
        else{
            rayColor = Color.red;
        }
        Debug.DrawRay(playerCollider.bounds.center, Vector2.down * (playerCollider.bounds.extents.y + extraHeightText));
        //Debug.Log(raycastHit.collider);
        if (raycastHit.collider != null){
            //groundedForDialogue = true;
            return true;
        }
        else { return false; }
        //return raycastHit.collider != null;
    }
    private bool WallCheck(){
        float extraHeightText = .02f;
        if (!isFacingRight){
            RaycastHit2D raycastHit = Physics2D.Raycast(playerCollider.bounds.center, Vector2.right, playerCollider.bounds.extents.y + extraHeightText, ~playerLayerMask);
            Debug.DrawRay(playerCollider.bounds.center, Vector2.right * (playerCollider.bounds.extents.y + extraHeightText));
            if (raycastHit.collider != null){
                return true;
            }
            else { return false; }
        }
        else if (isFacingRight){
            RaycastHit2D raycastHit = Physics2D.Raycast(playerCollider.bounds.center, Vector2.left, playerCollider.bounds.extents.y + extraHeightText, ~playerLayerMask);
            Debug.DrawRay(playerCollider.bounds.center, Vector2.left * (playerCollider.bounds.extents.y + extraHeightText));
            if (raycastHit.collider != null){
                return true;
            }
            else { return false; }
        }
        else { return false; }
        
    }

}
