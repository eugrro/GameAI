using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowThePath : MonoBehaviour
{

    public Transform[] waypoints;
    public SceneChanger SceneChanger;
    private float moveSpeed = 2f;
    public int current = 0;
    public float rayCastDistance = 2;
    public float rotationSpeed = 2.5f;
    private bool centerHit = false;
    private bool leftHit = false;
    private bool rightHit = false;
    private float angleChange = 25;

    public bool isCenterHit(){
        return centerHit;
    } 
    public bool isLeftHit(){
        return leftHit;
    }
    public bool isRightHit(){
        return rightHit;
    }
    // Use this for initialization
    void Start()
    {

        // Set position of Enemy as position of the first waypoint
        this.transform.position = waypoints[current].transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        float angleBias = 0;
        var angle1 = transform.rotation.eulerAngles.z;
        if (Physics.Raycast(this.transform.position,new Vector3(Mathf.Cos((angle1 * Mathf.PI)/180), Mathf.Sin((angle1 * Mathf.PI)/180), 0), rayCastDistance)){
            moveSpeed = 1f;
            centerHit = true;
            angleBias += angleChange;
        }else{
            moveSpeed = 2f;
            centerHit = false;
        }
        
        if (Physics.Raycast(this.transform.position, new Vector3(Mathf.Cos(((angle1 - 30) * Mathf.PI)/180), Mathf.Sin(((angle1 - 30) * Mathf.PI)/180), 0), rayCastDistance)){
            rightHit = true;
            angleBias += angleChange;
        }
        else{ 
            rightHit = false;
        }
        
        if (Physics.Raycast(this.transform.position,new Vector3(Mathf.Cos(((angle1 + 30) * Mathf.PI)/180), Mathf.Sin(((angle1 + 30) * Mathf.PI)/180), 0), rayCastDistance)){
            leftHit = true;
            angleBias -= angleChange;
        }
        else{ 
            leftHit = false;
        }
        
        if (Input.GetKey(KeyCode.Q)){
            SceneChanger.MainMenu();
        }
        if (current <= waypoints.Length - 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, waypoints[current].transform.position, moveSpeed * Time.deltaTime);
            Vector3 dir = waypoints[current].position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleBias;
            //transform.rotate = Quaternion.AngleAxis(angle, Vector3.forward);
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            //Roatate current game object to face the target using a slerp function which adds some smoothing to the move
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

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
