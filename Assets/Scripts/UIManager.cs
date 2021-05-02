using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Text _scoreText;
    [SerializeField] private Sprite[] _livesSprites;
    [SerializeField] private Image _livesImage;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Text _restartGameText;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Image _shieldPower;
    [SerializeField] private Sprite[] _shieldPowerSprites;
    [SerializeField] private Text _ammoCountText;
    [SerializeField] private Image _thrusterPower;
    [SerializeField] private Sprite[] _thrusterPowerBar;
    [SerializeField] private Text _thrusterPowerText;
    [SerializeField] private Text _waveText;
    [SerializeField] private Image missleUI1;
    [SerializeField] private Image missleUI2;
    [SerializeField] private Text hintText;

    private void Start()
    {
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _restartGameText.gameObject.SetActive(false);        
    }

    public void UpdateScoreUI(int currentScore)
    {
        _scoreText.text = "Score: " + currentScore;
    }

    public void UpdateLives(int currentLives)
    {
        _livesImage.sprite = _livesSprites[currentLives];

        if (currentLives != 0) return;
        if (_gameManager != null)
        { 
            _gameManager.GameOver("Game Over!"); 
        }            
        _restartGameText.gameObject.SetActive(true);
        _gameOverText.gameObject.SetActive(true);
        StartCoroutine(FlickerText(_gameOverText, "Game Over"));
    }

    public void GameOver(string gameOverText)
    {
        _gameOverText.text = gameOverText;
        _gameOverText.gameObject.SetActive(true);
    }

    public void UpdateShield(int currentShield)
    {
        _shieldPower.sprite = _shieldPowerSprites[currentShield];
    }

    public void UpdateAmmoCount(int ammoCount, int maxAmmo)
    {
        _ammoCountText.color = ammoCount switch
        {
            0 => Color.red,
            _ => Color.white
        };
        _ammoCountText.text = "Ammo: " + ammoCount + "/" + maxAmmo;
    }

    private static IEnumerator FlickerText(Text uiTextObject, string uiText)
    {
        while (true)
        {
            if (uiTextObject.text == "YOU WIN!")
            {
                uiTextObject.color = Color.green; 
            }
            uiTextObject.text = uiText;
            yield return new WaitForSeconds(0.5f);
            uiTextObject.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void UpdateWaveUI(int currentWave)
    {
        _waveText.text = "Wave: " + currentWave;
    }

    public void UpdateHintText(bool status)
    {
        hintText.gameObject.SetActive(status);
    }

    public void UpdateMissileUI(int missileCount)
    {
        switch (missileCount)
        {
            case 1:
                missleUI1.gameObject.SetActive(true);
                missleUI2.gameObject.SetActive(false);
                break;
            case 2: 
                missleUI1.gameObject.SetActive(true);
                missleUI2.gameObject.SetActive(true);
                break; 
            case 0: 
                missleUI1.gameObject.SetActive(false);
                missleUI2.gameObject.SetActive(false);
                break;
        }
    }

    public void UpdateThrusterUI(int currentPower)
    {
        if (currentPower is < 0 or > 15) return;
        switch (currentPower)
        {
            case 0:
                _thrusterPowerText.text = "Recharging";
                _thrusterPowerText.color = Color.red;
                break;
            case 15:
                _thrusterPowerText.color = Color.green;
                _thrusterPowerText.text = "Full Power";
                break;
        }
        _thrusterPower.sprite = _thrusterPowerBar[currentPower];
    }
}
