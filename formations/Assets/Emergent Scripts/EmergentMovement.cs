using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  

public class EmergentMovement : MonoBehaviour
{
    public int idNumber;
    public float speed = 2.0f;
    private float searchRadius = 3.0f;
    private float searchHalfAngle = 65.0f;
    public float evadeDist = 0.2f;

    public PathInfo pathInfo;
    public GameObject parentAgent = null;
    public int numChildren = 0;

    public LayerMask agentLayerMask;
    public LayerMask raycastLayerMask;
    public GameObject leftRayTrans;
    public GameObject rightRayTrans;
    public GameObject centerRayTrans;

    private Vector3 currentMovement;
    private Vector3 currentTarget;
    private int currentTargetIndex = 0;
    private int numTargets;
    private int maxChildren = 1;

    private Vector3 DoRaycasts(){
        Vector2 result = Vector3.zero;
        var l = Raycast(ref leftRayTrans);
        var r = Raycast(ref rightRayTrans);
        var c = Raycast(ref centerRayTrans);
        int num = (l.collider != null ? 1 : 0) + (r.collider != null ? 1 : 0);
        
        result += l.collider != null ? new Vector2(rightRayTrans.transform.position.x, rightRayTrans.transform.position.y) : Vector2.zero;
        result += r.collider != null ? new Vector2(leftRayTrans.transform.position.x, leftRayTrans.transform.position.y) : Vector2.zero;

        result /= num > 0 ? num : 1;

        if(r.collider != null && l.collider != null){
            float distanceToRight = (r.point - new Vector2(transform.position.x, transform.position.y)).magnitude;
            float distanceToLeft = (l.point - new Vector2(transform.position.x, transform.position.y)).magnitude;

            float amount = distanceToLeft > distanceToRight ? 0 : 1;


            return Vector3.Lerp(leftRayTrans.transform.position,
                                rightRayTrans.transform.position,
                                amount);
        }
        if(c.collider != null && r.collider == null && l.collider == null) result = leftRayTrans.transform.position;

        Vector3 v3 = new Vector3(result.x, result.y, 0);

        return (v3);
    }

    private RaycastHit2D Raycast(ref GameObject g){
        return Physics2D.Raycast(transform.position, g.transform.position - transform.position, (g.transform.position - transform.position).magnitude, raycastLayerMask);
    }

    private bool ReachedTarget(){
        float dist = (transform.position - pathInfo.targets[currentTargetIndex]).magnitude;
        if(dist < 0.75f){
            currentTargetIndex++;
            if (parentAgent == null && currentTargetIndex < numTargets) {
                currentTarget = pathInfo.targets[currentTargetIndex];
                return true;
            }
        }
        return false;
    }

    private GameObject FindAgentToFollow() {
        Collider2D[] returnVal = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        GameObject g = null;
        int lowestNum = 10000;

        foreach (Collider2D collider in returnVal) {
            EmergentMovement possibleParent = collider.gameObject.GetComponent<EmergentMovement>();
            if (possibleParent == null) continue;
            Vector2 direction = (possibleParent.transform.position - transform.position).normalized;
            float dotProduct = Vector2.Dot(transform.up, direction);
            float cos = Mathf.Cos(searchHalfAngle * Mathf.Deg2Rad);

            if (dotProduct > cos && possibleParent.idNumber < idNumber && possibleParent.numChildren < maxChildren) {
                if (possibleParent.idNumber < lowestNum) {
                    possibleParent.numChildren++;
                    lowestNum = possibleParent.idNumber;
                    g = collider.gameObject;
                }
            }
        }
        return g;
    }

    private GameObject FindClosestAgent() {
        Collider2D[] returnVal = Physics2D.OverlapCircleAll(transform.position, searchRadius);

        GameObject g = null;
        float nearestDist = 1.0f;

        foreach (Collider2D collider in returnVal) {
            EmergentMovement possibleCollision = collider.gameObject.GetComponent<EmergentMovement>();
            if (possibleCollision == null) continue;
            
            var dist = Vector2.Distance(transform.position, collider.gameObject.transform.position);
            if (possibleCollision.idNumber < idNumber && dist < nearestDist) {
                nearestDist = dist;
                g = collider.gameObject;
            }
        }
        return g;
    }

    private Vector3 Evade(Vector3 currentTarget, Vector3 obstacle, float magnitude = 1.0f) {
        Vector3 direction = (obstacle - transform.position).normalized;
        float dotProduct = Vector3.Dot(direction, transform.right);
        if (dotProduct > 0) {
            return (Vector3)currentTarget - transform.right*0.5f;
        } else {
            return (Vector3)currentTarget + transform.right*0.5f;
        }
    }

    private bool startUpdate = false;
    IEnumerator StartingWaitCoroutine() {
        yield return new WaitForSeconds(idNumber*0.1f);
        startUpdate = true;
    }

    void Start()
    {
        GameObject pathgo = GameObject.Find("PathPoints");
        pathInfo = pathgo.GetComponent<PathInfo>();
        StartCoroutine(StartingWaitCoroutine());
        currentMovement = transform.up;
        numTargets = pathInfo.targets.Length;
    }

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
        if(!startUpdate) return; // wait until ready

        if (currentTargetIndex > 5) { // switch to two-lane and reset parents
            maxChildren = 2;
            numChildren = 0;
            parentAgent = null;
        }
        if (parentAgent == null) {
            parentAgent = FindAgentToFollow(); // look for parent if not currently following one
        }

        if (parentAgent == null) {
            currentTarget = pathInfo.targets[currentTargetIndex]; // follow path if no parent found
            print("Agent " + idNumber + " is following the path\n");
        } else {
            currentTargetIndex = parentAgent.GetComponent<EmergentMovement>().currentTargetIndex;
            currentTarget = parentAgent.transform.position; // follow parent if found
            EmergentMovement e = parentAgent.GetComponent<EmergentMovement>();
            print("Agent " + idNumber + " is following Agent " + e.idNumber + "\n");
        }

        var v = DoRaycasts();
        Vector3 directionToAim = v == Vector3.zero ? new Vector3(currentTarget.x, currentTarget.y, 0) - transform.position : v - transform.position;

        GameObject closestAgent = FindClosestAgent();
        Vector3 evasion = Vector3.zero;
        if (closestAgent != null && Vector2.Distance(transform.position, closestAgent.transform.position) < evadeDist) {
            directionToAim += Evade(directionToAim, closestAgent.transform.position);
        }

        directionToAim.Normalize();
        currentMovement.Normalize();
        currentMovement = Vector3.Lerp(currentMovement, directionToAim, 5.0f * Time.deltaTime);
        transform.Translate(currentMovement * speed * Time.deltaTime, Space.World);
        transform.up = currentMovement;
  
        ReachedTarget();
    }
}
