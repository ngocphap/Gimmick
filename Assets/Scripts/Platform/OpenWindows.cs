using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWindows : MonoBehaviour
{
    public Animator anima;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        anima = GetComponent<Animator>();
        player = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.name.Equals("Player"))
        {
            anima.Play("open");
        }
    }
}
