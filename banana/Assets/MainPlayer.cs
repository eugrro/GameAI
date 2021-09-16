using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayer : MonoBehaviour
{
    public float speed;
    public GameObject chaserEnemy;
    public float enemySpawnRate;
    private WaitForSeconds waitForSeconds;
    public SceneChanger SceneChanger;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("SpawnEnemies");
        waitForSeconds = new WaitForSeconds(enemySpawnRate);
    }
    IEnumerator SpawnEnemies() {
        while(true){
            // execute block of code here
            float randomX = 0;
            float randomY = 0;
            while(randomX > -50 && randomX < 50){
                randomX = Random.Range(-100.0f, 100.0f);
            }
            while(randomY > -50 && randomY < 50){
                randomY = Random.Range(-100.0f, 100.0f);
            }
            Instantiate(chaserEnemy, transform.position + new Vector3(randomX, randomY, 0),  Quaternion.identity);
            yield return waitForSeconds;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 position = this.transform.position;
            position.x -= speed;
            this.transform.position = position;
            this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 position = this.transform.position;
            position.x += speed;
            this.transform.position = position;
            this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 position = this.transform.position;
            position.y += speed;
            this.transform.position = position;
            this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 position = this.transform.position;
            position.y -= speed;
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
        if(transform.position.x > 1000 || transform.position.x < -1000 || transform.position.y > 1000 || transform.position.y < -1000){
            SceneChanger.GetComponent<SceneChanger>().EndGame();
        }
    }
}
