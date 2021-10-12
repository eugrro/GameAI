using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  

public class leader : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed = .6f;
    public float rotationSpeed = 10f;


    [Header("Targets")]
    public Transform[] targets;
    private int current = 0;

    public GameObject ally;
    public GameObject enemy;
    List<GameObject> allyArray = new List<GameObject>();
    private float[] deltaV = new float[]{45, 90, 135, 180, 225, 270, 315, 360};
    private float avgDistance = 0;
    private float actAvgDistance = 0;
    // Start is called before the first frame update
    void Start()
    {

        var lAngle = transform.rotation.eulerAngles.z;
        for(int i = 0; i < deltaV.Length;i++){
            GameObject newAlly = Instantiate(ally, transform.position + new Vector3(Mathf.Cos((deltaV[i] * Mathf.PI)/180), Mathf.Sin((deltaV[i] * Mathf.PI)/180), 0),  transform.rotation);
            allyArray.Add(newAlly);
            avgDistance += Vector3.Distance(newAlly.transform.position, transform.position);
        }
        avgDistance /= deltaV.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown( KeyCode.Alpha1 ))
        {
            SceneManager.LoadScene(0); 
        }else if(Input.GetKeyDown( KeyCode.Alpha2 )){
            SceneManager.LoadScene(1);
        }
        else if(Input.GetKeyDown( KeyCode.Alpha3 )){
            SceneManager.LoadScene(2);
        }
        var ratio = (avgDistance/actAvgDistance);
        transform.position = Vector2.MoveTowards(transform.position, targets[current].transform.position, ratio < .8 ? moveSpeed / 1.2f * Time.deltaTime : moveSpeed * Time.deltaTime);
        Vector3 dir = targets[current].position - transform.position;
        var lAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //transform.rotate = Quaternion.AngleAxis(angle, Vector3.forward);
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, lAngle - 90));
        //Roatate current game object to face the target using a slerp function which adds some smoothing to the move
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        if ((transform.position - targets[current].transform.position).magnitude < .5f)
        {
            current += 1;
        }
        
        if (current == targets.Length - 1)
        {
            current = 0;
        }
        actAvgDistance = 0;
        for(int i = 0; i < allyArray.Count;i++){
            GameObject allyMove = allyArray[i];
            actAvgDistance += Vector3.Distance(allyMove.transform.position, transform.position);
            var rVal = allyMove.GetComponent<AvoidObstacles>().DoRaycasts();
            Debug.Log(rVal);
            var targetLocation = transform.position + new Vector3(Mathf.Cos((deltaV[i] * Mathf.PI)/180), Mathf.Sin((deltaV[i] * Mathf.PI)/180), 0);
            if(Vector3.Distance(allyMove.transform.position, enemy.transform.position) < .5){
                allyArray.Remove(allyMove);
                Destroy(allyMove.gameObject);
            }
            //allyMove.transform.position = Vector2.MoveTowards(allyMove.transform.position,targetLocation + locationBias, moveSpeed * Time.deltaTime);
            var dir2 = allyMove.transform.position - (transform.position + new Vector3(Mathf.Cos((deltaV[i] * Mathf.PI)/180), Mathf.Sin((deltaV[i] * Mathf.PI)/180), 0));
            var lAngle2 = Mathf.Atan2(dir2.y, dir2.x) * Mathf.Rad2Deg;
            Debug.Log(lAngle2);
            if(rVal == 0){
                allyMove.transform.rotation = Quaternion.Slerp(allyMove.transform.rotation, Quaternion.Euler(new Vector3(0, 0, lAngle2 + 90 + rVal*2)), rotationSpeed * Time.deltaTime);  
            }else{
                allyMove.transform.rotation = Quaternion.Slerp(allyMove.transform.rotation, Quaternion.Euler(new Vector3(0, 0, allyMove.transform.rotation.eulerAngles.z + rVal)), rotationSpeed * Time.deltaTime);
            }
            allyMove.transform.position += allyMove.transform.up * Time.deltaTime * moveSpeed;
            //allyMove.transform.position += Vector3.Distance(allyMove.transform.position, transform.position) > avgDistance * 2 ? allyMove.transform.up * Time.deltaTime * moveSpeed * 3 : allyMove.transform.up * Time.deltaTime * moveSpeed;
        }
        actAvgDistance /= allyArray.Count;
        
    }
}
