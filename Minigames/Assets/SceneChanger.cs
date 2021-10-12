using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;  
using UnityEngine.SceneManagement;  

public class SceneChanger: MonoBehaviour {  
    public void MainMenu(){
        SceneManager.LoadScene(0);  
    }
    public void playFlocking() {  
        SceneManager.LoadScene(1);  
    }  
    public void playCollision(){
        SceneManager.LoadScene(2);  
    }
    public void playRayCast(){
        SceneManager.LoadScene(3);  
    }
    
} 