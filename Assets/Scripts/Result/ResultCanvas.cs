using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ResultCanvas : MonoBehaviour
{
    [SerializeField] Text _title;
    [SerializeField] Text _score;
    [SerializeField] Text _stats;
    [SerializeField] Text _randomSeed;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Game");
        }
    }

    public void Init(GameOverReason ggReason, GameController gameController)
    {
        _title.text = ggReason == GameOverReason.Finished ? "SUCCESS" : "FAILED...";
        _score.text = gameController.score.ToString("000000000");
        _stats.text = gameController.lv + "\n"+
            gameController.maxCombo + "\n" +
            gameController.steppedFloor + "\n";
        _randomSeed.text = gameController.randomSeed.ToString();
        // _score.text = 
    }
}