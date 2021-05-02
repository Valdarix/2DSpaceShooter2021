using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAmmo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  void GoTowards(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction) * transform.rotation;
        this.gameObject.GetComponent<Rigidbody2D>().velocity = direction*4;
    }
}
