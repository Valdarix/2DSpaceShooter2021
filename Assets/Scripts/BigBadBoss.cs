using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BigBadBoss : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private GameObject bossMainBody;
    [SerializeField] private GameObject bossLeftGun;
    [SerializeField] private GameObject bossRightGun;
    [SerializeField] private GameObject bossLeftGunBase;
    [SerializeField] private GameObject bossRightGunBase;
    [SerializeField] private GameObject mainGunProjectile;
    [SerializeField] private GameObject turretProjectile;
    [SerializeField] private GameObject bossShield;
    [SerializeField] private LayerMask ignoreHitDetection;
    private int _shieldPower = 3;
    private bool _isShielded = true;
    private Transform _leftTurretBase;
    private Transform _leftTurret;
    private bool _leftTurretDestroyed = false;
    private Transform _rightTurret;
    private Transform _rightTurretBase;
    private bool _rightTurretDestroyed = false;
    private Transform _bossTransform;
    private bool _canFireLeftTurret = true;
    private bool _canFireRightTurret = true;
    private SpriteRenderer _bossShieldSprite;
    private bool _detectedLeft = false;
    private bool _detectedRight = false;
    private Transform _playerTarget;

    // Start is called before the first frame update
    private void Start()
    {
        _leftTurret = bossLeftGun.GetComponent<Transform>();
        _rightTurret = bossRightGun.GetComponent<Transform>();
        _leftTurretBase = bossLeftGunBase.GetComponent<Transform>();
        _rightTurretBase = bossRightGunBase.GetComponent<Transform>();
        _bossTransform = GetComponent<Transform>();
        _bossShieldSprite = bossShield.GetComponent<SpriteRenderer>();

        _playerTarget = GameObject.Find("Player").GetComponent<Transform>();

     
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void FixedUpdate()
    {
        RotateLeftGun();
        RotateRightGun();
    }

    private void RotateLeftGun()
    {
        if (_leftTurretDestroyed)
        {
            return;
        }
        var targetPos = _playerTarget.position;
        var dir = CalculateDirection(targetPos, (Vector2) _leftTurret.position);
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var rayInfo = Physics2D.Raycast(_leftTurret.position, dir, 50);
        Debug.DrawRay(_leftTurret.position,dir,Color.red);
        _leftTurret.transform.up = dir;
        if (rayInfo.collider != null && rayInfo.collider.CompareTag("Player"))
        {
            _detectedLeft = _detectedLeft switch
            {
                //Fire at player!
                false => true,
                true => false
            };
            if (_detectedLeft && _canFireLeftTurret)
            {
                var force = 7f;
                var bossLaser = Instantiate(turretProjectile, _leftTurret.position,_leftTurret.rotation);
                bossLaser.GetComponent<Rigidbody2D>().AddForce(dir * force);
                _canFireLeftTurret = false;
                StartCoroutine(TurretCooldown("LeftTurret"));
            }
        }
    }
    private static Vector2 CalculateDirection(Vector2 targetPos, Vector2 turretPos)
    {
        return targetPos - turretPos;
    }

    private void RotateRightGun()
    {
        if (_rightTurretDestroyed)
        {
            return;
        }

        var targetPos = _playerTarget.position;
        var dir = CalculateDirection(targetPos, (Vector2) _rightTurret.position);
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var rayInfo = Physics2D.Raycast(_rightTurret.position, dir, 50);
        Debug.DrawRay(_rightTurret.position,dir,Color.red);
        _rightTurret.transform.up = dir;
        if (rayInfo.collider != null && rayInfo.collider.CompareTag("Player"))
        {
            _detectedRight = _detectedRight switch
            {
                //Fire at player!
                false => true,
                true => false
            };
            if (_detectedRight && _canFireRightTurret)
            {
                var force = 7f;
                var bossLaser = Instantiate(turretProjectile, _rightTurret.position,_rightTurret.rotation);
                bossLaser.GetComponent<Rigidbody2D>().AddForce(dir * force);
                _canFireRightTurret = false;
                StartCoroutine(TurretCooldown("RightTurret"));
            }
        }
    }

    private IEnumerator TurretCooldown(string turret)
    {
        yield return new WaitForSeconds(1f);
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
        if (!other.CompareTag("Laser") && !other.CompareTag(("Player"))) return;
        if (_shieldPower > 0)
        {
            _shieldPower--;
            if (_shieldPower == 0)
            {
                _bossShieldSprite.enabled = false;
            }
        }
        else
        {
            if (other.CompareTag("Laser"))
            {
                
            }
            else if (CompareTag("Player"))
            {
                // Damage the player
            }
        }
    }
}