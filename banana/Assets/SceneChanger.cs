using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;  
using UnityEngine.SceneManagement;  

public class SceneChanger: MonoBehaviour {  
    public void MainMenu(){
        SceneManager.LoadScene(0);  
    }
    public void PlayGame() {  
        SceneManager.LoadScene(1);  
    }  
    public void EndGame(){
        SceneManager.LoadScene(2);  
    }
    public void WinGame(){
        SceneManager.LoadScene(3);  
    }
    
} 