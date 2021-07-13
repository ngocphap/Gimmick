using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : MonoBehaviour
{
   // public GameObject Enemy;
    Rigidbody2D rb;
    Animator anima;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anima = GetComponent<Animator>();
       // Enemy = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            anima.Play("Falling");
            rb.isKinematic = false;
        }
           

       
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //Destroy(gameObject);
    }

    
}
