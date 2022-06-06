using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIState
{
    PATROL,
    CHASEANDSHOOT,
    SEARCH,
    DEAD
}

public class AIStateMachine : MonoBehaviour
{
    //AI components
    [SerializeField] private Rigidbody2D aiRB;
    [SerializeField] private CapsuleCollider2D aiCollider;
    [SerializeField] private GameObject visionCone;
    private MeshCollider visionConeCollider;
    [SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;
    //AIstates
    [SerializeField] private AIState aiState = AIState.PATROL;
    //masks
    [SerializeField] private LayerMask aiLayerMask;
    //player references
    [SerializeField] private Transform playerTarget;
    //ai movement variables
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float asdf;
    //ai path
    
    //ai bools
    private bool isFacingRight;
    //ai collider size
    private Vector2 aiColliderSize;
    //slope check variables
    private float slopeDownAngle;
    private float slopeDownAngleOld;
    private float slopeSideAngle;
    private Vector2 slopeNormalPerpendicular;
    private bool isOnSlope;

    // Start is called before the first frame update
    void Start()
    {
        aiColliderSize = aiCollider.size;
        visionConeCollider = visionCone.GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if(aiState == AIState.PATROL)
        {
            //aiRB.velocity = ()
        }
        else if (aiState == AIState.CHASEANDSHOOT)
        {

        }
        else if (aiState == AIState.SEARCH)
        {

        }
        else if (aiState == AIState.DEAD)
        {

        }
    }
    private void FlipCharacter()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
    private bool IsGrounded()
    {
        float extraHeightText = .02f;
        RaycastHit2D raycastHit = Physics2D.Raycast(aiCollider.bounds.center, Vector2.down, aiCollider.bounds.extents.y + extraHeightText, ~aiLayerMask);
        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(aiCollider.bounds.center, Vector2.down * (aiCollider.bounds.extents.y + extraHeightText));
        if (raycastHit.collider != null)
        {
            //groundedForDialogue = true;
            return true;
        }
        else { return false; }
    }
    private bool WallCheck()
    {
        float extraHeightText = .02f;
        if (!isFacingRight)
        {
            RaycastHit2D raycastHit = Physics2D.Raycast(aiCollider.bounds.center, Vector2.right, aiCollider.bounds.extents.y + extraHeightText, ~aiLayerMask);
            Debug.DrawRay(aiCollider.bounds.center, Vector2.right * (aiCollider.bounds.extents.y + extraHeightText));
            if (raycastHit.collider != null)
            {
                return true;
            }
            else { return false; }
        }
        else if (isFacingRight)
        {
            RaycastHit2D raycastHit = Physics2D.Raycast(aiCollider.bounds.center, Vector2.left, aiCollider.bounds.extents.y + extraHeightText, ~aiLayerMask);
            Debug.DrawRay(aiCollider.bounds.center, Vector2.left * (aiCollider.bounds.extents.y + extraHeightText));
            if (raycastHit.collider != null)
            {
                return true;
            }
            else { return false; }
        }
        else { return false; }

    }
    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0.0f, aiColliderSize.y / 2);
        HorizontalSlopeCheck(checkPos);
        VerticalSlopeCheck(checkPos);
    }
    private void HorizontalSlopeCheck(Vector2 checkPos)
    {
        float slopeCheckDistance = 0.5f;
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, ~aiLayerMask);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, ~aiLayerMask);

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
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopCheckDistance, ~aiLayerMask);
        if (hit)
        {
            slopeNormalPerpendicular = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }

            slopeDownAngleOld = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPerpendicular, Color.black);
            Debug.DrawRay(hit.point, hit.normal, Color.blue);
        }
        if (isOnSlope && aiState == AIState.DEAD)
        {
            aiRB.sharedMaterial = fullFriction;
        }
        else
        {
            aiRB.sharedMaterial = noFriction;
        }
    }
}
