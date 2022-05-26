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
    [SerializeField] private BoxCollider2D playerCollider;
    //playerstates
    [SerializeField] private CharacterState playerState = CharacterState.IDLE;
    //masks
    [SerializeField] private LayerMask playerLayerMask;
    //movement speeds
    private float moveSpeed = 5f;
    private float slideSpeed = 15f;
    private float diveSpeed = 8f;
    private float diveHeight = 3.5f;
    private float jumpHeight = 10f;
    private float fallSpeed = -3f;
    private float wallSlideSpeed = -2f;
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
    private bool fallCheck = false;
    private bool falling = false;
    private bool reGround = false;
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
                    StateObserver.StateChanged("JUMPING");
                }
            }
            if (playerState == CharacterState.RUNNING){
                slideSpeed = 15f;
                diveSpeed = 8f;
                playerStateChanged = false;
                playerRB.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y);
                if (!IsGrounded() && !jumpCheck && !diveCheck && !slideCheck)
                {
                    playerRB.velocity = new Vector2(moveDirection.x * (moveSpeed), fallSpeed);
                }
                else if (runCheck && diveCheck && !slideCheck && !jumpCheck){
                    playerCollider.offset = new Vector2(playerCollider.offset.x, 0.35f);
                    playerCollider.size = new Vector2(playerCollider.size.x, .3f);
                    playerRB.velocity = transform.up * diveHeight;
                    playerStateChanged = true;
                    playerState = CharacterState.DIVING;
                    StateObserver.StateChanged("DIVING");
                }
                else if (runCheck && jumpCheck && !slideCheck && !diveCheck){
                    playerStateChanged = true;
                    playerState = CharacterState.JUMPING;
                    StateObserver.StateChanged("JUMPING");
                }
                else if (slideCheck && runCheck && !jumpCheck && !diveCheck){
                    playerCollider.offset = new Vector2(playerCollider.offset.x, -0.35f);
                    playerCollider.size = new Vector2(playerCollider.size.x, .3f);
                    playerStateChanged = true;
                    playerState = CharacterState.SLIDING;
                    StateObserver.StateChanged("SLIDING");
                }
                else if (!runCheck){
                    playerStateChanged = true;
                    playerState = CharacterState.IDLE;
                    StateObserver.StateChanged("IDLE");
                }
            }
            if (playerState == CharacterState.JUMPING){
                slideSpeed = 15f;
                diveSpeed = 8f;
                playerStateChanged = false;
                Debug.Log("falling" + falling);
                Debug.Log("fallcheck" + fallCheck);
                if (runCheck && !fallCheck){
                    playerRB.velocity = new Vector2(moveDirection.x * moveSpeed, playerRB.velocity.y);
                }
                else if (!runCheck)
                {
                    playerRB.velocity = new Vector2(moveDirection.x, playerRB.velocity.y);
                }
                else if (fallCheck)
                {
                    falling = true;
                    playerRB.velocity = new Vector2(moveDirection.x, wallSlideSpeed);
                }
                else if (falling && !fallCheck)
                {
                    playerRB.velocity = new Vector2(moveDirection.x, fallSpeed); //fallSpeed);
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
                if(!IsGrounded() && slideCheck && runCheck)
                {
                    reGround = true;
                    playerRB.velocity = new Vector2(moveDirection.x * (slideSpeed), fallSpeed);
                }
                else if (IsGrounded() && reGround)
                {
                    slideCheck = false;
                    reGround = false;
                }
                else if (!slideCheck && !runCheck){
                    playerCollider.offset = new Vector2(playerCollider.offset.x, 0f);
                    playerCollider.size = new Vector2(playerCollider.size.x, 1f);
                    playerStateChanged = true;
                    playerState = CharacterState.IDLE;
                    StateObserver.StateChanged("IDLE");
                }
                else if (!slideCheck && runCheck){
                    playerCollider.offset = new Vector2(playerCollider.offset.x, 0f);
                    playerCollider.size = new Vector2(playerCollider.size.x, 1f);
                    playerStateChanged = true;
                    playerState = CharacterState.RUNNING;
                    StateObserver.StateChanged("RUNNING");
                }
            }
            if (playerState == CharacterState.DIVING){
                slideSpeed = 15f;
                playerStateChanged = false;
                playerRB.velocity = new Vector2(moveDirection.x * (diveSpeed), playerRB.velocity.y);
                IsGrounded();
            }
            //Debug.Log("onground" + IsGrounded());
            //Debug.Log("onwall" + WallCheck());
            WallCheck();
            //Debug.Log("jumpCheck" + jumpCheck);
        }
    }
    public void MoveAction(InputAction.CallbackContext ctx){
        if (ctx.started /*&& !slideCheck*/ && (playerState == CharacterState.IDLE || playerState == CharacterState.RUNNING)){
            moveDirection = ctx.ReadValue<Vector2>();
            playerStateChanged = true;
            runCheck = true;
            playerState = CharacterState.RUNNING;
            StateObserver.StateChanged("RUNNING");
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
            StateObserver.StateChanged("JUMPING");
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
            //Debug.Log(slideCheck);
        }
    }
    public void SlideOff(InputAction.CallbackContext ctx){
        if (ctx.performed){
            slideCheck = false;
            //Debug.Log(slideCheck);
        }
    }
    public void DiveOn(InputAction.CallbackContext ctx){
        if (ctx.performed){
            diveCheck = true;
            Debug.Log(diveCheck);
        }
    }
    public void DiveOff(InputAction.CallbackContext ctx){
        if (ctx.performed){
            diveCheck = false;
            Debug.Log(diveCheck);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision){
        if (collision != null){
            //Debug.Log("Collided");
            if (!IsGrounded()){

            }
            else if (IsGrounded() && playerState == CharacterState.JUMPING){
                if(falling || fallCheck)
                {
                    fallCheck = false;
                    falling = false;
                }
                if (runCheck){
                    playerStateChanged = true;
                    playerState = CharacterState.RUNNING;
                    StateObserver.StateChanged("RUNNING");
                }
                else if (!runCheck){
                    playerStateChanged = true;
                    playerState = CharacterState.IDLE;
                    StateObserver.StateChanged("IDLE");
                }
            }
            else if (IsGrounded() && playerState == CharacterState.DIVING)
            {
                diveCheck = false;
                if (runCheck)
                {
                    playerCollider.offset = new Vector2(playerCollider.offset.x, 0f);
                    playerCollider.size = new Vector2(playerCollider.size.x, 1f);
                    playerStateChanged = true;
                    playerState = CharacterState.RUNNING;
                    StateObserver.StateChanged("RUNNING");
                    slideCheck = false;
                }
                else if (!runCheck)
                {
                    playerCollider.offset = new Vector2(playerCollider.offset.x, 0f);
                    playerCollider.size = new Vector2(playerCollider.size.x, 1f);
                    playerStateChanged = true;
                    playerState = CharacterState.IDLE;
                    StateObserver.StateChanged("IDLE");
                    slideCheck = false;
                }
            }
            /*else if (WallCheck() && !IsGrounded() && playerState == CharacterState.JUMPING)
            {

            }*/
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision != null)
        {
            Debug.Log("CollidedStay");
            if (WallCheck() && !IsGrounded() && (playerState == CharacterState.JUMPING || playerState == CharacterState.DIVING))
            {
                fallCheck = true;
            }
            if(fallCheck && IsGrounded() && (playerState == CharacterState.JUMPING || playerState == CharacterState.DIVING))
            {
                fallCheck = false;
                if (runCheck)
                {
                    playerStateChanged = true;
                    playerState = CharacterState.RUNNING;
                    StateObserver.StateChanged("RUNNING");
                }
                else if (!runCheck)
                {
                    playerStateChanged = true;
                    playerState = CharacterState.IDLE;
                    StateObserver.StateChanged("IDLE");
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
        if (raycastHit.collider != null){
            //groundedForDialogue = true;
            return true;
        }
        else { return false; }
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
