using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class LaserBehavior : MonoBehaviour
{
    [FormerlySerializedAs("_speed")] [SerializeField]private float speed = 1.0f;
    [FormerlySerializedAs("_rotateSpeed")] [SerializeField] private float rotateSpeed = 1.0f;
    [FormerlySerializedAs("_destroyOnTrigger")] [SerializeField] private bool destroyOnTrigger = true;  

    private void Start()
    {
        // if this is false we don't want to destroy on trigger so we start the cooldown for destruction on spawn
        if (destroyOnTrigger == false)
        { 
            StartCoroutine(Pulse());
        }
    }

    // Update is called once per frame
    private void Update()
    {

        if (speed > 0f)
        {
            HandleMovement();
        }

        if (rotateSpeed > 0f)
        {
            HandleRotate();
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {   
        if (this.CompareTag("Laser"))
        {
            if (other.CompareTag("Enemy") || other.CompareTag("Asteroid"))
            {
                if (destroyOnTrigger)
                {
                    Destroy(this.gameObject);
                }
            }
        }

        if (!this.CompareTag("EnemyLaser")) return;
        if (other.CompareTag("Player"))
        {
            if (destroyOnTrigger != true) return;
            var player = other.transform.GetComponent<Player>();
            if (player == null) return;
            //Apply Damage to player
            player.DamagePlayer(1);
            Destroy(this.gameObject);
        }
        else if (other.CompareTag("Powerup"))
        {
            var powerupToDestroy = other.transform.GetComponent<Powerup>();
            powerupToDestroy.DestroyPowerup();
            Destroy(this.gameObject);
        }

    }

    private void HandleMovement()
    {
        
        if (this.CompareTag("Laser"))
        { 
            transform.Translate(Vector3.up * (speed * Time.deltaTime));

            if (transform.position.y > 8.0f)
            {
                Destroy(this.gameObject);
            }
        }
        
        if (!this.CompareTag("EnemyLaser")) return;
        transform.Translate(Vector2.up * (speed * Time.deltaTime));


        if (!(transform.position.y < -8.0f)) return;
        Destroy(this.gameObject);
    }

    private void HandleRotate()
    {
        transform.Rotate(Vector3.forward * (rotateSpeed * Time.deltaTime));
    }

    private IEnumerator Pulse()
    {
        yield return new WaitForSeconds(1.2f);
        Destroy(gameObject);
    }    
}
