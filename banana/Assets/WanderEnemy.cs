using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderEnemy : MonoBehaviour
{
    public GameObject target;
    public GameObject player;
    public SceneChanger SceneChanger;
    public float speed;
    private Vector3 banana;
    // Update is called once per frame
    void Start()
    {
        banana = target.transform.position;
    }
    void Update()
    {
        var range2 = Vector2.Distance(transform.position, player.transform.position);
        var range = Vector2.Distance(transform.position, banana);
        if (range > 100)
        {
            transform.Rotate(0, 0, Random.Range(-90f, 90.0f));
            transform.position = Vector2.MoveTowards(transform.position, banana, speed * Time.deltaTime);
            wait();
        }
        transform.position += transform.right * Time.deltaTime * speed;
        if (range2 < 10)
        {
            SceneChanger.GetComponent<SceneChanger>().EndGame();
        }
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(1);
    }
}
