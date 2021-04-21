using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpClass : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField]
    private int _powerUpID;
    [SerializeField]
    private Sprite _spriteIcon;
    [SerializeField]
    private Animator _animator;


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
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.EnablePowerUp(_powerUpID);
            }

            Destroy(gameObject);
        }
    }
}
