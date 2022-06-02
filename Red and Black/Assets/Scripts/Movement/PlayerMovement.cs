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
    [SerializeField] private CapsuleCollider2D playerCollider;
    [SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;
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
    private float fallSpeed = -5f;
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
    //player vectors
    private Vector2 colliderSize;
    //slope check variables
    private float slopeDownAngle;
    private float slopeDownAngleOld;
    private float slopeSideAngle;
    private Vector2 slopeNormalPerpendicular;
    private bool isOnSlope;

    private void Start()
    {
        colliderSize = playerCollider.size;
    }

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
                playerRB.velocity = new Vector2(moveDirection.x, fallSpeed);
                if (jumpCheck){
                    playerStateChanged = true;
                    playerState = CharacterState.JUMPING;
                    StateObserver.StateChanged("JUMPING");
                }
                else if (!IsGrounded())
                {
                    playerRB.velocity = new Vector2(moveDirection.x, fallSpeed);
                }
            }
            if (playerState == CharacterState.RUNNING){
                slideSpeed = 15f;
                diveSpeed = 8f;
                falling = false;
                playerStateChanged = false;
                playerRB.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y);
                if (!IsGrounded() && !jumpCheck && !diveCheck && !slideCheck && runCheck && isOnSlope)
                {
                    playerRB.velocity = new Vector2(moveSpeed * slopeNormalPerpendicular.x * -moveDirection.x, moveSpeed * slopeNormalPerpendicular.y * -moveDirection.x);
                }
                else if (!IsGrounded() && !jumpCheck && !diveCheck && !slideCheck && !isOnSlope)
                {
                    playerRB.velocity = new Vector2(moveDirection.x * (moveSpeed), fallSpeed);
                }
                else if (runCheck && diveCheck && !slideCheck && !jumpCheck){
                    playerCollider.offset = new Vector2(playerCollider.offset.x, 0.25f);
                    playerCollider.size = new Vector2(playerCollider.size.x, .5f);
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
                    playerCollider.offset = new Vector2(playerCollider.offset.x, -0.25f);
                    playerCollider.size = new Vector2(playerCollider.size.x, .5f);
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
                //Debug.Log("fallcheck" + fallCheck);
                if (runCheck && !fallCheck){
                    playerRB.velocity = new Vector2(moveDirection.x * moveSpeed, playerRB.velocity.y);
                    //Debug.Log("Running1");
                }
                else if (!runCheck)
                {
                    playerRB.velocity = new Vector2(moveDirection.x, playerRB.velocity.y);
                    //Debug.Log("Running2");
                }
                else if (fallCheck && !falling)
                {
                    falling = true;
                    playerRB.velocity = new Vector2(moveDirection.x, wallSlideSpeed);
                    //Debug.Log("Running3");
                }
                else if (falling && fallCheck)
                {
                    playerRB.velocity = new Vector2(moveDirection.x * moveSpeed, playerRB.velocity.y);
                    //Debug.Log("Running4");
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
            WallCheck();
            SlopeCheck();
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
            //Debug.Log(diveCheck);
        }
    }
    public void DiveOff(InputAction.CallbackContext ctx){
        if (ctx.performed){
            diveCheck = false;
            //Debug.Log(diveCheck);
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
        float extraHeightText = .05f;
        /*if (isOnSlope)
        {
            extraHeightText = .3f;
        }
        else
        {
            extraHeightText = .05f;
        }*/
        if (playerState == CharacterState.DIVING)
        {
            RaycastHit2D divecastHit1 = Physics2D.Raycast(playerCollider.bounds.center, Vector2.down, playerCollider.bounds.extents.y + extraHeightText, ~playerLayerMask);
            RaycastHit2D divecastHit2 = Physics2D.Raycast(playerCollider.bounds.center, Vector2.down, playerCollider.bounds.extents.y + extraHeightText, ~playerLayerMask);
            RaycastHit2D divecastHit3 = Physics2D.Raycast(playerCollider.bounds.center, Vector2.down, playerCollider.bounds.extents.y + extraHeightText, ~playerLayerMask);
            Color rayColor;
            if (divecastHit1.collider != null || divecastHit2.collider != null || divecastHit3.collider != null)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }
            Debug.DrawRay(playerCollider.bounds.center, Vector2.down * (playerCollider.bounds.extents.y + extraHeightText));
            if (divecastHit1.collider != null || divecastHit2.collider != null || divecastHit3.collider != null)
            {
                //groundedForDialogue = true;
                return true;
            }
            else { return false; }
        }
        else {
            RaycastHit2D raycastHit = Physics2D.Raycast(playerCollider.bounds.center, Vector2.down, playerCollider.bounds.extents.y + extraHeightText, ~playerLayerMask);
            Color rayColor;
            if (raycastHit.collider != null) {
                rayColor = Color.green;
            }
            else {
                rayColor = Color.red;
            }
            Debug.DrawRay(playerCollider.bounds.center, Vector2.down * (playerCollider.bounds.extents.y + extraHeightText));
            if (raycastHit.collider != null) {
                //groundedForDialogue = true;
                return true;
            }
            else { return false; }
        }

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

    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0.0f, colliderSize.y / 2);
        HorizontalSlopeCheck(checkPos);
        VerticalSlopeCheck(checkPos);
    }
    private void HorizontalSlopeCheck(Vector2 checkPos)
    {
        float slopeCheckDistance = 0.5f;
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, ~playerLayerMask);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, ~playerLayerMask);

        if (slopeHitFront)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }
    }
    private void VerticalSlopeCheck(Vector2 checkPos)
    {
        float slopCheckDistance = 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopCheckDistance, ~playerLayerMask);
        if (hit)
        {
            slopeNormalPerpendicular = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if(slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }

            slopeDownAngleOld = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPerpendicular, Color.black);
            Debug.DrawRay(hit.point, hit.normal, Color.blue);
        }
        if(isOnSlope && playerState == CharacterState.IDLE)
        {
            playerRB.sharedMaterial = fullFriction;
        }
        else
        {
            playerRB.sharedMaterial = noFriction;
        }
    }

}
