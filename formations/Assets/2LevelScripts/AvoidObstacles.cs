using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidObstacles : MonoBehaviour
{
    private Vector3 currentMovement;
    public float accelerationFloat = .85f;

    public float speed = 3;


    [Header("Raycasts")]
    public float normalDistance = 1;
    public LayerMask raycastLayerMask;
    public GameObject leftRayTrans;
    public GameObject rightRayTrans;
    public GameObject centerRayTrans;

    public Material green;
    public Material red;
    private Vector3 directionToAim; 

    private void Start() {
        currentMovement = transform.up;
    }

    private void Update() {
        var v = DoRaycasts();
        //directionToAim = v == Vector3.zero ? new Vector3(-10.0f, -10.0f, 0.0f) - transform.position : v - transform.position;

        //directionToAim.Normalize();
        //currentMovement.Normalize();
        //currentMovement = Vector3.Lerp(currentMovement, directionToAim, accelerationFloat * Time.deltaTime);
        /*transform.Translate(currentMovement * speed * Time.deltaTime, Space.World);
        transform.up = currentMovement;
        */

        //ReachedTarget();
    }

    public float DoRaycasts(){
        Vector2 result = Vector3.zero;
        var l = Raycast(ref leftRayTrans);
        var r = Raycast(ref rightRayTrans);
        var c = Raycast(ref centerRayTrans);
        int num = (l.collider != null ? 1 : 0) + (r.collider != null ? 1 : 0);
        
        result += l.collider != null ? new Vector2(rightRayTrans.transform.position.x, rightRayTrans.transform.position.y) : Vector2.zero;
        result += r.collider != null ? new Vector2(leftRayTrans.transform.position.x, leftRayTrans.transform.position.y) : Vector2.zero;

        result /= num > 0 ? num : 1;
        float dRight = 1.1f;
        float dLeft = 1.1f;
        float dCenter = 1.1f;
        if(r.collider != null){
            dRight = (r.point - new Vector2(transform.position.x, transform.position.y)).magnitude;
        }if(l.collider != null){
            dLeft = (l.point - new Vector2(transform.position.x, transform.position.y)).magnitude;
        }
        if(c.collider != null){
            dCenter = (c.point - new Vector2(transform.position.x, transform.position.y)).magnitude;
        }
        float ratio = 0f;
        float mult = 10;
        if(dRight > dLeft){
            ratio = dRight/dLeft;
            return -ratio * mult;
        }else if(dRight != dLeft){
            ratio = dLeft/dRight;
            return ratio * mult;
        }else if(dCenter < .5f && dRight < .5f && dLeft < .5f){
            return -1;
        }
        else{
            return 0;
        }

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

    /*private bool ReachedTarget(){
        if((transform.position - currentTarget.position).magnitude < .5f){
            currentTargetIndex++;
            currentTarget = targets[currentTargetIndex];
            return true;
        }
        return false;
    }*/
}
