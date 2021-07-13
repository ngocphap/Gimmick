using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyJumpGround : MonoBehaviour
{
   // float dirX;

    [SerializeField]
    float moveSpeed = 3f;

    Rigidbody2D rb2d;

   // bool facingRight = false;

   // Vector3 localScale;

    [SerializeField]
    Transform player;

    [SerializeField]
    float agroRange;

    // Rigidbody2D rb2d;
    Animator anima;
    // Start is called before the first frame update
    void Start()
    {
        //localScale = transform.localScale;
        rb2d = GetComponent<Rigidbody2D>();
       // dirX = -1f;
    }

    // Update is called once per frame
    void Update()
    {
       /*if(transform.position.x <-9f)
        {
            dirX = 1f;

        }
        else if (transform.position.x >9f)
        {
            dirX = -1f;
        }*/

       
    }
     void FixedUpdate()
    {
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer < agroRange)
        {
           // rb2d.velocity = new Vector2(dirX * moveSpeed, rb2d.velocity.y);
            //dirX = 1f;
            //code to chase player
            ChasePlayer();
        }
        else
        {
            //dirX = -1f;
            //stop chasing player
            StopChasingPlayer();
        }
        //rb2d.velocity = new Vector2(dirX * moveSpeed, rb2d.velocity.y);
    }

     void LateUpdate()
    {
      
      // CheckWhereToFace();
    }
    /*void CheckWhereToFace()
    {
        if (dirX > 0)
            facingRight = true;
        else if (dirX < 0)
            facingRight = false;

        if (((facingRight) && (localScale.x < 0)) || ((!facingRight) && (localScale.x > 0)))
            localScale.x *= -1;

        transform.localScale = localScale;
    }*/

    void ChasePlayer()
    {
        if (transform.position.x < player.position.x)
        {
            //enemy is to the left side of the player, so move right
            rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
            GetComponent<SpriteRenderer>().flipX = false;
           

        }
        else if (transform.position.x > player.position.x)
        {
            //enemy is to the right side of the player, so move left
            rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
            GetComponent<SpriteRenderer>().flipX = true;
           

        }
       
    }

    void StopChasingPlayer()
    {
        //rb2d.isKinematic = false;
        rb2d.velocity = new Vector2(0, 0);
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        switch(col.tag)
        {
            case "BigStump":
                rb2d.AddForce(Vector2.up * 250f);
                break;
            case "SmallStump":
                rb2d.AddForce(Vector2.up * 650f);
                break;
        }
    }
}
