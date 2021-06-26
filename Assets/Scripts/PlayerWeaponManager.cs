using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [SerializeField]private GameObject bulletPrefab1;
    [SerializeField] private GameObject bulletPrefab2;
    [SerializeField] private GameObject bulletPrefab3;

    PlayerController playerShootScript;

    private void Awake()
    {
        playerShootScript = GetComponent<PlayerController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SetWeapon(3);
        }
    }

    void SetWeapon(int weaponID)
    {
        switch(weaponID)
        {
            case 1:
                playerShootScript.SetBulletPrefab(bulletPrefab1);
                break;
            case 2:
                playerShootScript.SetBulletPrefab(bulletPrefab2);
                break;
            case 3:
                playerShootScript.SetBulletPrefab(bulletPrefab3);
                break;

        }
    }
    void ChangeWeaponEnergn()
    {

    }
}
