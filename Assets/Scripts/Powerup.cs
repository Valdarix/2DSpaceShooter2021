using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{   
    [SerializeField]
    private float _speed = 3.0f; 
    [SerializeField]
    private int _powerUpID;
    [SerializeField]
    private AudioClip _audioClip;
    private AudioSource _as;

    private void Start()
    {
        _as = gameObject.GetComponent<AudioSource>();
        if (_as == null)
        {
            Debug.LogError("Power: Audio Source is NULL");
        }
        _as.clip = _audioClip;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -8.0f)
        {          
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.EnablePowerUp(_powerUpID);
                _as.Play();
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }

            Destroy(gameObject,1f);
        }
    }
    public void DestroyPowerup()
    {
        Destroy(gameObject);
    }
}

   
