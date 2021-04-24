using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 5.0f;
    [SerializeField]
    private int _health = 5;   
    private SpawnManager _sm;
    private Animator _animator;
    [SerializeField]
    private GameObject _explosion;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 randomLocation = new Vector3(Random.Range(-8f, 8f), Random.Range(4f, 6f), 0);
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

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            _health--;
         

            if (_health == 0)
            {
                GameObject exp = Instantiate(_explosion, transform.position, Quaternion.identity);             
                exp.gameObject.GetComponent<Animator>().SetTrigger("CanExplode");
                Destroy(this.gameObject);
                _sm.StartSpawning();
            }
        }
    }
}
