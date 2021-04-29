using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissle : MonoBehaviour
{
    private GameObject[] _enemiesOnScreen;
    private GameObject _targetedEnemy;
    private float _closestDistance;
    private bool _first = true;
    private bool _moveToPlayer = true;
    private Vector3 _origPos;
   
    // Start is called before the first frame update
    private void Start()
    {
        _enemiesOnScreen = GameObject.FindGameObjectsWithTag("Enemy");
        _origPos = transform.position;

        foreach (var obj in _enemiesOnScreen)
        {
            var distance = Vector3.Distance(obj.transform.position, transform.position);
            if (_first)
            {
                _targetedEnemy = obj;
                _closestDistance = distance;
                _first = false;
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
        if (_moveToPlayer)
        {
            StartCoroutine(MovingToPlayer());
        }

    }

    IEnumerator MovingToPlayer()
    {
        _moveToPlayer = false;
        while (true)
        {
            if (_targetedEnemy == null)
            {
                while (true)
                {
                    transform.Translate(transform.position + new Vector3(1,1,1 * (6f * Time.deltaTime)));
                }
            }
            this.gameObject.transform.position = Vector3.MoveTowards(transform.position,_targetedEnemy.gameObject.transform.position, 6f * Time.deltaTime);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
