using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowThePath : MonoBehaviour
{

    public Transform[] waypoints;
    public Transform target;
    public GameObject SceneChanger;
    public float minDistance = 1f;
    public float moveSpeed = 2f;
    public int current = 1;

    // Use this for initialization
    void Start()
    {

        // Set position of Enemy as position of the first waypoint
        transform.position = waypoints[current].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var range = Vector2.Distance(transform.position, target.position);

        if (range < minDistance)
        {
            SceneChanger.GetComponent<SceneChanger>().EndGame();
        }
        if (current <= waypoints.Length - 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, waypoints[current].transform.position, 10 * Time.deltaTime);
            Vector3 dir = waypoints[current].position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            //transform.rotate = Quaternion.AngleAxis(angle, Vector3.forward);
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            //Roatate current game object to face the target using a slerp function which adds some smoothing to the move
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 2.5f * Time.deltaTime);

            if (transform.position.x == waypoints[current].transform.position.x && transform.position.y == waypoints[current].transform.position.y)
            {
                current += 1;
            }
        }
        else
        {
            current = 0;
        }
    }
}
