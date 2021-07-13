using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// all items will require these components
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]

public class ItemScript : MonoBehaviour
{
    Animator animator;
    BoxCollider2D box2d;
    Rigidbody2D rb2d;
    SpriteRenderer sprite;

    //ColorSwap colorSwap;

    private enum SwapIndex
    {
        Primary = 64,
        Secondary = 128
    }

    public enum ItemTypes
    {
        Nothing,
        Random,
        BonusBall,
       // ExtraLife,
        LifeEnergyBig,
        LifeEnergySmall,
        WeaponEnergyBig,
        WeaponEnergySmall,
       // MagnetBeam,
        WeaponPart,
       // Yashichi
    };

    [SerializeField] ItemTypes itemType;

    [SerializeField] bool animate;
    [SerializeField] float destroyDelay;
    [SerializeField] int lifeEnergy;
    [SerializeField] int weaponEnergy;
    [SerializeField] int bonusPoints;

    [Header("Audio Clips")]
    [SerializeField] AudioClip itemClip;

    [Header("Bonus Ball Settings")]
    [SerializeField] RuntimeAnimatorController racBonusBallBlue;
    [SerializeField] RuntimeAnimatorController racBonusBallGray;
    [SerializeField] RuntimeAnimatorController racBonusBallGreen;
    [SerializeField] RuntimeAnimatorController racBonusBallOrange;
    [SerializeField] RuntimeAnimatorController racBonusBallRed;
    [SerializeField] RuntimeAnimatorController racBonusBallBlack;
    public enum BonusBallColors { Random, Blue, Gray, Green, Orange, Red ,Black};
    [SerializeField] BonusBallColors bonusBallColor = BonusBallColors.Blue;

    [Header("Weapon Part Settings")]
    [SerializeField] RuntimeAnimatorController racWeaponPartBlue;
    [SerializeField] RuntimeAnimatorController racWeaponPartOrange;
    [SerializeField] RuntimeAnimatorController racWeaponPartRed;
    public enum WeaponPartColors { Random, Blue, Orange, Red };
    [SerializeField] WeaponPartColors weaponPartColor = WeaponPartColors.Blue;
    public enum WeaponPartEnemies { None, BombMan, FireMan };
    [SerializeField] WeaponPartEnemies weaponPartEnemy = WeaponPartEnemies.None;

    [Header("Bonus Item Events")]
    public UnityEvent BonusItemEvent;

    void Awake()
    {
        // get components
        animator = GetComponent<Animator>();
        box2d = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // color swap component to change item's palette
        //colorSwap = GetComponent<ColorSwap>();

        // set the color swap palette
        SetColorPalette();

        // if no animation then default to the first frame
        Animate(animate);

        // if there is a delay set then apply it
        SetDestroyDelay(destroyDelay);

        // set bonus ball color
        if (itemType == ItemTypes.BonusBall)
        {
            SetBonusBallColor(bonusBallColor);
        }

        // set weapon part color
        if (itemType == ItemTypes.WeaponPart)
        {
            SetWeaponPartColor(weaponPartColor);
        }
    }

    public void Animate(bool animate)
    {
        if (animate)
        {
            animator.Play("Default");
            animator.speed = 1;
        }
        else
        {
            animator.Play("Default", 0, 0);
            animator.speed = 0;
        }
    }

    public void SetDestroyDelay(float delay)
    {
        destroyDelay = delay;
        if (delay > 0)
        {
            Destroy(gameObject, delay);
        }
    }

    public void SetBonusBallColor(BonusBallColors color)
    {
        if (itemType == ItemTypes.BonusBall)
        {
            bonusBallColor = color;
            SetBonusBallAnimatorController();
        }
    }

    void SetBonusBallAnimatorController()
    {
        // get bonus ball color
        BonusBallColors color = bonusBallColor;

        // if random pick a bonus ball color
        if (color == BonusBallColors.Random)
        {
            color = (BonusBallColors)UnityEngine.Random.Range(
                1, Enum.GetNames(typeof(BonusBallColors)).Length);
        }

        // set color animator controller
        switch (color)
        {
            case BonusBallColors.Blue:
                animator.runtimeAnimatorController = racBonusBallBlue;
                break;
            case BonusBallColors.Gray:
                animator.runtimeAnimatorController = racBonusBallGray;
                break;
            case BonusBallColors.Green:
                animator.runtimeAnimatorController = racBonusBallGreen;
                break;
            case BonusBallColors.Orange:
                animator.runtimeAnimatorController = racBonusBallOrange;
                break;
            case BonusBallColors.Red:
                animator.runtimeAnimatorController = racBonusBallRed;
                break;
            case BonusBallColors.Black:
                animator.runtimeAnimatorController = racBonusBallBlack;
                break;
        }
    }

