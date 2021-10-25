using UnityEngine;
using UnityEngine.SceneManagement;

 public class UserInput : MonoBehaviour
 {
     public float moveSpeed = 0.25f;
     // public float rotationRate = 15f;
     // public Transform target;
 
     private Rigidbody2D rigidbody;
     private Vector2 moveInput;
     public static Vector3Int clickedPosition;
     public GameObject MapHandler;
 
     private void Awake() {
        clickedPosition = new Vector3Int(0, 0, 0);
         TryGetComponent(out rigidbody);
     }
    
    public void changeScene(){

    }
 
     private void FixedUpdate() {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rigidbody.position += moveInput * moveSpeed;

     }
    void Update() {
        if (Input.GetMouseButtonDown (0)) {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickedPosition = new Vector3Int((int)worldPosition.x, (int)worldPosition.y, 0);
        }
        if (Input.GetKeyDown(KeyCode.R)){
         if(SceneManager.GetActiveScene().buildIndex == 0){
             Debug.Log("Changing to Scene 1");
             SceneManager.LoadScene(1);
         }else{
             Debug.Log("Changing to Scene 0");
             SceneManager.LoadScene(0);
         }
        }
        
        /*if (Input.GetKeyDown(KeyCode.Alpha1)){
            MapHandler.map
        }*/
    }
 }