using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public List<Transform> points;
    public Transform platfrom;
    int goalPoint = 0;
    public float moveSpeed = 2;

    private void Update()
    {
        MoveToNextPoint();  
    }
    void MoveToNextPoint()
    {
        //change the position oof  the platfrom (move towards the goal point)
        // vi trí
        platfrom.position = Vector2.MoveTowards(platfrom.position,points[goalPoint].position,Time.deltaTime*moveSpeed);
        // Check if we are in very clos proximity of the next point
        if(Vector2.Distance(platfrom.position,points[goalPoint].position)<0.1f)
        {
            //if so change goal point to next one
            if (goalPoint == points.Count - 1)
                goalPoint = 0;
            else
                goalPoint++;
        }

    }
}
