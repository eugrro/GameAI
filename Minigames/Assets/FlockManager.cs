using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public int numFlock;
    public GameObject flocker;
    public SceneChanger SceneChanger;
    public GameObject MainPlayer;
    public GameObject[] flockers;
    public float spawnDistance = 15;
    public float flockSpeed = 5;
    public float neighborDistance;
    public float rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        numFlock++;
        SpawnFlock();
    }
    void SpawnFlock() {
        flockers = new GameObject[numFlock];
        flockers[0] = flocker;
        for(int i = 1; i < numFlock;i++){
            GameObject newFlocker = Instantiate(flocker, transform.position + new Vector3(Random.Range(-spawnDistance, spawnDistance), Random.Range(-spawnDistance, spawnDistance), 0),  Quaternion.identity);
            flockers[i] = newFlocker;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q)){
            SceneChanger.MainMenu();
        }
        Vector3 avgPosition = MainPlayer.transform.position;
        Vector3 avoidPostion = Vector3.zero;
        for(int i = 0;i<numFlock;i++){
            GameObject curr = flockers[i];
            //get average Position
            avgPosition += curr.transform.position;
            for(int j = 0; j < numFlock;j++){
                if(i != j && Vector3.Distance(curr.transform.position, flockers[j].transform.position) < neighborDistance){
                    avoidPostion += (curr.transform.position - flockers[j].transform.position)*5;
                }
        
            }
            float userWeight = 5;
            Vector3 direction = (avgPosition/numFlock + avoidPostion - curr.transform.position + MainPlayer.transform.position * userWeight)/userWeight;
            //Calculate direction

            float angle = 0;
            angle = Mathf.Rad2Deg * Mathf.Atan2((direction.y-curr.transform.position.y),(direction.x - curr.transform.position.x));
            //Debug.Log(curr.transform.position + "  " + direction + "  " + angle);
            curr.transform.rotation = Quaternion.Slerp(curr.transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime);
            //move forward
            curr.transform.Translate(Time.deltaTime * flockSpeed, 0, 0); 
        }
    }
    
}
