using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    [SerializeField]
    Transform player;

    [SerializeField]
    float agroRange;

    [SerializeField]
    float moveSpeed;

    Rigidbody2D rb2d;
    Animator anima;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anima = GetComponent<Animator>();
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

    void ChasePlayer()
    {
        if (transform.position.x < player.position.x)
        {
            //enemy is to the left side of the player, so move right
            // rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
            //   GetComponent<SpriteRenderer>().flipX = false;
            //anima.Play("Falling");
            rb2d.isKinematic = false;

        }
        else if (transform.position.x > player.position.x)
        {
            //enemy is to the right side of the player, so move left
            // rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
            // GetComponent<SpriteRenderer>().flipX = true;
           // anima.Play("Falling");
            rb2d.isKinematic = false;

        }
        anima.Play("Falling");
        rb2d.isKinematic = false;
    }

    void StopChasingPlayer()
    {
        rb2d.velocity = new Vector2(0, 0);
    }
}
