using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y >= -5.5f)
        {
            transform.Translate(Vector3.down * 1.5f * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(Random.Range(-10.0f, 10.0f), 7.5f, 0);
        }
    }
}
