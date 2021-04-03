using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    private int _damageValue = 1;
    

    private void Start()
    {   
      
       
    }
    // Update is called once per frame
    void Update()
    {
        MoveEnemy();          
    }

    void MoveEnemy()
    {
        if (transform.position.y >= -5.5f)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(Random.Range(-10.0f, 10.0f), 7.5f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            //Damage Player
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                //Apply Damange to player
                player.DamagePlayer(_damageValue);
                           
            }
        }
        else if (other.CompareTag("Laser"))
        {
            Destroy(gameObject);

           
        }
    }

}
