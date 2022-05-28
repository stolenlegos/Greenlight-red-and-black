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
    [SerializeField] private BoxCollider2D aiCollider;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        
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
}
