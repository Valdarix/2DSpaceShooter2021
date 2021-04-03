using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyOject;
    [SerializeField]
    private float _spawnTimer = 5.0f;
    private bool _enemyCanSpawn = true;
    [SerializeField]
    private GameObject _enemyContainer;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemy());
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
            yield return new WaitForSeconds(_spawnTimer);
        }
    }

    public void StopSpawning()
    {
        _enemyCanSpawn = false;
    }
}
