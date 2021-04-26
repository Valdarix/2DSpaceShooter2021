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
    [SerializeField]
    private int _enemyID;
    private float RotateSpeed = 7f;
    private float Radius = 1.0f;
    [SerializeField]
    private Transform _target;
    private float _angle;
    [SerializeField]
    private GameObject _laser;
    [SerializeField]
    private AudioClip _laserFX;
    private AudioSource _audioFXSource;
    private bool _canFire = true;

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

        if (_enemyID == 1)
        {
            if (_target == null)
            {
                Debug.LogError("Target is NULL");
            }           
        }
        _audioFXSource = this.GetComponent<AudioSource>();
        if (_audioFXSource == null)
        {
            Debug.LogError("Player: Audio Source is NULL on EnenmyID: " + _enemyID);
             
        }
        else
        {
            _audioFXSource.clip = _laserFX;
        }
    }
     
    // Update is called once per frame
    void Update()
    {
        MoveEnemy();          
    }

    private void FixedUpdate()
    {
        if (_canFire == true && _enemyID == 2 )
        {
            float rayLength = 50f;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayLength);
            if (hit.collider != null)
            {
              
                if (hit.collider.CompareTag("Player"))
                {
                    //Fire at player!

                    Vector3 offset = new Vector3(0, 0, 0);
                    Instantiate(_laser, (transform.position + offset), Quaternion.identity);                   
                    _canFire = false;
                    StartCoroutine(LaserCooldownTimer());

                }
            }           
        }
       
    }

    IEnumerator LaserCooldownTimer()
    {
        var fireRate = 0.5f;
        if (_enemyID == 3)
            fireRate = 1f;    
        yield return new WaitForSeconds(fireRate);
        
        _canFire = true;
    }

    void MoveEnemy()
    {
        if (transform.position.y >= -5.5f)
        {
            switch (_enemyID)
            {
                case 1:
                    _angle += RotateSpeed * Time.deltaTime;

                    var offset = new Vector3(Mathf.Sin(_angle), Mathf.Cos(_angle),0) * Radius; 
                    transform.position = _target.transform.position + offset;
                    break;
                default:
                    transform.Translate(Vector3.down * _speed * Time.deltaTime);
                    break;
            }            
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
