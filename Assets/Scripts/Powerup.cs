using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{   

    [SerializeField] private float _speed = 3.0f; 
    [SerializeField] private int _powerUpID;
    [SerializeField] private AudioClip _audioClip;
    private AudioSource _as;
    private Player _player;
    private bool _isMovingToPlayer = false;

    private void Start()
    {
        _as = gameObject.GetComponent<AudioSource>();
        if (_as == null)
        {
            Debug.LogError("Power: Audio Source is NULL");
        }
        _as.clip = _audioClip;

        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }
    }

    // Update is called once per frame
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.C))
        {           
            _isMovingToPlayer = true;
        }
        
        switch (_isMovingToPlayer)
        {
            case false:
                transform.Translate(Vector3.down * (_speed * Time.deltaTime));
                break;
            case true:
            {
                var step = _speed * Time.deltaTime;
                this.gameObject.transform.position = Vector3.MoveTowards(transform.position, _player.gameObject.transform.position, step);
                break;
            }
        }   
       
        if (transform.position.y < -8.0f)
        {          
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var player = other.transform.GetComponent<Player>();

        if (player != null)
        {
            player.EnablePowerUp(_powerUpID);
            _as.Play();
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }

        Destroy(gameObject,1f);
    }
    public void DestroyPowerup()
    {
        Destroy(gameObject);
    }
}

   
