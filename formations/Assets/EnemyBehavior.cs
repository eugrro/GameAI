using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public float speed = 3.0f;
    public float rotationOffset = -90.0f;
    public float attackRadius = 0.2f;
    public GameObject formMan;
    FormationManager fscript;

    private void AttackAgents()
    {
        Collider2D[] returnVal = Physics2D.OverlapCircleAll(transform.position, attackRadius);


        foreach (Collider2D collider in returnVal)
        {
            //Debug.Log(collider.gameObject);
            ScalableMovement possibleCollision = collider.gameObject.GetComponent<ScalableMovement>();
            if (possibleCollision == null) continue;
            fscript.RemoveCharacter(collider.gameObject);
        }
    }
    void Start()
    {
        formMan = GameObject.Find("Formation");
        fscript = (FormationManager) formMan.GetComponent("FormationManager");

    }
    void Update()
    {
        Vector3 cursorPos = Input.mousePosition;
        cursorPos.z = 0;
        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);

        cursorPos.x -= objectPos.x;
        cursorPos.y -= objectPos.y;

        float angle = Mathf.Atan2(cursorPos.y, cursorPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + rotationOffset));

        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPos.z = 0;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        AttackAgents();
    }
}
