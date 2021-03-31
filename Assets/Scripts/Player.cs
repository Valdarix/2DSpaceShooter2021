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
  

    //Privates
    private bool _laserCanFire = true;

    void Start()
    {
        //take the current position and assign a start position of (0,0,0)
        transform.position = new Vector3(0,0,0);
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

        // use 11.5, anything less seems to cause a bug if the player tries to shift back in the other
        // direction too quickly. Multiple both by *1 to handle both in one line of code. 
        if (transform.position.x >= 11.5 || (transform.position.x <= -11.5))
        {           
            transform.position = new Vector3(transform.position.x * -1, transform.position.y,0);
            
        }     

    }
    void CheckFireLaser()
    {
   
        if (Input.GetKeyDown(KeyCode.Space) && _laserCanFire)
        {            
            Vector3 offset = new Vector3(0.0f,0.8f,0.0f);
            Instantiate(_laserPrefab, (transform.position + offset), Quaternion.identity);
            _laserCanFire = false;
            StartCoroutine(LaserCooldownTimer());
        }

    }

    IEnumerator LaserCooldownTimer()
    {
        yield return new WaitForSeconds(_laserCooldownTimer);
        _laserCanFire = true;
    }

    

   


}
