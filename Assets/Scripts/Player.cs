using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _maxLives = 3;
    [SerializeField]
    private float _speed = 5.0f;
    private bool _laserCanFire = true;
    private int _powerupID = -1; // 0 = Triple Shot 1 = Speedboost 2 = shields
    private int _score = 0;
    private float _speedBoostMultiplier = 1f;
    private int _shieldStrength = 0;
    private int _ammoCount = 15;
    private int _maxAmmo = 15;
    private int _thrusterPower = 15;
    private float _thrusterBoost = 3.0f;
    [SerializeField]
    private GameObject _playerShieldObject;
    [SerializeField]
    private float _laserCooldownTimer = 0.5f;
    [SerializeField]
    private GameObject _laserPrefab;      
    [SerializeField]
    private GameObject _powerUp;
    [SerializeField]
    private GameObject _powerUpUltra;
    [SerializeField]
    private GameObject _explosion;
    [SerializeField]
    private UIManager _ui;
    [SerializeField]
    private AudioClip _laserSoundFX;
    [SerializeField]
    private AudioClip _powerUpSFX;
    [SerializeField]
    private AudioClip _powerUpUltraSFX;
    private AudioSource _audioFXSource;
    private Animator _animator;
    private SpawnManager _spawnManger;
    private float _lastThrusterUpdate = 0.25f;
    private float _thrusterUpdateDelay = 0.25f;
    private bool _thrusterCharging = false;
    private bool _canTakeDamage = true;

   

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
        else
        {
            // Semd default UI Elements
            _ui.UpdateAmmoCount(_ammoCount, _maxAmmo);
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
   
        if (_ammoCount > 0)
        {
            CheckFireLaser();
        }     
    }


    void MovePlayer()
    {           
        float horizontalInput = Input.GetAxis("Horizontal");        
        float verticalInput = Input.GetAxis("Vertical");        
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0).normalized;        

        if (horizontalInput == 0f)
        {
            _animator.SetBool("isMoving",false);
            _animator.SetFloat("xInput", horizontalInput);      
        }
        else        
        {
            _animator.SetBool("isMoving", true);
            _animator.SetFloat("xInput", horizontalInput);          
        }
        if (Input.GetKey(KeyCode.LeftShift) && _thrusterCharging == false)
        { 
            _thrusterBoost = 3.0f;         
            if (Time.time - _lastThrusterUpdate > _thrusterUpdateDelay)
            {               
                switch (_thrusterPower)
                {
                    case 0:
                        _thrusterCharging = true;
                        StartCoroutine(ChargeThruster());
                        break;
                    default:
                        _thrusterPower--;
                        _ui.UpdateThrusterUI(_thrusterPower);
                        break;
                }
              
                _lastThrusterUpdate = Time.time;
            }
        }
        else
        { 
            _thrusterBoost = 1.0f;
        }    

        transform.Translate(direction * ((_speed + _thrusterBoost) * _speedBoostMultiplier) * Time.deltaTime);

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
            switch (_powerupID)
            {
                case 0:
                    InstantiateWeapon(_powerUp);
                    _ammoCount--;
                    _ui.UpdateAmmoCount(_ammoCount,_maxAmmo);
                    break;
                case 5: // Shield Pulse doesnt take ammo
                    InstantiateWeapon(_powerUpUltra);
                    break;
                default:
                    InstantiateWeapon(_laserPrefab);
                    _ammoCount--;
                    _ui.UpdateAmmoCount(_ammoCount,_maxAmmo);
                    break;
            }           

            _audioFXSource.Play();
            _laserCanFire = false;
            StartCoroutine(LaserCooldownTimer());
        }
    }

    private void InstantiateWeapon(GameObject weaponProjectile)
    {
        Vector3 offset = new Vector3(0.0f, 1.05f, 0.0f);
        Instantiate(weaponProjectile, (transform.position + offset), Quaternion.identity);
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
                _shieldStrength--;
                _ui.UpdateShield(_shieldStrength);
                if (_shieldStrength == 0)
                {
                    _playerShieldObject.SetActive(false);
                }                
            }
            else if (_canTakeDamage == true)
            {
                CameraShake.cameraInstance.ShakeCamera();
                _lives = _lives - DamageAmount;
                UpdateDamageFX(_lives);
                // The code below is for a special shield type, work in progress.
               //_canTakeDamage = false;
               // StartCoroutine(EnableWeakShield());
            }
            
            _ui.UpdateLives(_lives);
            if (_lives <= 0)
            {
                _spawnManger.StopSpawning();
                UpdateDamageFX(_lives);
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
        _powerupID = powerupID;
        switch (_powerupID)
        {
            case 0:
                //Triple shot do nothing
                _audioFXSource.clip = _powerUpSFX;
                break;
            case 1:
                //speed boost will give an instant burst for 5 seconds and instantly recharge your thrusters
                _speedBoostMultiplier = 2.0f;
                _thrusterPower = 15;               
                break;
            case 2:
                //shields
                if (_playerShieldObject != null)
                {
                    UpdateShieldStatus(true, 3);
                }
                break;
            case 3:
                _ammoCount = 15;
                _ui.UpdateAmmoCount(_ammoCount,_maxAmmo);
                break;
            case 4:
                if (_lives < _maxLives)
                { 
                    _lives++;
                    _ui.UpdateLives(_lives);
                    UpdateDamageFX(_lives);
                }                
                break;
            case 5:
                _audioFXSource.clip = _powerUpUltraSFX;
                break;
            default:
                //nothing
                break;                
        }
        StartCoroutine(PowerUpTimer());
    }

    private void UpdateShieldStatus(bool Toggle, int Strength)
    {
        if (_playerShieldObject != null)
        {
            _playerShieldObject.SetActive(Toggle);
            _shieldStrength = Strength;
            _ui.UpdateShield(_shieldStrength);
        }
    }

    IEnumerator PowerUpTimer()
    {
        yield return new WaitForSeconds(5.0f);       
        switch (_powerupID)
        {
            case 0:
                _audioFXSource.clip = _laserSoundFX;
                //Triple shot do nothing
                break;
            case 1:
                //speed boost 
                _speedBoostMultiplier = 1.0f;
                break;
            case 2:
                //shields last until hit so do nothing                           
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                _audioFXSource.clip = _laserSoundFX;
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

    private void UpdateDamageFX(int _lives)
    {        
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
                gameObject.transform.Find("LeftWingDamage").transform.gameObject.SetActive(false);
                break;
            case 3:
                // do nothing full health. Later we can toggle these off
                gameObject.transform.Find("LeftWingDamage").transform.gameObject.SetActive(false);
                gameObject.transform.Find("RightWingDamage").transform.gameObject.SetActive(false);
               break;
        }
    }

    IEnumerator ChargeThruster()
    {
        while (_thrusterCharging == true)
        {
            yield return new WaitForSeconds(0.5f);
            _thrusterPower++;            
            if (_thrusterPower == 15)
            {
                _thrusterCharging = false;
            }
            _ui.UpdateThrusterUI(_thrusterPower);
        }       
    }

    IEnumerator EnableWeakShield()
    { // Using this to provide cool weak shield effect when a player takes damage.
        UpdateShieldStatus(true, 3);
        yield return new WaitForSeconds(0.5f);
        _canTakeDamage = true;
        UpdateShieldStatus(false, 0);
    }    
}
