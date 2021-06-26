using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    BoxCollider2D box2d;
    Rigidbody2D rb2d;
    SpriteRenderer sprite;

    float keyHorizontal;
   // float keyVertical;
    bool keyJump;
    bool keyShoot;

    bool isGrounded;
    bool isJumping;
    bool isShooting;
    bool isThrowing;
    bool isTakingDamage;
    bool isInvincible;
    bool isFacingRight;

    bool hitSideRight;

    bool freezeInput;
    bool freezePlayer;
    bool freezeBullets;

    float shootTime;
    float shootTimeLength;
    bool keyShootRelease;
    float keyShootReleaseTimeLength;

    bool canUseWeapon;


    bool isCheckgroundslip = false;
    RigidbodyConstraints2D rb2dConstraints;

    public float gforce;//invoked from GroudMoveController.cs
    //
    public enum WeaponTypes { GimmickBuster, BombMan ,FireMan};
    public WeaponTypes playerWeapon = WeaponTypes.GimmickBuster;

    [System.Serializable]
    public struct WeaponsStruct
    {
        public WeaponTypes weaponType;
        public bool enabled;
        public int currentEnergy;
        public int maxEnergy;

        public int energyCost;
        public int weaponDamage;
        public GameObject weaponPrefab;
    }
    public WeaponsStruct[] weaponsData;
    //

    public int currentHealth;
    public int maxHealth = 4;

    [SerializeField] float moveSpeed = 1.5f;
    [SerializeField] float jumpSpeed = 3.7f;

    [SerializeField] int bulletDamage = 1;
    [SerializeField] float bulletSpeed = 5f;
   

    [Header("Audio Clips")]
    //[SerializeField] AudioClip teleportClip;
    [SerializeField] AudioClip jumpLandedClip;
    [SerializeField] AudioClip shootBulletClip;
    [SerializeField] AudioClip takingDamageClip;
    [SerializeField] AudioClip explodeEffectClip;
    [SerializeField] AudioClip energyFillClip;

    [Header("Positions and Prefabs")]
    [SerializeField] Transform bulletShootPos;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject explodeEffectPrefab;


    private void Awake()
    {
        // get handles to components
        animator = GetComponent<Animator>();
        box2d = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        

        // sprite defaults to facing right
        isFacingRight = true;

        // start at full health
        currentHealth = maxHealth;
        SetWeapon(playerWeapon);
        FillWeaponEnergies();
       /* for(int i=0;i<playerWeaponStructs.Length;i++)
        {
            playerWeaponStructs[i].currentEnergy = playerWeaponStructs[i].maxEnergy;
        }
        //
        SetWeapon(playerWeapon);*/
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
                SoundManager.Instance.Play(jumpLandedClip);
            }
            
            
        }
        // draw debug lines
        raycastColor = (isGrounded) ? Color.green : Color.red;
        Debug.DrawRay(box_origin + new Vector3(box2d.bounds.extents.x, 0), Vector2.down * (box2d.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_origin - new Vector3(box2d.bounds.extents.x, 0), Vector2.down * (box2d.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_origin - new Vector3(box2d.bounds.extents.x, box2d.bounds.extents.y / 4f + raycastDistance), Vector2.right * (box2d.bounds.extents.x * 2), raycastColor);
    }

    // Update is called once per frame
    void Update()
    {
        // taking damage from projectiles, touching enemies, or other environment objects
        if (isTakingDamage)
        {
            animator.Play("Player_Hit");
            return;
        }
        // player input and movement
        PlayerDebugInput();
        PlayerDirectionInput();
        PlayerJumpInput();
        PlayerShootInput();
        PlayerMovement();

        //fire selected weappn
        FireWeapon();
    }

    void PlayerDebugInput()
    {
        // B for Bullets
        if (Input.GetKeyDown(KeyCode.B))
        {
            GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
            if (bullets.Length > 0)
            {
                freezeBullets = !freezeBullets;
                foreach (GameObject bullet in bullets)
                {
                    bullet.GetComponent<BulletScript>().FreezeBullet(freezeBullets);
                }
            }
            Debug.Log("Freeze Bullets: " + freezeBullets);
        }

        // E for explosions
        if (Input.GetKeyDown(KeyCode.E))
        {
            Defeat();
            Debug.Log("Defeat()");
        }

        // I for Invincible
        if (Input.GetKeyDown(KeyCode.I))
        {
            Invincible(!isInvincible);
            Debug.Log("Invincible: " + isInvincible);
        }
        // L for Life Energy
        if (Input.GetKeyDown(KeyCode.L))
        {
            ApplyLifeEnergy(10);
            Debug.Log("ApplyLifeEnergy(10)");
        }

        // K for Keyboard
        if (Input.GetKeyDown(KeyCode.K))
        {
            FreezeInput(!freezeInput);
            Debug.Log("Freeze Input: " + freezeInput);
        }

        // P for Player
        if (Input.GetKeyDown(KeyCode.P))
        {
            FreezePlayer(!freezePlayer);
            Debug.Log("Freeze Player: " + freezePlayer);
        }

        // S for Switch Weapon
        if (Input.GetKeyDown(KeyCode.S))
        {
            int nextWeapon = (int)playerWeapon;
            int maxWeapons = weaponsData.Length;
            while (true)
            {
                // cycle to next weapon index
                if (++nextWeapon > maxWeapons - 1)
                {
                    nextWeapon = 0;
                }
                // if weapon is enabled then use it
                if (weaponsData[nextWeapon].enabled)
                {
                    SwitchWeapon((WeaponTypes)nextWeapon);
                    break;
                }
            }
            Debug.Log("SwitchWeapon()");
        }
        // T 
        if (Input.GetKeyDown(KeyCode.T))
        {
           // SetWeapon((PlayerWeapons)UnityEngine.Random.Range(0,
            //    Enum.GetValues(typeof(PlayerWeapons)).Length));
            //Teleport(true);
            //Debug.Log("Teleport(true)");
        }
        // #1 for random Enemy Health energy bar color
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UIEnergyBars.Instance.SetImage(
                UIEnergyBars.EnergyBars.PlayerWeapon1,
                (UIEnergyBars.EnergyBarTypes)UnityEngine.Random.Range(0,
                    Enum.GetValues(typeof(UIEnergyBars.EnergyBarTypes)).Length));
        }
        // #2 for random Enemy Health energy bar color
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UIEnergyBars.Instance.SetImage(
                UIEnergyBars.EnergyBars.PlayerWeapon2,
                (UIEnergyBars.EnergyBarTypes)UnityEngine.Random.Range(0,
                    Enum.GetValues(typeof(UIEnergyBars.EnergyBarTypes)).Length));
        }
        // #3 for random Enemy Health energy bar color
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UIEnergyBars.Instance.SetImage(
                UIEnergyBars.EnergyBars.PlayerWeapon3,
                (UIEnergyBars.EnergyBarTypes)UnityEngine.Random.Range(0,
                    Enum.GetValues(typeof(UIEnergyBars.EnergyBarTypes)).Length));
        }
    }

    void PlayerDirectionInput()
    {
        // get keyboard input
        if (!freezeInput)
        {
            keyHorizontal = Input.GetAxisRaw("Horizontal");
        }
    }

    void PlayerJumpInput()
    {
        // get keyboard input
        if (!freezeInput)
        {
            keyJump = Input.GetKeyDown(KeyCode.Space);
        }
    }

    void PlayerShootInput()
    {
       
        // get keyboard input
        if (!freezeInput)
        {
            keyShoot = Input.GetKey(KeyCode.C);
        }

      
    }

    void PlayerMovement()
    {
        // override speed may vary depending on state
        float speed = moveSpeed;
        // left arrow key - moving left
        if (keyHorizontal < 0)
        {
            // facing right while moving left - flip
            if (isFacingRight)
            {
                Flip();
            }
            // grounded play run animation
            if (isGrounded)
            {
                // play run shoot or run animation
                if (isShooting)
                {
                    animator.Play("Player_RunShoot");
                }
                else if (isThrowing)
                {
                    speed = 0f;
                    animator.Play("Player_RunShoot");
                }
                else
                {
                    animator.Play("Player_Run");
                }
            }
            // negative move speed to go left
            rb2d.velocity = new Vector2(-speed, rb2d.velocity.y);
        }
        else if (keyHorizontal > 0) // right arrow key - moving right
        {
            // facing left while moving right - flip
            if (!isFacingRight)
            {
                Flip();
            }
            // grounded play run animation
            if (isGrounded)
            {
                // play run shoot or run animation
                if (isShooting)
                {
                    animator.Play("Player_RunShoot");
                }
                else if (isThrowing)
                {
                    speed = 0f;
                    animator.Play("Player_RunShoot");
                }
                else
                {
                    animator.Play("Player_Run");
                }
            }
            // positive move speed to go right
            rb2d.velocity = new Vector2(speed, rb2d.velocity.y);
        }
        else   // no movement
        {
            // grounded play idle animation
            if (isGrounded)
            {
                // play shoot or idle animation
                if (isShooting)
                {
                    animator.Play("Player_Shoot");
                }
                else if (isThrowing)
                {
                    animator.Play("Player_Shoot");
                }
                else
                {
                    animator.Play("Player_Idle");
                }
            }
            // no movement zero x velocity
            rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
        }

        // pressing jump while grounded - can only jump once
        if (keyJump && isGrounded)
        {
            // play jump/jump shoot animation and jump speed on y velocity
            if (isShooting)
            {
                animator.Play("Player_JumpShoot");
            }
            else if (isThrowing)
            {
                animator.Play("Player_JumpShoot");
            }
            else
            {
                animator.Play("Player_Jump");
            }
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
        }
        // check player in groundMoving
        if (isGrounded)
        {
            /*25.6.2021*/
            rb2d.velocity = new Vector2(rb2d.velocity.x + gforce, rb2d.velocity.y);
        }
        // while not grounded play jump animation (jumping or falling)
        if (!isGrounded)
        {
            // triggers jump landing sound effect in FixedUpdate
            isJumping = true;
            // jump or jump shoot animation
            if (isShooting)
            {
                animator.Play("Player_JumpShoot");
            }
            else if (isThrowing)
            {
                animator.Play("Player_JumpShoot");
            }
            else
            {
                animator.Play("Player_Jump");
            }
        }
    }

    void Flip()
    {
        // invert facing direction and rotate object 180 degrees on y axis
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    public void SetWeapon(WeaponTypes weapon)
    {
        

        // set new selected weapon (determines color scheme)
        playerWeapon = weapon;

        // calculate weapon energy value to adjust the bars
        int currentEnergy = weaponsData[(int)playerWeapon].currentEnergy;
        int maxEnergy = weaponsData[(int)playerWeapon].maxEnergy;
        float weaponEnergyValue = (float)currentEnergy / (float)maxEnergy;

        // apply new selected color scheme with ColorSwap and set weapon energy bar
        switch (playerWeapon)
        {
            case WeaponTypes.GimmickBuster:
                // dark blue, light blue
                //  colorSwap.SwapColor((int)SwapIndex.Primary, ColorSwap.ColorFromInt(0x0073F7));
                //   colorSwap.SwapColor((int)SwapIndex.Secondary, ColorSwap.ColorFromInt(0x00FFFF));
                // the player weapon energy doesn't apply but we'll just set the default and hide it
                if (UIEnergyBars.Instance != null)
                {
                    UIEnergyBars.Instance.SetImage(UIEnergyBars.EnergyBars.PlayerWeapon1, UIEnergyBars.EnergyBarTypes.BigLifeHealth);
                    UIEnergyBars.Instance.SetVisibility(UIEnergyBars.EnergyBars.PlayerWeapon1, false);
                }
                break;
            case WeaponTypes.BombMan:
                // dark blue, light blue
                //  colorSwap.SwapColor((int)SwapIndex.Primary, ColorSwap.ColorFromInt(0x0073F7));
                // colorSwap.SwapColor((int)SwapIndex.Secondary, ColorSwap.ColorFromInt(0x00FFFF));
                // magnet beam energy and set visible
                if (UIEnergyBars.Instance != null)
                {
                    UIEnergyBars.Instance.SetImage(UIEnergyBars.EnergyBars.PlayerWeapon1, UIEnergyBars.EnergyBarTypes.BomBullt);
                    UIEnergyBars.Instance.SetValue(UIEnergyBars.EnergyBars.PlayerWeapon1, weaponEnergyValue);
                    UIEnergyBars.Instance.SetVisibility(UIEnergyBars.EnergyBars.PlayerWeapon1, true);
                }
                break;
            case WeaponTypes.FireMan:
                // green, light gray
                // colorSwap.SwapColor((int)SwapIndex.Primary, ColorSwap.ColorFromInt(0x009400));
                // colorSwap.SwapColor((int)SwapIndex.Secondary, ColorSwap.ColorFromInt(0xFCFCFC));
                // bombman's hyper bomb weapon energy and set visible
                if (UIEnergyBars.Instance != null)
                {
                    UIEnergyBars.Instance.SetImage(UIEnergyBars.EnergyBars.PlayerWeapon1, UIEnergyBars.EnergyBarTypes.FireRed);
                    UIEnergyBars.Instance.SetValue(UIEnergyBars.EnergyBars.PlayerWeapon1, weaponEnergyValue);
                    UIEnergyBars.Instance.SetVisibility(UIEnergyBars.EnergyBars.PlayerWeapon1, true);
                }
                break;
            
           
        }

        // apply the color changes
        //colorSwap.ApplyColor();
    }
    public void SwitchWeapon(WeaponTypes weaponType)
    {
        // we can call this function to switch the player to the chosen weapon
        // change color scheme, do the teleport animation, and enable weapon usage
        SetWeapon(weaponType);
        //Teleport(true);
        CanUseWeaponAgain();

        // update any in scene bonus item color palettes
       GameManager.Instance.SetBonusItemsColorPalette();
    }
    void FireWeapon()
    {
        // each weapon has its own function for firing
        switch (playerWeapon)
        {
            case WeaponTypes.GimmickBuster:
                GimmickBuster();
                break;
            ///case WeaponTypes.MagnetBeam:
             //   break;
            case WeaponTypes.BombMan:
                HyperBomb();
                break;
            case WeaponTypes.FireMan:
                FireManBullet();
                break;
            
        }
    }
    void GimmickBuster()
    {
        shootTimeLength = 0;
        keyShootReleaseTimeLength = 0;

        // shoot key is being pressed and key release flag true
        if (keyShoot && keyShootRelease)
        {
            isShooting = true;
         //   keyShootRelease = false;
            shootTime = Time.time;
            // Shoot Bullet
           // Invoke("ShootBullet", 0.1f);
        }
        // shoot key isn't being pressed and key release flag is false
        if (!keyShoot && !keyShootRelease)
        {
            keyShootReleaseTimeLength = Time.time - shootTime;
            keyShootRelease = true;
        }
        // while shooting limit its duration
        if (isShooting)
        {
            shootTimeLength = Time.time - shootTime;
            if (shootTimeLength >= 0.25f || keyShootReleaseTimeLength >= 0.15f)
            {
                isShooting = false;
                
                ShootBullet();
            }
        }
    }

    void HyperBomb()
    {
        shootTimeLength = 0;
        keyShootReleaseTimeLength = 0;

        // shoot key is being pressed and key release flag true
        if (keyShoot && keyShootRelease && canUseWeapon)
        {
            // only be able to throw a hyper bomb if there is energy to do so
            // placing the check here so isThrowing can't become true and activate the arm throw animation
            if (weaponsData[(int)WeaponTypes.BombMan].currentEnergy > 0)
            {
                isThrowing = true;
                canUseWeapon = false;
                keyShootRelease = false;
                shootTime = Time.time;
                // Throw Bomb
                Invoke("ThrowBomb", 0.1f);
                // spend weapon energy and refresh energy bar
                SpendWeaponEnergy(WeaponTypes.BombMan);
                RefreshWeaponEnergyBar(WeaponTypes.BombMan);
            }
        }
        // shoot key isn't being pressed and key release flag is false
        if (!keyShoot && !keyShootRelease)
        {
            keyShootReleaseTimeLength = Time.time - shootTime;
            keyShootRelease = true;
        }
        // while shooting limit its duration
        if (isThrowing)
        {
            shootTimeLength = Time.time - shootTime;
            if (shootTimeLength >= 0.25f)
            {
                isThrowing = false;
            }
        }
    }

    void FireManBullet()
    {
        shootTimeLength = 0;
        keyShootReleaseTimeLength = 0;

        // shoot key is being pressed and key release flag true
        if (keyShoot && keyShootRelease && canUseWeapon)
        {
            // only be able to throw a hyper bomb if there is energy to do so
            // placing the check here so isThrowing can't become true and activate the arm throw animation
            if (weaponsData[(int)WeaponTypes.FireMan].currentEnergy > 0)
            {
                isThrowing = true;
                canUseWeapon = false;
                keyShootRelease = false;
              shootTime = Time.time;
                // Throw Bomb
                Invoke("ThrowFire", 0.1f);
                // spend weapon energy and refresh energy bar
                SpendWeaponEnergy(WeaponTypes.FireMan);
                RefreshWeaponEnergyBar(WeaponTypes.FireMan);
            }
        }
        // shoot key isn't being pressed and key release flag is false
        if (!keyShoot && !keyShootRelease)
        {
            keyShootReleaseTimeLength = Time.time - shootTime;
            keyShootRelease = true;
        }
        // while shooting limit its duration
        if (isThrowing)
        {
            shootTimeLength = Time.time - shootTime;
            if (shootTimeLength >= 0.25f)
            {
                isThrowing = false;
            }
        }
    }
    public void ApplyLifeEnergy(int amount)
    {
        // only apply health if we need it
        if (currentHealth < maxHealth)
        {
            int healthDiff = maxHealth - currentHealth;
            if (healthDiff > amount) healthDiff = amount;
            // animate adding health bars via coroutine
            StartCoroutine(AddLifeEnergy(healthDiff));
        }
    }
    private IEnumerator AddLifeEnergy(int amount)
    {
        // loop the energy fill audio clip
        SoundManager.Instance.Play(energyFillClip, true);
        // increment the health bars with a small delay
        for (int i = 0; i < amount; i++)
        {
            currentHealth++;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            UIEnergyBars.Instance.SetValue(UIEnergyBars.EnergyBars.PlayerHealth,currentHealth / (float)maxHealth);
            yield return new WaitForSeconds(0.05f);
        }
        // done playing energy fill clip
        SoundManager.Instance.Stop();
    }
    public void ApplyWeaponEnergy(int amount)
    {
        // define this function when we get to boss battles and acquiring their weapons
        // only apply weapon energy if we need it
        int wt = (int)playerWeapon;
        if (weaponsData[wt].currentEnergy < weaponsData[wt].maxEnergy)
        {
            int energyDiff = weaponsData[wt].maxEnergy - weaponsData[wt].currentEnergy;
            if (energyDiff > amount) energyDiff = amount;
            // animate adding energy bars via coroutine
            StartCoroutine(AddWeaponEnergy(energyDiff));
        }
    }
    private IEnumerator AddWeaponEnergy(int amount)
    {
        int wt = (int)playerWeapon;
        // loop the energy fill audio clip
        SoundManager.Instance.Play(energyFillClip, true);
        // increment the energy bars with a small delay
        for (int i = 0; i < amount; i++)
        {
            weaponsData[wt].currentEnergy++;
            weaponsData[wt].currentEnergy = Mathf.Clamp(weaponsData[wt].currentEnergy, 0, weaponsData[wt].maxEnergy);
            UIEnergyBars.Instance.SetValue(
                UIEnergyBars.EnergyBars.PlayerWeapon1,
                weaponsData[wt].currentEnergy / (float)weaponsData[wt].maxEnergy);
            yield return new WaitForSeconds(0.05f);
        }
        // done playing energy fill clip
        SoundManager.Instance.Stop();
    }

    public void FillWeaponEnergies()
    {
        // start all energy bars at full
        for (int i = 0; i < weaponsData.Length; i++)
        {
            weaponsData[i].currentEnergy = weaponsData[i].maxEnergy;
        }
    }

    public void EnableWeaponPart(ItemScript.WeaponPartEnemies weaponPartEnemy)
    {
        // this will enable the collected weapon part in our weapon struct
        switch (weaponPartEnemy)
        {
            case ItemScript.WeaponPartEnemies.BombMan:
                weaponsData[(int)WeaponTypes.BombMan].enabled = true;
                break;
            
            case ItemScript.WeaponPartEnemies.FireMan:
                weaponsData[(int)WeaponTypes.FireMan].enabled = true;
                break;
            
        }
    }
    void ShootBullet()
    {
        // create bullet from prefab gameobject
        GameObject bullet = Instantiate(bulletPrefab, bulletShootPos.position, Quaternion.identity);
        // set its name to that of the prefab so it doesn't include "(Clone)" when instantiated
        bullet.name = bulletPrefab.name;
        
        // set bullet damage amount, speed, direction bullet will travel along the x, and fire!
        bullet.GetComponent<BulletScript>().SetDamageValue(bulletDamage);
        bullet.GetComponent<BulletScript>().SetBulletSpeed(bulletSpeed);
        bullet.GetComponent<BulletScript>().SetBulletDirection((isFacingRight) ? Vector2.right : Vector2.left);
        bullet.GetComponent<BulletScript>().SetDestroyDelay(5f);
        bullet.GetComponent<BulletScript>().Shoot();
        //SetBulletPrefab(bullet);
        SoundManager.Instance.Play(shootBulletClip);
    }

    void ThrowBomb()
    {
        // create bomb from prefab gameobject
        GameObject bomb = Instantiate(weaponsData[(int)WeaponTypes.BombMan].weaponPrefab);
        bomb.name = weaponsData[(int)WeaponTypes.BombMan].weaponPrefab.name + "(" + gameObject.name + ")";
        bomb.transform.position = bulletShootPos.position;
        // set the bomb properties and throw it
        bomb.GetComponent<BombScript>().SetContactDamageValue(0);
        bomb.GetComponent<BombScript>().SetExplosionDamageValue(weaponsData[(int)WeaponTypes.BombMan].weaponDamage);
        bomb.GetComponent<BombScript>().SetExplosionDelay(3f);
        bomb.GetComponent<BombScript>().SetCollideWithTags("Enemy");
        bomb.GetComponent<BombScript>().SetDirection((isFacingRight) ? Vector2.right : Vector2.left);
        bomb.GetComponent<BombScript>().SetVelocity(new Vector2(2f, 1.5f));
        bomb.GetComponent<BombScript>().Bounces(true);
        bomb.GetComponent<BombScript>().ExplosionEvent.AddListener(CanUseWeaponAgain);
        bomb.GetComponent<BombScript>().Launch(false);
    }
    void ThrowFire()
    {
        // create bomb from prefab gameobject
        GameObject bomb = Instantiate(weaponsData[(int)WeaponTypes.FireMan].weaponPrefab);
        bomb.name = weaponsData[(int)WeaponTypes.FireMan].weaponPrefab.name + "(" + gameObject.name + ")";
        bomb.transform.position = bulletShootPos.position;
        // set the bomb properties and throw it
        bomb.GetComponent<BombScript>().SetContactDamageValue(0);
        bomb.GetComponent<BombScript>().SetExplosionDamageValue(weaponsData[(int)WeaponTypes.FireMan].weaponDamage);
        bomb.GetComponent<BombScript>().SetExplosionDelay(3f);
        bomb.GetComponent<BombScript>().SetCollideWithTags("Enemy");
        bomb.GetComponent<BombScript>().SetDirection((isFacingRight) ? Vector2.right : Vector2.left);
        bomb.GetComponent<BombScript>().SetVelocity(new Vector2(2f, 1.5f));
        bomb.GetComponent<BombScript>().Bounces(true);
        bomb.GetComponent<BombScript>().ExplosionEvent.AddListener(CanUseWeaponAgain);
        bomb.GetComponent<BombScript>().Launch(false);
    }
    void SpendWeaponEnergy(WeaponTypes weaponType)
    {
        // deplete the weapon energy and make sure the value is within bounds
        int wt = (int)weaponType;
        weaponsData[wt].currentEnergy -= weaponsData[wt].energyCost;
        weaponsData[wt].currentEnergy = Mathf.Clamp(weaponsData[wt].currentEnergy, 0, weaponsData[wt].maxEnergy);
    }

    void RefreshWeaponEnergyBar(WeaponTypes weaponType)
    {
        // refresh the weapon energy bar (should be called after SpendWeaponEnergy)
        int wt = (int)weaponType;
        if (UIEnergyBars.Instance != null)
        {
            UIEnergyBars.Instance.SetValue(
                UIEnergyBars.EnergyBars.PlayerWeapon1,
                weaponsData[wt].currentEnergy / (float)weaponsData[wt].maxEnergy);
        }
    }

    void CanUseWeaponAgain()
    {
        // many (almost all) of our weapons require they play out their animation or be destroyed
        // before another copy can be used so this function resets the flag to be able to fire again
        canUseWeapon = true;
    }

    public void HitSide(bool rightSide)
    {
        // determines the push direction of the hit animation
        hitSideRight = rightSide;
    }

    public void Invincible(bool invincibility)
    {
        isInvincible = invincibility;
    }

    public void TakeDamage(int damage)
    {
        // take damage if not invincible
        if (!isInvincible)
        {
            // take damage amount from health and update the health bar
            if (damage > 0)
            {
                currentHealth -= damage;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                if (UIEnergyBars.Instance != null)
                {
                    UIEnergyBars.Instance.SetValue(UIEnergyBars.EnergyBars.PlayerHealth, currentHealth / (float)maxHealth);
                }
                // no more health means defeat, otherwise take damage
                if (currentHealth <= 0)
                {
                    Defeat();
                }
                else
                {
                    StartDamageAnimation();
                }
            }
        }
    }

    void StartDamageAnimation()
    {
        // once isTakingDamage is true in the Update function we'll play the Hit animation
        // here we go invincible so we don't repeatedly take damage, determine the X push force
        // depending which side we were hit on, and then apply that force
        if (!isTakingDamage)
        {
            isTakingDamage = true;
            Invincible(true);
            FreezeInput(true);
            float hitForceX = 0.50f;
            float hitForceY = 1.5f;
            if (hitSideRight) hitForceX = -hitForceX;
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(hitForceX, hitForceY), ForceMode2D.Impulse);
            SoundManager.Instance.Play(takingDamageClip);
        }
    }

    void StopDamageAnimation()
    {
        // this function is called at the end of the Hit animation
        // and we reset the animation because it doesn't loop otherwise
        // we can end up stuck in it
        isTakingDamage = false;
        //Invincible(false);
        FreezeInput(false);
        animator.Play("Player_Hit", -1, 0f);
        StartCoroutine(FlashAfterDamage());
    }


    private IEnumerator FlashAfterDamage()
    {
        float flashDelay = 0.0833f;
        for(int i=0;i<10;i++)
        {
            //sprite.enabled = false;
            // sprite.material = null;
            sprite.color = Color.clear;
            yield return new WaitForSeconds(flashDelay);
            // sprite.enabled = true;
            ///sprite.material = new Material(Shader.Find("Sprites/Default"));
            ///
            sprite.color = Color.white;
            yield return new WaitForSeconds(flashDelay);
        }
        Invincible(false);
    }
    void StartDefeatAnimation()
    {
        // freeze player and input and go KABOOM!
        FreezeInput(true);
        FreezePlayer(true);
        GameObject explodeEffect = Instantiate(explodeEffectPrefab);
        explodeEffect.name = explodeEffectPrefab.name;
        explodeEffect.transform.position = sprite.bounds.center;
        SoundManager.Instance.Play(explodeEffectClip);
        Destroy(gameObject);
    }

    void StopDefeatAnimation()
    {
        FreezeInput(false);
        FreezePlayer(false);
    }

    void Defeat()
    {
        // tell the game manager we died so it can take control
        GameManager.Instance.PlayerDefeated();
        // we died! player defeat animation - half second delay
        Invoke("StartDefeatAnimation", 0.5f);
    }

    public void FreezeInput(bool freeze)
    {
        // freeze/unfreeze user input
        freezeInput = freeze;
        if(freeze)
        {
            keyHorizontal = 0;
          //  keyVertical = 0;
            keyJump = false;
            keyShoot = false;
        }
    }

    public void FreezePlayer(bool freeze)
    {
        // freeze/unfreeze the player on screen
        // zero animation speed and freeze XYZ rigidbody constraints
        if (freeze)
        {
            freezePlayer = true;
            rb2dConstraints = rb2d.constraints;
            animator.speed = 0;
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            freezePlayer = false;
            animator.speed = 1;
            rb2d.constraints = rb2dConstraints;
        }
    }

    public void SimulateMoveStop()
    {
        keyHorizontal = 0f;
    }

    public void SimulateMoveLeft()
    {
        keyHorizontal = -1.0f;
    }
    public void SimulateMoveRight()
    {
        keyHorizontal = 1.0f;
    }

    public void SimulateJump()
    {
        keyJump = true;
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        
        if (coll.gameObject.tag == "MovingPlatfrom" )
        {
            isGrounded = true;
            transform.parent = coll.gameObject.transform;
        }
        else
        {
            transform.parent = null;
        }

        


    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }
    public void SetBulletPrefab(GameObject newBullet)
    {
        bulletPrefab = newBullet;
    }
}
