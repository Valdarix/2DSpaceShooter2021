using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ExplosionDamage());
        Destroy(this.gameObject, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.transform.GetComponent<Player>();
        if (player != null)
        {
            //Apply Damange to player
            player.DamagePlayer(1);
            
        }
    }

    IEnumerator ExplosionDamage()
    {
        yield return new WaitForSeconds(1f);
        this.gameObject.GetComponent<CircleCollider2D>().enabled = false;
    }

}
