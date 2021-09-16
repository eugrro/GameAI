using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour
{
    public static ScoreHandler instance;
    public SceneChanger SceneChanger;
    public Text text;
    private int score;
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null){
            instance = this;
        }
    }

    public void ChangeScore(int bananaCount){
        score += bananaCount;
        text.text = "x" + score.ToString();
        if(score == 3){
            SceneChanger.GetComponent<SceneChanger>().WinGame();
        }
    }
}
