using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BigBadBoss : MonoBehaviour
{
    [SerializeField] private float speed = 0.00025f;
    [SerializeField] private GameObject bossLeftGun;
    [SerializeField] private GameObject bossRightGun;
    [SerializeField] private GameObject mainGunProjectile;
    [SerializeField] private GameObject turretProjectile;
    [SerializeField] private GameObject bossShield;
    [SerializeField] private LayerMask ignoreHitDetection;
    [SerializeField] private GameObject frontGun;
    [SerializeField] private GameObject leftCannon;
    [SerializeField] private GameObject rightCannon;
    [SerializeField] private int bossHealth = 10;
    private int _shieldPower = 3;
    private Transform _leftTurret;
    private bool _leftTurretDestroyed = false;
    private Transform _rightTurret;
    private bool _rightTurretDestroyed = false;
    private bool _canFireLeftTurret = true;
    private bool _canFireRightTurret = true;
    private SpriteRenderer _bossShieldSprite;
    private bool _detectedLeft = false;
    private bool _detectedRight = false;
    private Transform _playerTarget;
    private float _shieldAlphaValue = 1f;
    private bool _isShielded = true;
    private bool _moveLeft = true;
    private bool _moveRight = false;
    [SerializeField] private GameManager gameManager;
    
    
    
    // Start is called before the first frame update
    private void Start()
    {
        _leftTurret = bossLeftGun.GetComponent<Transform>();
        _rightTurret = bossRightGun.GetComponent<Transform>();
        _bossShieldSprite = bossShield.GetComponent<SpriteRenderer>();
        
        _playerTarget = GameObject.Find("Player").GetComponent<Transform>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (transform.position.y > 0)
        {
            transform.Translate(-transform.up * (speed * Time.deltaTime));
        }
        else
        {
            if (_moveLeft)
                transform.Translate(-transform.right * (speed * Time.deltaTime));
            else if (_moveRight)
                transform.Translate(transform.right * (speed * Time.deltaTime));
        }
    }


    private void FixedUpdate()
    {
        if (_playerTarget == null) return;
        RotateLeftGun();
        RotateRightGun();
    }

    private void RotateLeftGun()
    {
        if (_leftTurretDestroyed) return;
        var targetPos = _playerTarget.position;
        var position = _leftTurret.position;
        var dir = CalculateDirection(targetPos, (Vector2) position);
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var rayInfo = Physics2D.Raycast(position, dir, 50, ~ignoreHitDetection);
        Debug.DrawRay(position, dir, Color.red);
        _leftTurret.transform.up = dir;
        if (rayInfo.collider == null || !rayInfo.collider.CompareTag("Player")) return;
        _detectedLeft = _detectedLeft switch
        {
            //Fire at player!
            false => true,
            true => false
        };
        if (!_detectedLeft || !_canFireLeftTurret) return;
        var force = 7f;
        var bossLaser = Instantiate(turretProjectile, _leftTurret.position, _leftTurret.rotation);
        bossLaser.GetComponent<Rigidbody2D>().AddForce(dir * force);
        _canFireLeftTurret = false;
        StartCoroutine(TurretCooldown("LeftTurret"));
    }
    private static Vector2 CalculateDirection(Vector2 targetPos, Vector2 turretPos)
    {
        return targetPos - turretPos;
    }

    private void RotateRightGun()
    {
        if (_rightTurretDestroyed) return;
        var targetPos = _playerTarget.position;
        var position = _rightTurret.position;
        var dir = CalculateDirection(targetPos, (Vector2) position);
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var rayInfo = Physics2D.Raycast(position, dir, 50, ~ignoreHitDetection);
        Debug.DrawRay(position, dir, Color.red);
        _rightTurret.transform.up = dir;
        if (rayInfo.collider == null || !rayInfo.collider.CompareTag("Player")) return;
        _detectedRight = _detectedRight switch
        {
            //Fire at player!
            false => true,
            true => false
        };
        if (!_detectedRight || !_canFireRightTurret) return;
        var force = 7f;
        var bossLaser = Instantiate(turretProjectile, _rightTurret.position, _rightTurret.rotation);
        bossLaser.GetComponent<Rigidbody2D>().AddForce(dir * force);
        _canFireRightTurret = false;
        StartCoroutine(TurretCooldown("RightTurret"));
    }

    private IEnumerator TurretCooldown(string turret)
    {
        yield return new WaitForSeconds(3f);
        switch (turret)
        {
            case "LeftTurret":
                _canFireLeftTurret = true;
                break;
            case "RightTurret":
                _canFireRightTurret = true;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("LeftTag"))
        {
            _moveLeft = false;
            _moveRight = true;
        } else if (other.CompareTag("RightTag"))
        {
            _moveLeft = true;
            _moveRight = false;
        }
        
        if (!other.CompareTag("Laser") && !other.CompareTag(("Player"))) return;
        if (_shieldPower > 0 && (other.CompareTag("Laser") || other.CompareTag("Player")))
        {
            _shieldPower--;
            StartCoroutine(Damage(_bossShieldSprite));
            if (other.CompareTag("Player"))
            {
                _playerTarget.GetComponent<Player>().DamagePlayer(1);
            }
        }
        else if (!_isShielded)
        {
            if (other.CompareTag("Laser"))
            {
                StartCoroutine(Damage(transform.GetComponent<SpriteRenderer>()));
                StartCoroutine(Damage(_leftTurret.GetComponent<SpriteRenderer>()));
                StartCoroutine(Damage(_rightTurret.GetComponent<SpriteRenderer>()));
               bossHealth--;
                if (bossHealth == 0)
                {
                    Destroy(gameObject);
                    // GAME OVER !
                    gameManager.GameOver("YOU WIN!"); 
                }
            }
            if (CompareTag("Player"))
            {
                // Damage the player
                
                _playerTarget.GetComponent<Player>().DamagePlayer(1);
            }
        }
    }

    private IEnumerator Damage(SpriteRenderer sprite)
    {
        var color = Color.red;
        if (_isShielded)
        {
            _shieldAlphaValue -= 0.25f;
            sprite.color = color;
            yield return new WaitForSeconds(0.250f);
            color = Color.white;
            color.a = _shieldAlphaValue - 0.23f;
            sprite.color = color;
            yield return new WaitForSeconds(0.1f);
            if (_shieldPower != 0) yield break;
            _isShielded = false;        
        }
        else
        {
            sprite.color = color;
            yield return new WaitForSeconds(0.250f);
            color = Color.white;
            sprite.color = color;
            yield return new WaitForSeconds(0.1f);
        }
    }
    
}