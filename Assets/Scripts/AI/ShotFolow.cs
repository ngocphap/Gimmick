using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotFolow : MonoBehaviour
{
    public float speed;

    private Transform player;
    private Vector2 target;

    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        target = new Vector2(player.position.x, player.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
