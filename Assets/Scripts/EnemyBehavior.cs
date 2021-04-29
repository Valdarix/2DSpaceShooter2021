using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private int _damageValue = 1;
    private Player _player;
    private Animator _animator;
    [SerializeField] private GameObject _explosion;
    [SerializeField] private int _enemyID;
    private float RotateSpeed = 10f;
    private float Radius = 1f;
    [SerializeField] private Transform _target;
    private float _angle;
    [SerializeField] private GameObject _laser;
    [SerializeField] private GameObject _laserRightSide;
    [SerializeField] private GameObject _laserLeftSide;
    [SerializeField] private AudioClip _laserFX;
    private AudioSource _audioFXSource;
    private bool _canFire = true;
    private bool _canFireLeft = true;
    private bool _canFireRight = true;
    private bool _canDodge = false;
    private int _randomLeftRight;
    private static readonly int CanExplode = Animator.StringToHash("CanExplode");


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
                Debug.Log(transform.parent.name);
            }

        }

        _audioFXSource = this.GetComponent<AudioSource>();
        if (_audioFXSource == null)
        {
            Debug.LogError("Player: Audio Source is NULL on EnemyID: " + _enemyID);
        }
        else
        {
            _audioFXSource.clip = _laserFX;
        }
        // Predetermine the dodge for dodge enemies
        _randomLeftRight = Random.Range(1, 100);

    }

    // Update is called once per frame
    private void Update()
    {
        MoveEnemy();
    }

    private void FixedUpdate()
    {
        var horizontalOffset = new Vector3(0, 0.8f, 0);
        const float rayLength = 10f;
        if (_canFire && (_enemyID == 2 || _enemyID == 3)) // front gun---should change to switch statement
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.down, rayLength);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    //Fire at player!
                    var offset = new Vector3(0, 0, 0);
                    Instantiate(_laser, (transform.position + offset), Quaternion.identity);
                    _canFire = false;
                    StartCoroutine(LaserCooldownTimer("MainLaser"));
                }
                if (hit.collider.CompareTag("Powerup"))
                {
                    //Fire at player!
                    var offset = new Vector3(0, 0, 0);
                    Instantiate(_laser, (transform.position + offset), Quaternion.identity);
                    _canFire = false;
                    StartCoroutine(LaserCooldownTimer("MainLaser"));
                }
            }
        }
        if (_canFireRight && _enemyID == 3)
        {
            var hitRight = Physics2D.Raycast((transform.position + horizontalOffset), Vector2.right, rayLength);
            if (hitRight.collider != null)
            {
                if (hitRight.collider.CompareTag("Player"))
                {
                    //Fire at player!
                    Vector3 offset = new Vector3(0.769999981f, 0.75999999f, 0);
                    Instantiate(_laserRightSide, (transform.position + offset), _laserRightSide.transform.rotation);
                    _canFireRight = false;
                    StartCoroutine(LaserCooldownTimer("RightCannon"));
                }
            }
        }
        if (_canFireLeft && _enemyID == 3)
        {
            var hitLeft = Physics2D.Raycast((transform.position + horizontalOffset), Vector2.left, rayLength);
            if (hitLeft.collider != null)
            {
                if (hitLeft.collider.CompareTag("Player"))
                {
                    //Fire at player!
                    Vector3 offset = new Vector3(-0.769999981f, 0.75999999f, 0);
                    Instantiate(_laserLeftSide, (transform.position + offset), _laserLeftSide.transform.rotation);
                    _canFireLeft = false;
                    StartCoroutine(LaserCooldownTimer("LeftCannon"));
                }
            }
        }

        if (_canFire != true || _enemyID != 4) return;
        {
            const float rayLengthforRam = 5f;
            var hit = Physics2D.Raycast(transform.position, Vector2.down, rayLengthforRam);
            if (hit.collider == null) return;
            if (!hit.collider.CompareTag("Player")) return;
            //Ram the player!
            _speed *= 3;
            _canFire = false;
            StartCoroutine(RamCoolDown());
        }
    }

    private IEnumerator RamCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        _speed = 3;
        _canFire = true;

    }

    private IEnumerator LaserCooldownTimer(string weaponToCooldown)
    {
        var fireRate = 0.5f;
        if (_enemyID == 3)
            fireRate = 0.7f;    
        yield return new WaitForSeconds(fireRate);

        switch (weaponToCooldown)
        {
            case "MainLaser":
                _canFire = true;
                break;
            case "RightCannon":
                _canFireRight = true;
                break;
            case "LeftCannon":
                _canFireLeft = true;
                break;
        }

    }

    private void MoveEnemy()
    {
        if (transform.position.y >= -5.5f)
        {

            if (_canDodge == true)
            {
               
                var step = _speed * 3 * Time.deltaTime;

                this.gameObject.transform.position = Vector3.MoveTowards(transform.position, _randomLeftRight % 2 == 0 ? new Vector3(transform.position.x + 2, transform.position.y, 0) : new Vector3(transform.position.x + -2, transform.position.y, 0), step);

                StartCoroutine(DisableDodge());
            }

            switch (_enemyID)
            {
                case 1:
                    _angle += RotateSpeed * Time.deltaTime;
                    var offset = new Vector3(Mathf.Sin(_angle), Mathf.Cos(_angle),0) * Radius; 
                    transform.position = _target.transform.position + offset;
                    transform.Rotate(Vector3.forward * (100 * Time.deltaTime));
                    break;
                case 5:
                    transform.Translate(Vector3.down * (_speed * Time.deltaTime));
                    if (gameObject.transform.position.y < _player.gameObject.transform.position.y - 2.25f  && _player != null)
                    {
                        if (_canFire)
                        {//Shot back at target
                            Instantiate(_laser, transform.position, _laser.transform.rotation);
                            _canFire = false;
                            StartCoroutine(LaserCooldownTimer("MainLaser"));
                        }

                    }
                    break;
                default:
                    transform.Translate(Vector3.down * (_speed * Time.deltaTime));
                    break;
            }            
        }
        else
        {
            transform.position = new Vector3(Random.Range(-10.0f, 10.0f), 7.5f, 0);
        }    
        
       
    }

    private IEnumerator DisableDodge()
    {
        yield return new WaitForSeconds(0.1f);
        _canDodge = false;
    }

    public void Dodge()
    {       
      _canDodge = true;           
    }

    private void OnTriggerEnter2D(Collider2D other)
    {    
        if (other.CompareTag("Player"))
        {            
            //Damage Player
            var player = other.transform.GetComponent<Player>();
            if (player == null) return;
            //Apply Damage to player
            player.DamagePlayer(_damageValue);
            var exp = Instantiate(_explosion, transform.position, Quaternion.identity);       
            exp.gameObject.GetComponent<Animator>().SetTrigger(CanExplode);
            _speed = 0;
            Destroy(this.gameObject);
        }
        else if (other.CompareTag("Laser"))
        {           
            var exp = Instantiate(_explosion, transform.position, Quaternion.identity);
            exp.gameObject.GetComponent<Animator>().SetTrigger(CanExplode);
            _speed = 0;
            Destroy(this.gameObject);
            _player.UpdateScore(10);
        }
    }

}
