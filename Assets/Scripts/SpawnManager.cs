using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _asteroid;
    [SerializeField]
    private GameObject[] _powerUpObjectT1;
    [SerializeField]
    private GameObject[] _powerUpObjectT2;
    [SerializeField]
    private GameObject[] _powerUpObjectT3;
    [SerializeField]
    private GameObject[] _enemyListT1;
    [SerializeField]
    private GameObject[] _enemyListT2;
    [SerializeField]
    private GameObject[] _enemyListT3;
    private bool _canSpawn = false;   
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject _powerUpContainer;
    [SerializeField]
    private float _spawnTimer = 5.0f;
    [SerializeField]
    private WaitForSeconds _spawnEnemyWaitForSeconds;   
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
            //Get Enemy to Spawn. 3 Tier System
            PickEnemy();
            
            _maxEnemiesToSpawn--;            
            if (_maxEnemiesToSpawn == 0) //handle spawning new wave
            {               
                _currentWave++;
                _gameManager.NewWave();
                _spawnTimer -= 1f;
                _spawnEnemyWaitForSeconds = new WaitForSeconds(_spawnTimer);
               
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

    private void PickEnemy()
    {        
        // roll random 100. Tier 1 = 0 - 60, Tier 2 = 61 - 85, Tier 3 = 75 - 99 
        int randomRoll = Random.Range(1, 100);
        Vector3 randomLocation = new Vector3(Random.Range(-8f, 8f), 7, 0);        
        switch (randomRoll)
        {
            case int n when (n >= 0 && n <= 60):
                GameObject newEnemyT1 = Instantiate(_enemyListT1[Random.Range(0,_enemyListT1.Length)], randomLocation, Quaternion.identity);
                newEnemyT1.transform.parent = _enemyContainer.transform;
                break;
            case int n when (n >= 61 && n <= 85):
                GameObject newEnemyT2 = Instantiate(_enemyListT2[Random.Range(0, _enemyListT2.Length)], randomLocation, Quaternion.identity);
                newEnemyT2.transform.parent = _enemyContainer.transform;
                break;
            case int n when (n >= 86 && n <= 99):
                GameObject newEnemyT3 = Instantiate(_enemyListT3[Random.Range(0, _enemyListT3.Length)], randomLocation, Quaternion.identity);
                newEnemyT3.transform.parent = _enemyContainer.transform;
                break;
            default:              
                break;
        }     

    }

    IEnumerator SpawnPowerup()
    {
        yield return new WaitForSeconds(3.0f);
        while (_canSpawn)
        {
            yield return new WaitForSeconds(Random.Range(4.0f, 8.0f));
            // roll random 100. Tier 1 = 0 - 60, Tier 2 = 61 - 85, Tier 3 = 75 - 99 
            int randomRoll = Random.Range(1, 100);
            Vector3 randomLocation = new Vector3(Random.Range(-8f, 8f), 7, 0);           
            switch (randomRoll)
            {
                case int n when (n >= 1 && n <= 60):
                    GameObject newPowerupT1 = Instantiate(_powerUpObjectT1[Random.Range(0, _powerUpObjectT1.Length)], randomLocation, Quaternion.identity);
                    newPowerupT1.transform.parent = _powerUpContainer.transform;
                    break;
                case int n when (n >= 61 && n <= 85):
                    GameObject newPowerupT2 = Instantiate(_powerUpObjectT2[Random.Range(0, _powerUpObjectT2.Length)], randomLocation, Quaternion.identity);
                    newPowerupT2.transform.parent = _powerUpContainer.transform;
                    break;
                case int n when (n >= 86 && n <= 99):
                    GameObject newPowerupT3 = Instantiate(_powerUpObjectT3[Random.Range(0, _powerUpObjectT3.Length)], randomLocation, Quaternion.identity);
                    newPowerupT3.transform.parent = _powerUpContainer.transform;
                    break;
                default:
                    break;
            }
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