    public void SetWeaponPartColor(WeaponPartColors color)
    {
        if (itemType == ItemTypes.WeaponPart)
        {
            weaponPartColor = color;
            SetWeaponPartAnimatorController();
        }
    }

    void SetWeaponPartAnimatorController()
    {
        // get weapon part color
        WeaponPartColors color = weaponPartColor;

        // if random pick a weapon part color
        if (color == WeaponPartColors.Random)
        {
            color = (WeaponPartColors)UnityEngine.Random.Range(
                1, Enum.GetNames(typeof(WeaponPartColors)).Length);
        }

        // set color animator controller
        switch (color)
        {
            case WeaponPartColors.Blue:
                animator.runtimeAnimatorController = racWeaponPartBlue;
                break;
            case WeaponPartColors.Orange:
                animator.runtimeAnimatorController = racWeaponPartOrange;
                break;
            case WeaponPartColors.Red:
                animator.runtimeAnimatorController = racWeaponPartRed;
                break;
        }
    }

    public void SetColorPalette()
    {
        // not all bonus items have the ColorSwap component
        // only the Extra Life, Magnet Beam and Weapon Energies
       // if (colorSwap != null)
        {
            // default to megabuster / magnetbeam colors
            // dark blue, light blue
           // colorSwap.SwapColor((int)SwapIndex.Primary, ColorSwap.ColorFromInt(0x0073F7));
           // colorSwap.SwapColor((int)SwapIndex.Secondary, ColorSwap.ColorFromInt(0x00FFFF));

            // find the player's controller to access the weapon type
            PlayerController player = GameObject.FindObjectOfType<PlayerController>();
            if (player != null)
            {
                // apply new selected color scheme with ColorSwap
                switch (player.playerWeapon)
                {
                    case PlayerController.WeaponTypes.BombMan:
                        // green, light gray
                       // colorSwap.SwapColor((int)SwapIndex.Primary, ColorSwap.ColorFromInt(0x009400));
                        //colorSwap.SwapColor((int)SwapIndex.Secondary, ColorSwap.ColorFromInt(0xFCFCFC));
                        break;
                    case PlayerController.WeaponTypes.FireMan:
                        // dark gray, light gray
                        //colorSwap.SwapColor((int)SwapIndex.Primary, ColorSwap.ColorFromInt(0x747474));
                       // colorSwap.SwapColor((int)SwapIndex.Secondary, ColorSwap.ColorFromInt(0xFCFCFC));
                        break;
                    case PlayerController.WeaponTypes.GimmickBuster:
                        // dark gray, light yellow
                       //colorSwap.SwapColor((int)SwapIndex.Primary, ColorSwap.ColorFromInt(0x747474));
                       // colorSwap.SwapColor((int)SwapIndex.Secondary, ColorSwap.ColorFromInt(0xFCE4A0));
                        break;
                   
                }
            }

            // apply the color changes
           // colorSwap.ApplyColor();
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            if (lifeEnergy > 0)
            {
                // add to player health energy
                player.ApplyLifeEnergy(lifeEnergy);
            }

            if (weaponEnergy > 0)
            {
                // add to current weapon energy
                player.ApplyWeaponEnergy(weaponEnergy);
            }

            if (bonusPoints >= 0)
            {
                // call game manager to add bonus points
                GameManager.Instance.AddScorePoints(bonusPoints);
            }

            

            if (itemType == ItemTypes.WeaponPart)
            {
                // collected a weapon part from a defeated boss
                player.EnableWeaponPart(weaponPartEnemy);
            }

            // play item sound
            if (itemClip != null)
            {
                SoundManager.Instance.Play(itemClip);
            }

            // invoke the bonus item event
            if (BonusItemEvent != null)
            {
                BonusItemEvent.Invoke();
            }

            // remove the item
            Destroy(gameObject);
        }
    }
}