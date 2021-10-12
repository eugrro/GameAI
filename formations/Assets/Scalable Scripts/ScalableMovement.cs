using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalableMovement : MonoBehaviour
{
    public int idNumber;
    public float speed = 2.0f;
    public float accelerationFloat = 2.2f;
    public float evadeDist = 0.2f;
    private float searchRadius = 0.3f;


    public LayerMask agentLayerMask;
    public LayerMask raycastLayerMask;
    public GameObject leftRayTrans;
    public GameObject rightRayTrans;
    public GameObject centerRayTrans;

    private Vector3 currentMovement;
    public Vector3 currentTarget;
    private int numTargets;

    public GameObject formMan;
    FormationManager fscript;

    private Vector3 DoRaycasts()
    {
        Vector2 result = Vector3.zero;
        var l = Raycast(ref leftRayTrans);
        var r = Raycast(ref rightRayTrans);
        var c = Raycast(ref centerRayTrans);
        int num = (l.collider != null ? 1 : 0) + (r.collider != null ? 1 : 0);

        result += l.collider != null ? new Vector2(rightRayTrans.transform.position.x, rightRayTrans.transform.position.y) : Vector2.zero;
        result += r.collider != null ? new Vector2(leftRayTrans.transform.position.x, leftRayTrans.transform.position.y) : Vector2.zero;

        result /= num > 0 ? num : 1;

        if (r.collider != null && l.collider != null)
        {
            float distanceToRight = (r.point - new Vector2(transform.position.x, transform.position.y)).magnitude;
            float distanceToLeft = (l.point - new Vector2(transform.position.x, transform.position.y)).magnitude;

            float amount = distanceToLeft > distanceToRight ? 0 : 1;


            return Vector3.Lerp(leftRayTrans.transform.position,
                                rightRayTrans.transform.position,
                                amount);
        }
        if (c.collider != null && r.collider == null && l.collider == null) result = leftRayTrans.transform.position;

        Vector3 v3 = new Vector3(result.x, result.y, 0);

        return (v3);
    }

    private RaycastHit2D Raycast(ref GameObject g)
    {
        return Physics2D.Raycast(transform.position, g.transform.position - transform.position, (g.transform.position - transform.position).magnitude, raycastLayerMask);
    }

    private GameObject FindClosestAgent()
    {
        Collider2D[] returnVal = Physics2D.OverlapCircleAll(transform.position, searchRadius);

        GameObject g = null;
        float nearestDist = 1.0f;
        int numRange = 0;

        foreach (Collider2D collider in returnVal)
        {
            ScalableMovement possibleCollision = collider.gameObject.GetComponent<ScalableMovement>();
            if (possibleCollision != null) continue;

            numRange++;
            var dist = Vector2.Distance(transform.position, collider.gameObject.transform.position);
            nearestDist = dist;
            g = collider.gameObject;
        }
        if (numRange > 0)
        {
            fscript.radius -= 0.01f;

        }
        return g;
    }

    private Vector3 Evade(Vector3 currentTarget, Vector3 obstacle, float magnitude = 1.0f)
    {
        Vector3 direction = (obstacle - transform.position).normalized;
        float dotProduct = Vector3.Dot(direction, transform.right);
        if (dotProduct > 0)
        {
            return (Vector3)currentTarget - transform.right * 0.5f;
        }
        else
        {
            return (Vector3)currentTarget + transform.right * 0.5f;
        }
    }

    void Start()
    {
        currentMovement = transform.up;
        formMan = GameObject.Find("Formation");
        fscript = (FormationManager)formMan.GetComponent("FormationManager");
    }

    void Update()
    {
        var v = DoRaycasts();

        //Evade(currentTarget, FindClosestAgent().transform.position);

    }

}
