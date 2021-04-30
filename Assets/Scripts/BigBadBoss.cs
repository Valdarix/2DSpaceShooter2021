using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private GameObject bossShield;
    [SerializeField] private LayerMask ignoreHitDetection;
    private int _shieldPower = 3;
    private bool _isShielded = true;
    private Transform _leftTurret;
    private Transform _leftTurretBase;
    private bool _leftTurretDestroyed = false;
    private Transform _rightTurret;
    private Transform _rightTurretBase;
    private bool _rightTurretDestroyed = false;
    private Transform _bossTransform;
    private float _angle;
    private const float RotateSpeed = 0.5f;
    private const float Radius = 0f;
    private bool _canFireLeftTurret = true;
    private bool _canFireRightTurret = true;
    private Turret _leftTurretGun;
    private Turret _rightTurretGun;
    private SpriteRenderer _bossShieldSprite;
    

    // Start is called before the first frame update
    private void Start()
    {
        _leftTurret = bossLeftGun.GetComponent<Transform>();
        _rightTurret = bossRightGun.GetComponent<Transform>();
        _leftTurretBase = bossLeftGunBase.GetComponent<Transform>();
        _rightTurretBase = bossRightGunBase.GetComponent<Transform>();
        _leftTurretGun = _leftTurret.GetComponent<Turret>();
        _rightTurretGun = _rightTurret.GetComponent<Turret>();
        _bossTransform = GetComponent<Transform>();
        _bossShieldSprite = bossShield.GetComponent<SpriteRenderer>(); 

        StartCoroutine(RotateLeftGun());
        StartCoroutine(RotateRightGun());
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private IEnumerator RotateLeftGun()
    {
        var dirToRotate = new Vector3(0,0,1);
        
        while (true)
        {
            
            if (_leftTurretDestroyed)
            {
                yield break;
            }
            dirToRotate = _rightTurret.rotation.z switch
            {
                < 88.69f => dirToRotate,
                > 191.32f => -dirToRotate,
                _ => dirToRotate
            };
            _angle += RotateSpeed * Time.deltaTime;
            var offset = new Vector3(Mathf.Sin(_angle), Mathf.Cos(_angle),0) * Radius; 
            _leftTurret.position = _leftTurretBase.position + offset;
            _leftTurret.Rotate(dirToRotate * (100 * Time.deltaTime));
            
            var horizontalOffset = new Vector3(0, 0.8f, 0);
            const float rayLength = 400f;
            var hitLeft = Physics2D.Raycast((_leftTurret.position), _leftTurret.up , rayLength,~ignoreHitDetection);
            
            Debug.DrawRay(_leftTurret.position, _leftTurret.up * rayLength, new Color(0f, 1f, 0.04f));
            if (_canFireLeftTurret && hitLeft.collider != null && hitLeft.collider.CompareTag("Player"))
            {
                //Fire at player!
                _leftTurretGun.FireLaser();
                _canFireLeftTurret = false;
                StartCoroutine(TurretCooldown("LeftTurret"));
            }

            yield return null;
        }
    }
    
    private IEnumerator RotateRightGun()
    {
        var dirToRotate = Vector2.right;
        while (true)
        {
            if (_rightTurretDestroyed)
            {
                yield break;
            }
            _angle += RotateSpeed * Time.deltaTime;
            var offset = new Vector3(Mathf.Sin(_angle), Mathf.Cos(_angle),0) * Radius; 
            _rightTurret.position = _rightTurretBase.position + offset;
            _rightTurret.Rotate(-Vector3.forward * (100 * Time.deltaTime));
            
            var horizontalOffset = new Vector3(0, 0.8f, 0);
            const float rayLength = 400f;
            dirToRotate = _rightTurret.rotation.z switch
            {
                < 14.44f => Vector2.right,
                > 81.663f => Vector2.left,
                _ => dirToRotate
            };

            var hitRight = Physics2D.Raycast(_rightTurret.position, _rightTurret.up , rayLength,~ignoreHitDetection);
            
            Debug.DrawRay(_rightTurret.position, _rightTurret.up * rayLength, Color.red);
            if (_canFireRightTurret && hitRight.collider != null && hitRight.collider.CompareTag("Player"))
            {
                //Fire at player!
                _canFireRightTurret = false;
                _rightTurretGun.FireLaser();
                StartCoroutine(TurretCooldown("RightTurret"));
            }
            yield return null;
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
