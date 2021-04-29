using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeDetection : MonoBehaviour
{
    private EnemyBehavior _enemyParent;

    // Start is called before the first frame update
    private void Start()
    {
        _enemyParent = this.gameObject.transform.parent.gameObject.GetComponent<EnemyBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Laser")) return;
        _enemyParent.Dodge();
        Destroy(gameObject); //Prevent him from dodging more than once. 
    }
}
