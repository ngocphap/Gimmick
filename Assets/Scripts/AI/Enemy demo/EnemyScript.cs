using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    const string LEFT = "left";
    const string RIGHT = "right";

    [SerializeField]
    Transform castPos;

    [SerializeField]
    float baseCastDist;


    string facingDirection;

    Vector3 baseScale;

    Rigidbody2D rb2d;
    float moveSpeed = 2;
    // Start is called before the first frame update
    void Start()
    {
        baseScale = transform.localScale;

        facingDirection = RIGHT;

        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        float vX = moveSpeed;
        if( facingDirection ==LEFT)
        {
            vX = -moveSpeed;
        }
        //move the game object
        rb2d.velocity = new Vector2(vX, rb2d.velocity.y);
        if(IsHittingWall() || IsNearEdge())
        {
            print("HIT WALLL?????");
            if(facingDirection ==LEFT)
            {
                ChangeFacingDirection(RIGHT);
            }
            else if(facingDirection==RIGHT)
            {
                ChangeFacingDirection(LEFT);
            }
        }
    }
    void ChangeFacingDirection(string newDirection)
    {
        Vector3 newScale = baseScale;

        if(newDirection ==LEFT)
        {
            newScale.x = -baseScale.x;
        }
        else if(newDirection ==RIGHT)
        {
            newScale.x=baseScale.x;
        }
        transform.localScale = newScale;
        facingDirection = newDirection;
    }

    bool IsHittingWall()
    {
        bool val = false;
        float castDist = baseCastDist;
        //define the cast distance for left and right
        if(facingDirection == LEFT)
        {
            castDist = -baseCastDist;
        }
        else
        {
            castDist = baseCastDist;
        }
        // determine the taget destination based on the cast distance
        Vector3 targetPos = castPos.position;
        targetPos.x += castDist;

        Debug.DrawLine(castPos.position, targetPos, Color.blue);

        if(Physics2D.Linecast(castPos.position,targetPos,1<<LayerMask.NameToLayer("Ground")))
        {
            val = true;
        }
        else
        {
            val = false;
        }
        return val;
    }

    bool IsNearEdge()
    {
        bool val = true;

        float castDist = baseCastDist;
        //define the cast distance for left and right
        
        // determine the taget destination based on the cast distance
        Vector3 targetPos = castPos.position;
        targetPos.y -= castDist;

        Debug.DrawLine(castPos.position, targetPos, Color.red);

        if (Physics2D.Linecast(castPos.position, targetPos, 1 << LayerMask.NameToLayer("Ground")))
        {
            val = false;
        }
        else
        {
            val = true;
        }
        return val;
    }
}
