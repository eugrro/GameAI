using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunawayBanana : MonoBehaviour
{
    public Transform target;
    public float speed = 10f;
    public float maxDistance = 15; 
    private float range;

    void Update ()
     {
        range = Vector2.Distance(transform.position, target.position);

        if (range > maxDistance)
        {  
            transform.Translate((target.position - transform.position) * speed * Time.deltaTime);
            
        }else{
            transform.Translate((transform.position - target.position) * speed * Time.deltaTime);
        }
     }
}
