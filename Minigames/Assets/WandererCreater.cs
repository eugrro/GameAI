using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandererCreater : MonoBehaviour
{
    public GameObject banana;
    public GameObject WandererEnemy;
    // Start is called before the first frame update
    void Start()
    {
        float randomX = 0;
        float randomY = 0;
        for (int i = 0; i < 50; i++)
        {

            randomX = Random.Range(-100.0f, 100.0f);
            randomY = Random.Range(-100.0f, 100.0f);

            Instantiate(WandererEnemy, banana.transform.position + new Vector3(randomX, randomY, 0), new Quaternion(Random.Range(0f, 360.0f), Random.Range(0f, 360.0f), 0, 0));
        }

    }

}
