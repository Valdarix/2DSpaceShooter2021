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
    private GameObject[] _powerUpObject = new GameObject[6];
    [SerializeField]
    private GameObject[] _enemyList;
    private bool _canSpawn = false;   
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject _powerUpContainer;
    [SerializeField]
    private float _spawnTimer = 5.0f;
    [SerializeField]
    private WaitForSeconds _spawnEnemyWaitForSeconds;
    private float _lastUltraSpawn = 0f;
    private float _timebetweenUltraSpawns = 5f;
    private int _maxEnemiesToSpawn = 10;
    private int _baseEnemiesPerLevel = 10;
    [SerializeField]
    private GameManager _gameManager;
    private int _currentWave = 1;

    // Start is called before the first frame update
    void Start()
    {
        _spawnEnemyWaitForSeconds = new WaitForSeconds(_spawnTimer);
        GameObject newAsteroid = Instantiate(_asteroid);
        newAsteroid.transform.parent = _enemyContainer.transform;
        _gameManager.NewWave();
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
            GameObject newEnemy = Instantiate(_enemyList[Random.Range(0,_enemyList.Length)], randomLocation, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            _maxEnemiesToSpawn--;            
            if (_maxEnemiesToSpawn == 0) //handle spawning new wave
            {               
                _currentWave++;
                _gameManager.NewWave();
                _spawnTimer -= 1f;
                _spawnEnemyWaitForSeconds = new WaitForSeconds(_spawnTimer);
                Debug.Log("Wave: " + _currentWave + " Spawn Timer: " + _spawnTimer + " Wait for Seconds " + _spawnEnemyWaitForSeconds);
                switch (_currentWave)
                {
                    case 2:
                        _maxEnemiesToSpawn = _baseEnemiesPerLevel + 2;
                        break;
                    case 3:
                        _maxEnemiesToSpawn = _baseEnemiesPerLevel + 4;
                        break;
                    case 4:
                        _maxEnemiesToSpawn = _baseEnemiesPerLevel + 6;
                        break;
                    default:
                        _canSpawn = false; // Stop spawning and prepare for the boss.
                        break;
                }    
            }
            yield return _spawnEnemyWaitForSeconds;
        }
        // Spawn the boss waves are completed
    }

    IEnumerator SpawnPowerup()
    {
        yield return new WaitForSeconds(3.0f);
        while (_canSpawn)
        {
            yield return new WaitForSeconds(Random.Range(4.0f, 8.0f));
            int _powerupID;
            // This is a poor implementation for limiting the rarity of the Ultra spawn, it requires the object to be at the end of the array to work. 
            if (Time.time - _lastUltraSpawn > _timebetweenUltraSpawns)
            {
                 _powerupID = Random.Range(0, _powerUpObject.Length);
                _lastUltraSpawn = Time.time;              
            }
            else
            {
                 _powerupID = Random.Range(0, _powerUpObject.Length - 1);
            }
            
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
