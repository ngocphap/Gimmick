using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3 : MonoBehaviour
{
    Animator animator;
    BoxCollider2D box2d;
    Rigidbody2D rb2d;
    SpriteRenderer sprite;

    bool isGrounded;
    bool isJumping;
    bool isFacingRight;

    [SerializeField]
    Transform player;

    [SerializeField]
    float agroRange;

    [SerializeField]
    float moveSpeed;

   // Rigidbody2D rb2d;
    Animator anima;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        box2d = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //distance to player
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer < agroRange)
        {
            //code to chase player
            ChasePlayer();
        }
        else
        {
            //stop chasing player
            StopChasingPlayer();
        }
    }

    private void FixedUpdate()
    {
        isGrounded = false;
        Color raycastColor;
        RaycastHit2D raycastHit;
        //RaycastHit2D raycastHitMovingPlatfrom;//
        float raycastDistance = 0.05f;
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        // int tranfromMovingplatform = 1 << LayerMask.NameToLayer("MovingPlatfrom");//
        // ground check
        Vector3 box_origin = box2d.bounds.center;
        box_origin.y = box2d.bounds.min.y + (box2d.bounds.extents.y / 4f);
        Vector3 box_size = box2d.bounds.size;
        box_size.y = box2d.bounds.size.y / 4f;
        raycastHit = Physics2D.BoxCast(box_origin, box_size, 0f, Vector2.down, raycastDistance, layerMask);
        // raycastHitMovingPlatfrom = Physics2D.BoxCast(box_origin, box_size, 0f, Vector2.down, raycastDistance, tranfromMovingplatform);
        // player box colliding with ground layer
        if (raycastHit.collider != null)
        {
            isGrounded = true;
            // just landed from jumping/falling
            if (isJumping)
            {
                isJumping = false;
                //SoundManager.Instance.Play(jumpLandedClip);
            }


        }
        // draw debug lines
        raycastColor = (isGrounded) ? Color.green : Color.red;
        Debug.DrawRay(box_origin + new Vector3(box2d.bounds.extents.x, 0), Vector2.down * (box2d.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_origin - new Vector3(box2d.bounds.extents.x, 0), Vector2.down * (box2d.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_origin - new Vector3(box2d.bounds.extents.x, box2d.bounds.extents.y / 4f + raycastDistance), Vector2.right * (box2d.bounds.extents.x * 2), raycastColor);
    }

    void ChasePlayer()
    {
        if (transform.position.x < player.position.x)
        {
            //enemy is to the left side of the player, so move right
             rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
             GetComponent<SpriteRenderer>().flipX = false;
            //anima.Play("Falling");
            //rb2d.isKinematic = false;

        }
        else if (transform.position.x > player.position.x)
        {
            //enemy is to the right side of the player, so move left
            rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
            GetComponent<SpriteRenderer>().flipX = true;
            // anima.Play("Falling");
          // rb2d.isKinematic = false;

        }
       // anima.Play("Falling");
        rb2d.isKinematic = false;
    }

    void StopChasingPlayer()
    {
        //rb2d.isKinematic = false;
        rb2d.velocity = new Vector2(0, 0);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
       /* if (collision.gameObject.name.Equals("Player"))
        {
            //anima.Play("Falling");
            rb2d.isKinematic = false;
        }*/
    }
}
