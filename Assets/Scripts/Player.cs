using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    //Private Seialized
    [SerializeField]
    private float _speed = 5.0f;
    private float _speedBoostMultiplier = 1f;
    [SerializeField]
    private GameObject _playerShieldObject;
    // use a private for later powerup. 
    [SerializeField]
    private float _laserCooldownTimer = 0.5f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManger;
    [SerializeField]
    private bool isPowerUpEnabled = false;
    [SerializeField]
    private GameObject _powerUp;
    //Privates
    private bool _laserCanFire = true;
    private int _powerupID = -1; // 0 = Triple Shot 1 = Speedboost 2 = shields
    private int _score = 0;
    [SerializeField]
    private UIManager _ui;
    [SerializeField]
    private AudioClip _laserSoundFX;
    private AudioSource _audioFXSource;
    private Animator _animator;
    [SerializeField]
    private GameObject _explosion;

    void Start()
    {
        //take the current position and assign a start position of (0,0,0)
        transform.position = new Vector3(0,0,0);
        _spawnManger = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManger == null)
        {
            Debug.LogError("Player: Spawn Manager is NULL.");
        }
        _ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_ui == null)
        {
            Debug.LogError("Player: UIManager is NULL");
        }
        _audioFXSource = this.GetComponent<AudioSource>();   
        if (_audioFXSource == null)
        {
            Debug.LogError("Player: Audio Source is NULL");
        } 
        else
        {
            _audioFXSource.clip = _laserSoundFX;
        }

        _animator = gameObject.GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Player: Animator not set");
        }
        _score = 0;
    }

    // Update is called once per frame
    void Update()
    {         
        MovePlayer();

        CheckFireLaser();
    }


    void MovePlayer()
    {
        
        float horizontalInput = Input.GetAxis("Horizontal");        
        float verticalInput = Input.GetAxis("Vertical");        
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0).normalized;    
       
        transform.Translate(direction * (_speed * _speedBoostMultiplier) * Time.deltaTime);

        float yClamp = Mathf.Clamp(transform.position.y, -4f, 6f);
        transform.position = new Vector3(transform.position.x, yClamp, 0);

        if (transform.position.x >= 11.5 || (transform.position.x <= -11.5))
        {           
            transform.position = new Vector3(transform.position.x * -1, transform.position.y,0);            
        }     

    }
    void CheckFireLaser()
    {
   
        if (Input.GetKeyDown(KeyCode.Space) && _laserCanFire)
        {            
            Vector3 offset = new Vector3(0.0f,1.05f,0.0f);
            if (isPowerUpEnabled == true && _powerupID == 0)
            {
                Instantiate(_powerUp, (transform.position + offset), Quaternion.identity);
               
            } else 
            {
                Instantiate(_laserPrefab, (transform.position + offset), Quaternion.identity);
                
            }
          
            _audioFXSource.Play();
            _laserCanFire = false;
            StartCoroutine(LaserCooldownTimer());
        }
    }

    IEnumerator LaserCooldownTimer()
    {
        yield return new WaitForSeconds(_laserCooldownTimer);
        _laserCanFire = true;
    }

    public void DamagePlayer(int DamageAmount)
    {
        if (_lives > 0) // only damage player if they are alive and if not shielded
        {
            if (_playerShieldObject.activeInHierarchy == true)
            {
                _playerShieldObject.SetActive(false);
            }
            else
            {
                _lives = _lives - DamageAmount;        
                switch (_lives)
                {
                    case 0:
                        //Do nothing player will die 
                        break;
                    case 1:
                        gameObject.transform.Find("LeftWingDamage").transform.gameObject.SetActive(true);
                        break;
                    case 2:
                        gameObject.transform.Find("RightWingDamage").transform.gameObject.SetActive(true);
                        break;
                    case 3:
                        // do nothing full health. Later we can toggle these off
                        break;
                }
                    
            }
            _ui.UpdateLives(_lives);
            if (_lives <= 0)
            {
                _spawnManger.StopSpawning();
                gameObject.transform.Find("LeftWingDamage").transform.gameObject.SetActive(false);
                gameObject.transform.Find("RightWingDamage").transform.gameObject.SetActive(false);
                gameObject.transform.Find("Thruster").transform.gameObject.SetActive(false);
                gameObject.transform.Find("PlayerShield").transform.gameObject.SetActive(false);
                _speed = 0;
                GameObject exp = Instantiate(_explosion, transform.position, Quaternion.identity);
                exp.gameObject.GetComponent<Animator>().SetTrigger("CanExplode");
                Destroy(gameObject);              
            }
        }       
     }

    public void EnablePowerUp(int powerupID)
    {
        isPowerUpEnabled = true;
        _powerupID = powerupID;
        switch (_powerupID)
        {
            case 0:
                //Triple shot do nothing
                break;
            case 1:
                //speed boost 
                _speedBoostMultiplier = 2.0f;
                break;
            case 2:
                //shields
                if (_playerShieldObject != null)
                {
                    _playerShieldObject.SetActive(true);
                }
                break;
            default:
                //nothing
                break;
                
        }
        StartCoroutine(PowerUpTimer());
    }

    IEnumerator PowerUpTimer()
    {
        yield return new WaitForSeconds(5.0f);
        isPowerUpEnabled = false;
        switch (_powerupID)
        {
            case 0:
                //Triple shot do nothing
                break;
            case 1:
                //speed boost 
                _speedBoostMultiplier = 1.0f;
                break;
            case 2:
                //shields last until hit so do nothing               
                break;
            default:
                //nothing
                break;

        }
        _powerupID = -1;
    }

    public void UpdateScore(int scoreToAdd)
    {
        _score = _score + scoreToAdd;
        _ui.UpdateScoreUI(_score);
    }

}
