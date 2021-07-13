using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MambuController : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb2d;
    EnemyController enemyController;

    bool isFacingRight;
    bool isShooting;

    float openTimer;
    float closedTimer;
    float shootTimer;

    public float moveSpeed = 1f;
    public float openDelay = 1f;
    public float closedDelay = 1.5f;
    public float shootDelay = 0.5f;

    public enum MambuState { Closed, Open };
    public MambuState mambuState = MambuState.Closed;

    public enum MoveDirections { Left, Right };
    [SerializeField] MoveDirections moveDirection = MoveDirections.Left;

    // Start is called before the first frame update
    void Start()
    {
        // get components from EnemyController
        enemyController = GetComponent<EnemyController>();
        animator = enemyController.GetComponent<Animator>();
        rb2d = enemyController.GetComponent<Rigidbody2D>();

        // sprite sheet images face right
        // switch to facing left if it's set
        isFacingRight = true;
        if (moveDirection == MoveDirections.Left)
        {
            isFacingRight = false;
            enemyController.Flip();
        }

        // not shooting...yet
        isShooting = false;

        // set up the state timer we're starting with
        if (mambuState == MambuState.Closed)
        {
            closedTimer = closedDelay;
        }
        else if (mambuState == MambuState.Open)
        {
            openTimer = openDelay;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyController.freezeEnemy)
        {
            // add anything here to happen while frozen i.e. time compensations
            return;
        }

       
        switch (mambuState)
        {
            case MambuState.Closed:
                animator.Play("Mambu_Closed");
                rb2d.velocity = new Vector2(((isFacingRight) ? moveSpeed : -moveSpeed), rb2d.velocity.y);
                closedTimer -= Time.deltaTime;
                if (closedTimer < 0)
                {
                    mambuState = MambuState.Open;
                    openTimer = openDelay;
                    shootTimer = shootDelay;
                }
                break;
            case MambuState.Open:
                animator.Play("Mambu_Open");
                rb2d.velocity = new Vector2(0, rb2d.velocity.y);
                shootTimer -= Time.deltaTime;
                if (shootTimer < 0 && !isShooting)
                {
                    ShootBullet();
                    isShooting = true;
                }
                openTimer -= Time.deltaTime;
                if (openTimer < 0)
                {
                    mambuState = MambuState.Closed;
                    closedTimer = closedDelay;
                    isShooting = false;
                }
                break;
        }
    }

    public void SetMoveDirection(MoveDirections direction)
    {
        // we can call this to change the moving direction in real-time
        moveDirection = direction;
        // flip the facing side if it's needed
        if (moveDirection == MoveDirections.Left)
        {
            if (isFacingRight)
            {
                isFacingRight = !isFacingRight;
                enemyController.Flip();
            }
        }
        else
        {
            if (!isFacingRight)
            {
                isFacingRight = !isFacingRight;
                enemyController.Flip();
            }
        }
    }

    private void ShootBullet()
    {
        // initialize the bullet vectors
        GameObject[] bullets = new GameObject[1];
        Vector2[] bulletVectors = {
          //  new Vector2(-1f, 0),            // Left
            new Vector2(1f, 0),             // Right
          //  new Vector2(0, -1f),            // Down
          //  new Vector2(0, 1f),             // Up
          //  new Vector2(-0.75f, -0.75f),    // Left-Down
          //  new Vector2(-0.75f, 0.75f),     // Left-Up
           // new Vector2(0.75f, -0.75f),     // Right-Down
          //  new Vector2(0.75f, 0.75f)       // Right-Up
        };
        // initialize and shoot all the bullets
        for (int i = 0; i < bullets.Length; i++)
        {
            bullets[i] = Instantiate(enemyController.bulletPrefab);
            bullets[i].name = enemyController.bulletPrefab.name;
            bullets[i].transform.position = enemyController.bulletShootPos.transform.position;
            bullets[i].GetComponent<BulletScript>().SetBulletType(BulletScript.BulletTypes.MiniPink);
            bullets[i].GetComponent<BulletScript>().SetDamageValue(enemyController.bulletDamage);
            bullets[i].GetComponent<BulletScript>().SetBulletSpeed(enemyController.bulletSpeed);
            bullets[i].GetComponent<BulletScript>().SetBulletDirection(bulletVectors[i]);
            bullets[i].GetComponent<BulletScript>().SetCollideWithTags("Player");
            bullets[i].GetComponent<BulletScript>().SetDestroyDelay(5f);
            bullets[i].GetComponent<BulletScript>().Shoot();
        }
        // play only one bullet sound
        SoundManager.Instance.Play(enemyController.shootBulletClip);
    }

    // we call these functions from our two animations
    // Closed - make'em invincible
    private void StartInvincibleAnimation()
    {
        enemyController.Invincible(true);
    }

    // Open - beware the Mega Buster!
    private void StopInvincibleAnimation()
    {
        enemyController.Invincible(false);
    }
}
