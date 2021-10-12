using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopCollision : MonoBehaviour
{
    public GameObject b;
    public GameObject otherCenter;
    public BottomCollision s;
    public GameObject[] bottoms;
    public Transform[] waypoints;
    public float rotationSpeed = 2.5f;
    public int current = 0;
    private float moveSpeed = 1f;
    private float xDist = .4f;
    private float yDist = .4f;
    private float[,] locationTargets;
    private float[] intersectionPoints = new float[4] {-7f, -4.2f, -1.45f, 1.4f};
    // Start is called before the first frame update
    void Start()
    {
        locationTargets = new float[,] {{xDist, 0f}, {-xDist, 0f}, {xDist, yDist}, {-xDist, yDist}, {0f, yDist}};
        SpawnFlock();
    }
    void SpawnFlock() {
        bottoms = new GameObject[6];
        for(int i = 0; i < 5;i++){
            GameObject newB = Instantiate(b, transform.position + new Vector3(locationTargets[i,0], locationTargets[i,1], 0),  Quaternion.identity);
            bottoms[i] = newB;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)){
            transform.position = new Vector3(-8.5f, 2.5f, 0);
            for(int i = 0; i < 5;i++){
                bottoms[i].transform.position = transform.position + new Vector3(locationTargets[i,0], locationTargets[i,1], 0);
            }
            current = 0;
        }
        if (current <= waypoints.Length - 1)
        {  
        float angleBias = 0;
        Vector3 theirPos = otherCenter.transform.position;
        if(s.avoidanceType == 0){
            float currAngle = transform.rotation.eulerAngles.z;
            float xPosL = Mathf.Cos(((currAngle-40) * Mathf.PI)/180) * 2 + transform.position.x;
            float yPosL =  Mathf.Sin(((currAngle-40) * Mathf.PI)/180) * 2 + transform.position.y;
            float xPosR = Mathf.Cos(((currAngle+40) * Mathf.PI)/180) * 2 + transform.position.x;
            float yPosR =  Mathf.Sin(((currAngle+90) * Mathf.PI)/180) * 2 + transform.position.y;
            if(((theirPos.x > xPosL && theirPos.x < xPosR) || (theirPos.x < xPosL && theirPos.x > xPosR)) && ((theirPos.y > yPosL && theirPos.y < yPosR) || (theirPos.y < yPosL && theirPos.y >yPosR))){
                moveSpeed = .5f;
                angleBias = 45;
                transform.position = Vector2.MoveTowards(transform.position, transform.position + transform.forward - new Vector3(.2f, 0, 0), moveSpeed * Time.deltaTime);
            }else{
                moveSpeed = 1f;
                transform.position = Vector2.MoveTowards(transform.position, waypoints[current].transform.position, moveSpeed * Time.deltaTime);
            }
        }else{
            Vector3 avoidPosition = new Vector3(intersectionPoints[current], 0, 0); //Predicted collisions locations are known
            if((current % 2 == 0 && transform.position.y > 0) || (current % 2 == 1 && transform.position.y < 0)){
                transform.position = Vector2.MoveTowards(transform.position, avoidPosition + new Vector3(1f, 0, 0), moveSpeed * Time.deltaTime);
            }else{
                transform.position = Vector2.MoveTowards(transform.position, waypoints[current].transform.position, moveSpeed * Time.deltaTime);
            }
        }

        
            Vector3 dir = waypoints[current].position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - angleBias;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

            if (transform.position.x == waypoints[current].transform.position.x && transform.position.y == waypoints[current].transform.position.y)
            {
                current += 1;
            }

            for(int i = 0; i < 5; i++){
                GameObject curr = bottoms[i];
                curr.transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
                curr.transform.position = Vector2.MoveTowards(curr.transform.position, transform.position + new Vector3(locationTargets[i,0], locationTargets[i,1], 0), moveSpeed * Time.deltaTime);
            }
        }
    }
}
