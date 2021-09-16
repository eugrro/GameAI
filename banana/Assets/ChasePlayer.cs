using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayer : MonoBehaviour
{
    public Transform target;
    public float speed = 10f;
    public float minDistance = 1f;
    public float slowDistance = 20;
    private float range;

    public GameObject SceneChanger;
    void Update ()
     {
         range = Vector2.Distance(transform.position, target.position);
        if (range < slowDistance && speed != 10){
            speed = 7;
        } 
         if (range > minDistance)
         {
             
              transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            Vector3 dir = target.position - transform.position;
            float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }else{
            SceneChanger.GetComponent<SceneChanger>().EndGame();
        }
     }
}
