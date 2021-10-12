using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part3Movement : MonoBehaviour
{
    private Vector3 currentMovement;
    public float accelerationFloat = .85f;

    public float speed = 3;

    [Header("Targets")]
    public Transform[] targets;

    private Transform currentTarget;
    private int currentTargetIndex = 0;

    [Header("Raycasts")]
    public float normalDistance = 1;
    public LayerMask raycastLayerMask;
    public GameObject leftRayTrans;
    public GameObject rightRayTrans;
    public GameObject centerRayTrans;

    public Material green;
    public Material red;

    private void Start() {
        currentMovement = transform.up;
        currentTarget = targets[0];
    }

    private void Update() {
        Vector3 directionToAim; 
        var v = DoRaycasts();
        directionToAim = v == Vector3.zero ? currentTarget.position - transform.position : v - transform.position;

        directionToAim.Normalize();
        currentMovement.Normalize();
        currentMovement = Vector3.Lerp(currentMovement, directionToAim, accelerationFloat * Time.deltaTime);
        transform.Translate(currentMovement * speed * Time.deltaTime, Space.World);
        transform.up = currentMovement;

        ReachedTarget();
    }

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
        var LR = g.GetComponent<LineRenderer>();
        Vector3 [] poses = new Vector3[2];
        poses[0] = transform.position;
        poses[1] = g.transform.position;
        LR.SetPositions(poses);

        var r = Physics2D.Raycast(transform.position, g.transform.position - transform.position, (g.transform.position - transform.position).magnitude, raycastLayerMask);
        if(r.collider != null){
            LR.material = red;
        }
        else{
            LR.material = green;
        }
        return r;
    }

    private bool ReachedTarget(){
        if((transform.position - currentTarget.position).magnitude < .5f){
            currentTargetIndex++;
            currentTarget = targets[currentTargetIndex];
            return true;
        }
        return false;
    }
}
