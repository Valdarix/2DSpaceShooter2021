using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissle : MonoBehaviour
{
    [SerializeField] private GameObject[] enemiesOnScreen;
    private GameObject _targetedEnemy;
    private float _closestDistance;
    private bool isClosest = true;
    
    // Start is called before the first frame update
    private void Start()
    {
        enemiesOnScreen ??= GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var obj in enemiesOnScreen)
        {
            var distance = Vector3.Distance(obj.transform.position, transform.position);
            if (isClosest)
            {
                
            }
            else if (distance < _closestDistance)
            {
                _targetedEnemy = obj;
                _closestDistance = distance;
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        var step = 4 * Time.deltaTime;
        transform.Translate(Vector3.MoveTowards(_targetedEnemy.transform.position,_targetedEnemy.transform.position,step));
    }
}
