using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    [SerializeField] private int _lives = 3;
    [SerializeField] private int _maxLives = 3;
    [SerializeField] private float _speed = 5.0f;
    private bool _laserCanFire = true;
    private int _powerupID = -1; // 0 = Triple Shot 1 = Speedboost 2 = shields
    private int _score = 0;
    private float _speedBoostMultiplier = 1f;
    private int _shieldStrength = 0;
    private int _ammoCount = 15;
    private int _maxAmmo = 15;
    private int _thrusterPower = 15;
    private float _thrusterBoost = 3.0f;
    [SerializeField] private GameObject _playerShieldObject;
    [SerializeField] private float _laserCooldownTimer = 0.5f;
    [SerializeField] private GameObject _laserPrefab;      
    [SerializeField] private GameObject _powerUp;
    [SerializeField] private GameObject _powerUpUltra;
    [SerializeField] private GameObject _explosion;
    [SerializeField] private GameObject _misslePrefab;
    [SerializeField] private UIManager _ui;
    [SerializeField] private AudioClip _laserSoundFX;
    [SerializeField] private AudioClip _powerUpSFX;
    [SerializeField] private AudioClip _powerUpUltraSFX;
    private AudioSource _audioFXSource;
    private Animator _animator;
    private SpawnManager _spawnManger;
    private float _lastThrusterUpdate = 0.25f;
    private float _thrusterUpdateDelay = 0.25f;
    private bool _thrusterCharging = false;
    private bool _canTakeDamage = true;
    private int _homingMissleCount = 0;
    private bool _missileCanFire = true;
    private static readonly int CanExplode = Animator.StringToHash("CanExplode");
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int XInput = Animator.StringToHash("xInput");

    private void Start()
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
            // Send default UI Elements
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
    private void Update()
    {                
        MovePlayer();

        if (_ammoCount <= 0) return;
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.F))
        {
            CheckFireLaser();
        }
    }


    private void MovePlayer()
    {           
        var horizontalInput = Input.GetAxis("Horizontal");        
        var verticalInput = Input.GetAxis("Vertical");        
        var direction = new Vector3(horizontalInput, verticalInput, 0).normalized;        

        if (horizontalInput == 0f)
        {
            _animator.SetBool(IsMoving,false);
            _animator.SetFloat(XInput, horizontalInput);      
        }
        else        
        {
            _animator.SetBool(IsMoving, true);
            _animator.SetFloat(XInput, horizontalInput);          
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

        transform.Translate(direction * ((_speed + _thrusterBoost) * _speedBoostMultiplier * Time.deltaTime));
        var yClamp = Mathf.Clamp(transform.position.y, -4f, 6f);
        transform.position = new Vector3(transform.position.x, yClamp, 0);
        if (transform.position.x >= 11.5 || (transform.position.x <= -11.5))
        {
            transform.position = new Vector3(transform.position.x * -1, transform.position.y, 0);            
        }     

    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void CheckFireLaser()
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
   
        if (!Input.GetKeyDown(KeyCode.F) || !_missileCanFire || _homingMissleCount <= 0) return;
        var enemyTagCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (enemyTagCount == 0) return;

        InstantiateWeapon(_misslePrefab);
        _homingMissleCount--;
        _ui.UpdateMissileUI(_homingMissleCount);
        if (_homingMissleCount == 0)
        {
            _powerupID = -1; 
        }
        _missileCanFire = false;
        StartCoroutine(MissileCooldownTimer());
    }

    private void InstantiateWeapon(GameObject weaponProjectile)
    {
        var offset = new Vector3(0.0f, 1.05f, 0.0f);
        Instantiate(weaponProjectile, (transform.position + offset), Quaternion.identity);
    }

    private IEnumerator LaserCooldownTimer()
    {
        yield return new WaitForSeconds(_laserCooldownTimer);
        _laserCanFire = true;
    }

    private IEnumerator MissileCooldownTimer()
    {
        yield return new WaitForSeconds(_laserCooldownTimer);
        _missileCanFire = true;
    }

    public void DamagePlayer(int damageAmount)
    {
        if (_lives <= 0) return;
        if (_playerShieldObject.activeInHierarchy)
        {
            _shieldStrength--;
            _ui.UpdateShield(_shieldStrength);
            if (_shieldStrength == 0)
            {
                _playerShieldObject.SetActive(false);
            }                
        }
        else if (_canTakeDamage)
        {
            CameraShake.CameraInstance.ShakeCamera();
            _lives = _lives - damageAmount;
            UpdateDamageFX(_lives);
        }
            
        _ui.UpdateLives(_lives);
        
        if (_lives > 0) return;
        _spawnManger.StopSpawning();
        UpdateDamageFX(_lives);
        gameObject.transform.Find("Thruster").transform.gameObject.SetActive(false);
        gameObject.transform.Find("PlayerShield").transform.gameObject.SetActive(false);
        _speed = 0;
        var exp = Instantiate(_explosion, transform.position, Quaternion.identity);
        exp.gameObject.GetComponent<Animator>().SetTrigger(CanExplode);
        Destroy(gameObject);
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
            case 6:
                _speed = 0;
                break;
            case 7:
                // TODO: Set Sound;
                _homingMissleCount = 2;
                _ui.UpdateMissileUI(_homingMissleCount);
                break;
        }
        StartCoroutine(PowerUpTimer());
    }

    private void UpdateShieldStatus(bool toggle, int strength)
    {
        if (_playerShieldObject == null) return;
        _playerShieldObject.SetActive(toggle);
        _shieldStrength = strength;
        _ui.UpdateShield(_shieldStrength);
    }

    private IEnumerator PowerUpTimer()
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
            case 6:
                _speed = 5.0f;
                break;
            case 7:
                _audioFXSource.clip = _laserSoundFX;
                break;
        }
        _powerupID = -1;
       
    }

    public void UpdateScore(int scoreToAdd)
    {
        _score = _score + scoreToAdd;
        _ui.UpdateScoreUI(_score);
    }

    private void UpdateDamageFX(int lives)
    {        
        switch (lives)
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

    private IEnumerator ChargeThruster()
    {
        while (_thrusterCharging)
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

}
