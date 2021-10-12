using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayer : MonoBehaviour
{
    public float speed;
    public SceneChanger SceneChanger;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q)){
            SceneChanger.MainMenu();
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 position = this.transform.position;
            position.x -= speed * Time.deltaTime;
            this.transform.position = position;
            this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 position = this.transform.position;
            position.x += speed * Time.deltaTime;
            this.transform.position = position;
            this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 position = this.transform.position;
            position.y += speed * Time.deltaTime;
            this.transform.position = position;
            this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 position = this.transform.position;
            position.y -= speed * Time.deltaTime;
            this.transform.position = position;
            this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
        }
        if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 315));
        }
        if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 225));
        }
        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 135));
        }
        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 45));
        }
    }
}
