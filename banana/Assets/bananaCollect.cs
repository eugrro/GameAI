using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bananaCollect : MonoBehaviour
{
    public ScoreHandler instance;
    private void OnTriggerEnter2D(Collider2D other){
        if(other.transform.tag == "banana"){
            Destroy(other.gameObject);
            instance.ChangeScore(1);
        }
    }
}
