using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public enum AIState
{
    PATROL,
    CHASEANDSHOOT,
    SEARCH,
    DEAD
}

public class AIStateMachine : MonoBehaviour
{
    [Header("AI Components")]
    [SerializeField] private Rigidbody2D aiRB;
    [SerializeField] private CapsuleCollider2D aiCollider;
    [SerializeField] private GameObject visionCone;
    private MeshCollider visionConeCollider;
    [SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;

    [Header("AI States")]
    [SerializeField] private AIState aiState = AIState.PATROL;

    [Header("Layer References")]
    [SerializeField] private LayerMask aiLayerMask;

    [Header("Player References and Pathfinding")]
    [SerializeField] private Transform playerTarget;
    [SerializeField] private float activateDistance = 50f;
    [SerializeField] private float pathUpdateSeconds = 0.5f;

    [Header("Movement Physics")]
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float nextWaypointDistance = 3f;
    [SerializeField] private float jumpNodeHeightRequirement = 0.8f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float jumpCheckOffset = 0.1f;

    [Header("Custom Behavior")]
    [SerializeField] private bool followCheck;
    [SerializeField] private bool jumpCheck;
    [SerializeField] private bool isFacingRight;

    //ai path
    private Path path;
    private int currentWaypoint = 0;
    Seeker seeker;
    //ai collider size
    private Vector2 aiColliderSize;
    //slope check variables
    private float slopeDownAngle;
    private float slopeDownAngleOld;
    private float slopeSideAngle;
    private Vector2 slopeNormalPerpendicular;
    private bool isOnSlope;
    //ai Patrol Positions
    private Vector3 startPos;
    [SerializeField] private float patrolPath = 4f;
    private bool patrolSwitch = false;

    // Start is called before the first frame update
    void Start()
    {
        visionCone = this.gameObject.transform.GetChild(0).gameObject;
        aiColliderSize = aiCollider.size;
        visionConeCollider = visionCone.GetComponent<MeshCollider>();
        seeker = GetComponent<Seeker>();
        startPos = this.gameObject.transform.position;

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if(aiState == AIState.PATROL)
        {
            if(this.gameObject.transform.position.x >= startPos.x && !patrolSwitch)
            {
                aiRB.velocity = new Vector2(moveSpeed, aiRB.velocity.y);
                if (isFacingRight)
                {
                    FlipCharacter();
                }
                if(this.gameObject.transform.position.x >= startPos.x + (patrolPath - .5f) && !patrolSwitch)
                {
                    patrolSwitch = true;
                }
            }
            else if(this.gameObject.transform.position.x <= startPos.x + patrolPath && patrolSwitch)
            {
                aiRB.velocity = new Vector2(-moveSpeed, aiRB.velocity.y);
                if(!isFacingRight)
                {
                    FlipCharacter();
                }
                if(this.gameObject.transform.position.x <= startPos.x && patrolSwitch)
                {
                    patrolSwitch = false;
                }
            }
            else if(this.gameObject.transform.position.x <= startPos.x && !patrolSwitch)
            {
                aiRB.velocity = new Vector2(moveSpeed, aiRB.velocity.y);
                if (isFacingRight)
                {
                    FlipCharacter();
                }
            }
            if (TargetInDistance() && followCheck)
            {
                Debug.Log("ChaseandShootChange");
                aiState = AIState.CHASEANDSHOOT;
            }
        }
        else if (aiState == AIState.CHASEANDSHOOT)
        {
            Debug.Log("Chasing");
            PathFollow();
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
    private void UpdatePath()
    {
        if(followCheck && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(aiRB.position, playerTarget.position, OnPathComplete);
        }
    }
    private void PathFollow()
    {
        if(path != null)
        {
            return;
        }
        if(currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        IsGrounded();
        // direction calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - aiRB.position).normalized;
        Vector2 force = direction * moveSpeed * Time.deltaTime;
        //jump
        if (jumpCheck && IsGrounded())
        {
            if(direction.y > jumpNodeHeightRequirement)
            {
                aiRB.AddForce(Vector2.up * moveSpeed * jumpHeight);
            }
        }
        // movement
        aiRB.AddForce(force);
        //next waypoint
        float distance = Vector2.Distance(aiRB.position, path.vectorPath[currentWaypoint]);
        if(distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if(aiRB.velocity.x > 0.05f)
        {
            FlipCharacter();
        }
        else if(aiRB.velocity.x < -0.05f)
        {
            FlipCharacter();
        }
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, playerTarget.transform.position) < activateDistance;
    }
    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}
