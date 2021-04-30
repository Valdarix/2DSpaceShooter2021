using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour
{

    [SerializeField] private GameObject turretProjectile1;

    [SerializeField] private bool isLeft;
    [SerializeField] private Vector3 test;
    [SerializeField] private Transform bigBossTransform;
    
    // Start is called before the first frame update
 
    public void FireLaser()
    {
        if (isLeft)
        {
            Quaternion prefabRotation;
            prefabRotation = turretProjectile1.transform.rotation;
            Instantiate(turretProjectile1, transform.position, prefabRotation);
            GameObject shot = (GameObject) Instantiate(turretProjectile1, transform.position, prefabRotation);
            shot.SendMessage("GoTowards",transform.forward);
        }

    }
}
