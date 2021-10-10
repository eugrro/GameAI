using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineController : MonoBehaviour
{
    LineRenderer lr;
    public Text txt;
    public GameObject main;
    public float lineLen = 2;
    public float angleBias = 0;
    public bool changeColors;
    public FollowThePath mainScript;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        if(changeColors) mainScript = main.GetComponent<FollowThePath>();
        
    }
    void turnRed(){
        lr.startColor = Color.red;
        lr.endColor = Color.red;
    }
    void turnWhite(){
        lr.startColor = Color.white;
        lr.endColor = Color.white;
    }
    // Update is called once per frame
    void Update()
    {
        if(!changeColors && txt.text == "Press 's' to change avoidance method: Cone"){
            lr.enabled = true;
        }else if(!changeColors){
            lr.enabled = false;
        }
        if(changeColors){
            if(angleBias == 0 && mainScript.isCenterHit()) turnRed();
            else if(angleBias < 0 && mainScript.isRightHit()) turnRed();
            else if(angleBias > 0 && mainScript.isLeftHit()) turnRed();
            else turnWhite();
        }else turnWhite();
        var angle = main.transform.rotation.eulerAngles.z + angleBias;
        lr.SetPosition(0, main.transform.position);
        lr.SetPosition(1, main.transform.position + new Vector3(Mathf.Cos((angle * Mathf.PI)/180), Mathf.Sin((angle* Mathf.PI)/180), 0) * lineLen);
    }
}
