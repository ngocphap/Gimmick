using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMoveController : MonoBehaviour
{
    [SerializeField] bool MoveForward;
    [SerializeField] bool MoveBack;

    private float force;

    private void Awake()
    {
        force = (MoveForward) ? 2.5f : -2.5f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            //Debug.Log("gforce: " + force);
            collision.gameObject.GetComponent<PlayerController>().gforce = force;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().gforce = force;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().gforce = 0;
        }
    }
}
