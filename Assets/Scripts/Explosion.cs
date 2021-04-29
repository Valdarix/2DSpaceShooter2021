using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(ExplosionDamage());
        Destroy(this.gameObject, 2.0f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.transform.GetComponent<Player>();
        if (player != null)
        {
            //Apply Damage to player
            player.DamagePlayer(1);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator ExplosionDamage()
    {
        yield return new WaitForSeconds(1f);
        this.gameObject.GetComponent<CircleCollider2D>().enabled = false;
    }

}
