using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Text _scoreText;
    void Start()
    {
        _scoreText.text = "Score: " + 0; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScoreUI(int currentScore)
    {
        _scoreText.text = "Score: " + currentScore;
    }
}
