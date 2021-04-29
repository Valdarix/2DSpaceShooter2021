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
    [SerializeField] private GameObject turretProjectile1;
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
    private const float RotateSpeed = 1f;
    private const float Radius = 0f;
    

    // Start is called before the first frame update
    private void Start()
    {
        _leftTurret = bossLeftGun.GetComponent<Transform>();
        _rightTurret = bossRightGun.GetComponent<Transform>();
        _leftTurretBase = bossLeftGunBase.GetComponent<Transform>();
        _rightTurretBase = bossRightGunBase.GetComponent<Transform>();
        _bossTransform = this.GetComponent<Transform>();

        StartCoroutine(RotateLeftGun());
        StartCoroutine(RotateRightGun());
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private IEnumerator RotateLeftGun()
    {
        var dirToRotate = Vector2.left;
        while (true)
        {
            
            if (_leftTurretDestroyed)
            {
                yield break;
            }
            dirToRotate = _rightTurret.rotation.z switch
            {
                < 88.69f => Vector2.right,
                > 191.32f => Vector2.left,
                _ => dirToRotate
            };
            _angle += RotateSpeed * Time.deltaTime;
            var offset = new Vector3(Mathf.Sin(_angle), Mathf.Cos(_angle),0) * Radius; 
            _leftTurret.position = _leftTurretBase.position + offset;
            _leftTurret.Rotate(Vector3.forward * (100 * Time.deltaTime));
            
            var horizontalOffset = new Vector3(0, 0.8f, 0);
            const float rayLength = 400f;
            var hitLeft = Physics2D.Raycast((_leftTurret.position + horizontalOffset), dirToRotate, rayLength,~ignoreHitDetection);
            
            Debug.DrawRay(_leftTurret.position, -_leftTurret.up * -rayLength, Color.red);
            if (hitLeft.collider != null)
            {
                Debug.Log(hitLeft.collider.name + " was hit bit Left");
                if (hitLeft.collider.CompareTag("Player"))
                {
                    Debug.Log(hitLeft.collider.name + " was hit bit Left");
                    //Fire at player!
                    var offsetLaser = new Vector3(-0,0, 0);
                    // Instantiate(turretProjectile1, (_leftTurret.position + offsetLaser), _leftTurret.rotation);
                    //_canFireLeft = false;
                    //StartCoroutine(LaserCooldownTimer("LeftCannon"));
                }
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

            var hitRight = Physics2D.Raycast((_rightTurret.position + horizontalOffset), dirToRotate, rayLength,~ignoreHitDetection);
            
            Debug.DrawRay(_rightTurret.position, -_rightTurret.up * -rayLength, Color.red);
            if (hitRight.collider != null)
            {
                if (hitRight.collider.CompareTag("Player"))
                {
                    Debug.Log(hitRight.collider.name + " was hit bit Right");
                    //Fire at player!
                    var offsetLaser = new Vector3(-0, 0, 0);
                    // Instantiate(turretProjectile1, (_leftTurret.position + offsetLaser), _leftTurret.rotation);
                    //_canFireLeft = false;
                    //StartCoroutine(LaserCooldownTimer("LeftCannon"));
                }
            }
            
            yield return null;
        }
    }
}
