using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumbEnemyAttack : MonoBehaviour
{
    [Header("For Petrolling")]
    [SerializeField] float moveSpeed;
    private float moveDiretion =1;
    private bool facingRight = true;
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] Transform wallCheckPoint;
    [SerializeField] float circleRadius;
    [SerializeField] LayerMask groundLayer;
    private bool checkingGround;
    private bool checkingWall;

    [Header("For JumbAttacking")]
    [SerializeField] float jumpHeaight;
    [SerializeField] Transform player;
    [SerializeField] Transform groundCheck;
    [SerializeField] Vector2 boxSize;
    private bool isGrounded;

    [Header("For SeeingPlayer")]
    [SerializeField] Vector2 lineOfSite;
    [SerializeField] LayerMask playerLayer;
    private bool canSeePlayer;


    [Header("Other")]
    private Rigidbody2D enemyRB;

    // Start is called before the first frame update
    void Start()
    {
        enemyRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        checkingGround = Physics2D.OverlapCircle(groundCheckPoint.position, circleRadius, groundLayer);
        checkingWall = Physics2D.OverlapCircle(wallCheckPoint.position, circleRadius, groundLayer);
        isGrounded = Physics2D.OverlapBox(groundCheck.position, boxSize, 0, groundLayer);
        canSeePlayer = Physics2D.OverlapBox(transform.position, lineOfSite, 0, playerLayer);
        Petrolling();
        //FlipTowardsPlayer();
    }
    void Petrolling()
    {
        if(!checkingGround||checkingWall)
        {
            if(facingRight)
            {
                Flip();
            }
            else if(!facingRight)
            {
                Flip();
            }
        }
        enemyRB.velocity = new Vector2(moveSpeed * moveDiretion, enemyRB.velocity.y);
    }

    void JumpAttack()
    {
        float distanceFromPlayer = player.position.x - transform.position.x;
        if(isGrounded)
        {
            enemyRB.AddForce(new Vector2(distanceFromPlayer, jumpHeaight), ForceMode2D.Impulse);
        }
    }

    void FlipTowardsPlayer()
    {
        float playerPosition = player.position.x - transform.position.x;
        if(playerPosition<0 && facingRight)
        {
            Flip();
        }
        else if(playerPosition >0 &&!facingRight)
        {
            Flip();
        }
    }
    void Flip()
    {
        moveDiretion *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    //kiem tra

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheckPoint.position, circleRadius);
        Gizmos.DrawWireSphere(wallCheckPoint.position, circleRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(groundCheck.position, boxSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, lineOfSite);
    }
}
