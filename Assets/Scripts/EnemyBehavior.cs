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
    private Player _player;
    private Animator _animator;
    [SerializeField]
    private GameObject _explosion;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }
        _animator = gameObject.GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator is NULL");
        }
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
            
            //Damage Player
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                //Apply Damange to player
                player.DamagePlayer(_damageValue);
                GameObject exp = Instantiate(_explosion, transform.position, Quaternion.identity);       
                exp.gameObject.GetComponent<Animator>().SetTrigger("CanExplode");
                _speed = 0;
                Destroy(this.gameObject);

            }
        }
        else if (other.CompareTag("Laser"))
        {
            GameObject exp = Instantiate(_explosion, transform.position, Quaternion.identity);           
            exp.gameObject.GetComponent<Animator>().SetTrigger("CanExplode");
            _speed = 0;
            Destroy(this.gameObject);
            _player.UpdateScore(10);           
           
        }

    }


}
