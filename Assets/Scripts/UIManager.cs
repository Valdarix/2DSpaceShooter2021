using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Sprite[] _livesSprites;
    [SerializeField]
    private Image _livesImage;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartGameText;
    [SerializeField]
    private GameManager _gameManager;
    [SerializeField]
    private Image _shieldPower;
    [SerializeField]
    private Sprite[] _shieldPowerSprites;

    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _restartGameText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScoreUI(int currentScore)
    {
        _scoreText.text = "Score: " + currentScore;
    }

    public void UpdateLives(int currentLives)
    {
        _livesImage.sprite = _livesSprites[currentLives];

        if (currentLives == 0)
        {
            if (_gameManager != null)
            { 
                _gameManager.GameOver(); 
            }
            
            _restartGameText.gameObject.SetActive(true);
            _gameOverText.gameObject.SetActive(true);
            StartCoroutine(FlickerGameOverText());                  
        }
    }

    public void UpdateShield(int currentShield)
    {
        _shieldPower.sprite = _shieldPowerSprites[currentShield];
    }

    IEnumerator FlickerGameOverText()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);           
        }
        
    }
}
