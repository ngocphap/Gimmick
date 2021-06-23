using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_behaviour : MonoBehaviour
{
    #region Public Variables
    public Transform rayCast;
    public LayerMask raycastMask;
    public float rayCastLength;
    public float attackDistance;
    public float moveSpeed;
    public float timer;
    #endregion

    #region Private Variables
    private RaycastHit2D hit;
    private GameObject target;
    private Animator anim;
    private float distance;
    private bool attackMode;
    private bool inRange;
    private bool cooling;
    private bool ischeckground;
    private bool isCheckJump=false;
    private float intTimer;
    #endregion

     void Awake()
    {
        intTimer = timer;
        anim = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(inRange)
        {
            hit = Physics2D.Raycast(rayCast.position, Vector2.left, rayCastLength, raycastMask);
            RaycastDebugger();
        }
        // awhen Player is deteced
        if (hit.collider != null)
        {
            EnemyLogic();
        }
        else if (hit.collider == null)
        {
            inRange = false;
        }
        if(inRange ==false)
        {
            anim.SetBool("canWalk", false);
            StopAttack();
        }
    }

    void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.transform.position);

        if(distance>attackDistance)
        {
            Move();
            StopAttack();
        }
        else if(attackDistance>=distance && cooling == false)
        {
            Attack();
        }
        if(cooling)
        {
            anim.SetBool("Attack", false);
        }
    }

    void Move()
    {
        anim.SetBool("canWalk", true);
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            Vector2 targetPosition = new Vector2(target.transform.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    void Attack()
    {
        timer = intTimer;//Reget Timer when Payer enter AttackRange
        attackMode = true;// to check if Enemy can still attack or not

        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", true);
    }

    void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", false);
    }
    private void OnTriggerEnter2D(Collider2D trig)
    {
        if(trig.gameObject.tag=="Player")
        {
            target = trig.gameObject;
            inRange = true;
        }
        if(trig.gameObject.tag=="Ground")
        {
            ischeckground = true;
            isCheckJump = false;
        }
    }

    void RaycastDebugger()
    {
        if(distance>attackDistance)
        {
            Debug.DrawRay(rayCast.position, Vector2.left * rayCastLength, Color.red);
        }
        else if(attackDistance >distance)
        {
            Debug.DrawRay(rayCast.position, Vector2.left * rayCastLength, Color.green);
        }
    }
}
