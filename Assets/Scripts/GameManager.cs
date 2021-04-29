using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool isGameOver = false;
    private int _wave = 0;
    [SerializeField] private UIManager _ui;

    private void Update()
    {        
        if (Input.GetKeyDown(KeyCode.R) && isGameOver)
        {
            SceneManager.LoadScene(1);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void GameOver()
    {
        isGameOver = true;
    }   
    public void NewWave()
    {
        _wave++;
        _ui.UpdateWaveUI(_wave);
    }
    public int GetWave()
    {
        return _wave;
    }
  
}
