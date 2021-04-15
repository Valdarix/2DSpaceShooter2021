using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyOject;
    private bool _enemyCanSpawn = true;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private float _spawnTimer = 5.0f;
    private WaitForSeconds _spawnWaitForSeconds;
    // Start is called before the first frame update
    void Start()
    {
        _spawnWaitForSeconds = new WaitForSeconds(_spawnTimer);
        StartCoroutine("SpawnEnemy");
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    IEnumerator SpawnEnemy()
    {
        while (_enemyCanSpawn)
        {
            Vector3 randomLocation = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyOject, randomLocation, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return _spawnWaitForSeconds;
        }
    }

    public void StopSpawning()
    {
        _enemyCanSpawn = false;
        StopCoroutine("SpawnEnemy");
    }
}
