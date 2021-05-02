using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidBehavior : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 5.0f;
    [SerializeField] private int _health = 5;  
    private SpawnManager _sm;
    private Animator _animator; 
    private float speed;
    private float scale;
    [SerializeField] private GameObject _explosion;
    private bool _setOutside = false;

    private static readonly int CanExplode = Animator.StringToHash("CanExplode");

    // Start is called before the first frame update
    private void Start()
    {
        var randomLocation = new Vector3(Random.Range(-8f, 8f), Random.Range(4f, 6f), 0);
        gameObject.transform.position = randomLocation;

        _animator = gameObject.GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Asteroid: Animator is NULL");
        }

        _sm = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_sm == null)
        {
            Debug.LogError("Asteroid: Spawn Manager is NULL");
        }
        scale = Random.Range(0.5f, 1.2f);
        if (!_setOutside)
            speed = Random.Range(1f, 4.5f);
        transform.position.Scale(new Vector3(scale,scale,0));
    }

    public void UpdateSpeed(int newSpeed)
    {
        speed = newSpeed;
        _setOutside = true;
    }

    // Update is called once per frame
    private void Update()
    {   
        transform.Rotate(Vector3.forward * (_rotateSpeed * Time.deltaTime));
        if (speed > 0 )
            transform.Translate(Vector3.down * (speed * Time.deltaTime));
       
        if (transform.position.y <= -5.5f)
            transform.position = new Vector3(Random.Range(-10.0f, 10.0f), 7.5f, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Laser")) return;
        _health--;
        
        if (_health != 0) return;
        var exp = Instantiate(_explosion, transform.position, Quaternion.identity);             
        exp.gameObject.GetComponent<Animator>().SetTrigger(CanExplode);
        Destroy(this.gameObject);
        _sm.StartSpawning();
    }
}
