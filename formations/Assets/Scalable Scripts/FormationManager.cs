using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationManager : MonoBehaviour
{
    public Transform center;
    public GameObject agentPrefab;
    public int numA;
    public float radius;

    struct SlotAssignment
    {
        public GameObject character;
        public int slotNumber;
    }

    struct myTransform
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    bool removing;
    List<SlotAssignment> slotAssignments;
    float offset;

    int CalcNumSlots(List<SlotAssignment> a)
    {
        int filled = 0;
        foreach (SlotAssignment s in a)
        {
            if (s.slotNumber >= filled)
            {
                filled = s.slotNumber;
            }
        }

        int numSlots = filled + 1;

        return numSlots;
    }

    myTransform GetSlotLocation(int i)
    {
        float angleCircle = i / slotAssignments.Count * Mathf.PI * 2;
        float radius = 0.3f / Mathf.Sin(Mathf.PI / slotAssignments.Count);

        myTransform tr = new myTransform();
        tr.position = new Vector3(radius * Mathf.Cos(angleCircle), radius * Mathf.Sin(angleCircle), 0);
        tr.rotation = Quaternion.Euler(angleCircle, 0, 0);

        return tr;
    }
    
    void GetDriftOffset(List<SlotAssignment> a)
    {
        Vector3 crot = center.rotation.eulerAngles;
        foreach (SlotAssignment s in a)
        {
            myTransform loc = GetSlotLocation(s.slotNumber);
            center.position += loc.position;
            crot += loc.rotation.eulerAngles;
        }
        int numAssign = a.Count;
        center.position /= numAssign;
        crot /= numAssign;
        center.rotation = Quaternion.Euler(crot);
    }

    bool AddCharacter(GameObject c)
    {
        int occupied = slotAssignments.Count;

        SlotAssignment slot = new SlotAssignment();
        slot.character = c;
        slot.slotNumber = occupied;
        slotAssignments.Add(slot);

        //UpdateSlotAssignments();
        //GetDriftOffset(slotAssignments);
        return true;

    }

    public void RemoveCharacter(GameObject c)
    {
        if (!c) { return; }
        removing = true;
        int slot = 100;
        for(int i = 0; i < slotAssignments.Count; i++)
        {
            if (c && i < slotAssignments.Count && slotAssignments[i].character == c)
            {
                slot = slotAssignments[i].slotNumber;
                slotAssignments.RemoveAt(i);
                /*for (int j = slot; j < slotAssignments.Count; j++)
                {
                    slotAssignments[j].slotNumber = j;
                }*/
                if (!c) { continue; }
                Destroy(c);
                break;
            }
        }
        removing = false;
        //UpdateSlotAssignments();
        //GetDriftOffset(slotAssignments);
    }

    void UpdateSlots()
    {
        Vector3 rot = center.rotation.eulerAngles;

        //var angleBias = transform.positoi
        for (int i = 0; i < slotAssignments.Count; i++)
        {
            if (removing)
            {
                return;
            }
            //myTransform relLoc = GetSlotLocation(slotAssignments[i].slotNumber);
            
            var centerAngle = 0;//center.transform.rotation.eulerAngles.z;
            var dA = (360 / numA) * i;
            //Debug.Log(dA);
            var newLoc = center.transform.position + new Vector3(Mathf.Cos(((centerAngle + dA) * Mathf.PI) / 180), Mathf.Sin(((centerAngle + dA) * Mathf.PI) / 180), 0)*radius;

            // Set slotAssignments[i].character target to newLoc
            if (i >= slotAssignments.Count) { return; }
            slotAssignments[i].character.GetComponent<ScalableMovement>().currentTarget = newLoc;
            slotAssignments[i].character.transform.position = Vector3.MoveTowards(slotAssignments[i].character.transform.position, newLoc, 1.5f * Time.deltaTime);
            slotAssignments[i].character.transform.rotation = center.rotation;
        }
    }


    void Start()
    {
        radius = 0.4f;
        removing = false;
        slotAssignments = new List<SlotAssignment>();
        numA = 12;
        for (int i = 0; i < numA; i++)
        {
            float centerAngle = (360 / numA) * i;
            Vector3 spawnPos = new Vector3(Mathf.Cos(((centerAngle) * Mathf.PI) / 180), Mathf.Sin(((centerAngle) * Mathf.PI) / 180), 0);
            GameObject a = Instantiate(agentPrefab, center.position + spawnPos, Quaternion.identity);
            AddCharacter(a);
        }
        UpdateSlots();
    }
    void Update()
    {
        if (slotAssignments.Count != numA)
        {
            numA = slotAssignments.Count;
        }
        UpdateSlots();
    }
}
