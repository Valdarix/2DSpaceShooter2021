using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyOject;
    [SerializeField]
    private GameObject _asteroid;
    [SerializeField]
    private GameObject[] _powerUpObject = new GameObject[5];
    private bool _canSpawn = false;   
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject _powerUpContainer;
    [SerializeField]
    private float _spawnTimer = 5.0f;
    [SerializeField]
    private WaitForSeconds _spawnEnemyWaitForSeconds;
    // Start is called before the first frame update
    void Start()
    {
        _spawnEnemyWaitForSeconds = new WaitForSeconds(_spawnTimer);
        GameObject newAsteroid = Instantiate(_asteroid);
        newAsteroid.transform.parent = _enemyContainer.transform;
       
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(3.0f);
        while (_canSpawn)
        {
            Vector3 randomLocation = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyOject, randomLocation, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return _spawnEnemyWaitForSeconds;
        }
    }

    IEnumerator SpawnPowerup()
    {
        yield return new WaitForSeconds(3.0f);
        while (_canSpawn)
        {
            yield return new WaitForSeconds(Random.Range(3.0f, 8.0f));
            int _powerupID = Random.Range(0, _powerUpObject.Length);
            Vector3 randomLocation = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject newPowerup = Instantiate(_powerUpObject[_powerupID], randomLocation, Quaternion.identity);
            newPowerup.transform.parent = _powerUpContainer.transform;
        }
    }

    public void StartSpawning()
    {
        _canSpawn = true;
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnPowerup());
    }

    public void StopSpawning()
    {
        _canSpawn = false;          
    }
}
