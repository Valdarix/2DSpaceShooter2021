using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    //Private Seialized
    [SerializeField]
    private float _speed = 3.5f;
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

    void Start()
    {
        //take the current position and assign a start position of (0,0,0)
        transform.position = new Vector3(0,0,0);
        _spawnManger = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManger == null)
        {
            Debug.LogError("Spawn Manager is NULL.");
        }
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

        transform.Translate(direction * _speed * Time.deltaTime);

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
            if (isPowerUpEnabled == true)
            {
                Instantiate(_powerUp, (transform.position + offset), Quaternion.identity);
            } else 
            {
                Instantiate(_laserPrefab, (transform.position + offset), Quaternion.identity); 
            }
            
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
        if (_lives > 0)
        {
            _lives = _lives - DamageAmount; 
            if (_lives <= 0)
            {
                _spawnManger.StopSpawning();
                Destroy(gameObject);
                
            }
        }       
     }

    public void EnablePowerUp()
    {
        isPowerUpEnabled = true;
        StartCoroutine(PowerUpTimer());
    }

    IEnumerator PowerUpTimer()
    {
        yield return new WaitForSeconds(5.0f);
        isPowerUpEnabled = false;
    }

}
