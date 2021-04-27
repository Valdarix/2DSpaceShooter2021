using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehavior : MonoBehaviour
{
    [SerializeField]
    private float _speed = 0.0f;
    [SerializeField]
    private float _rotateSpeed = 0.0f;
    [SerializeField]
    private bool _destroyOnTrigger = true;  


    private void Start()
    {
        // if this is false we don't want to destroy on trigger so we start the cooldown for destruction on spawn
        if (_destroyOnTrigger == false)
        { 
            StartCoroutine(Pulse());
        }    
    
    }

    // Update is called once per frame
    void Update()
    {

        if (_speed > 0f)
        {
            HandleMovement();
        }

        if (_rotateSpeed > 0f)
        {
            HandleRotate();
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {   
        if (this.CompareTag("Laser"))
        {
            if (other.CompareTag("Enemy"))
            {
                if (_destroyOnTrigger == true)
                {
                    Destroy(this.gameObject);
                }
            }
        }

        if (this.CompareTag("EnemyLaser"))
        {
            if (other.CompareTag("Player"))
            {
                if (_destroyOnTrigger == true)
                {
                    Player player = other.transform.GetComponent<Player>();
                    if (player != null)
                    {
                        //Apply Damange to player
                        player.DamagePlayer(1);
                        Destroy(this.gameObject);
                    }
                }
            }
        }

    }

    private void HandleMovement()
    {
        
        if (this.CompareTag("Laser"))
        { 
            transform.Translate(Vector3.up * _speed * Time.deltaTime);

            if (transform.position.y > 8.0f)
            {
                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }

        if (this.CompareTag("EnemyLaser"))
        {                      
          
            transform.Translate(Vector3.down * _speed * Time.deltaTime);        
            
            
            if (transform.position.y < -8.0f)
            {
                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    private void HandleRotate()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }

    IEnumerator Pulse()
    {
        yield return new WaitForSeconds(1.2f);
        Destroy(gameObject);
    }    
}
