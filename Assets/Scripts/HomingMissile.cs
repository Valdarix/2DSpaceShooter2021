using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    private GameObject[] _enemiesOnScreen;
    private GameObject _targetedEnemy;
    private float _closestDistance;
    private bool _first = true;
    private Rigidbody2D _rb;
    private Vector2 _currentDirection;
    [SerializeField] private float speed = 25f;
    [SerializeField] private float rotateSpeed = 300f;
    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
       _enemiesOnScreen = GameObject.FindGameObjectsWithTag("Enemy");

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

    private void FixedUpdate()
    {
        var up = transform.up;
        if (_targetedEnemy == null)
        {
            _rb.angularVelocity = 0;
            _rb.velocity = Vector2.zero;
            transform.Translate(Vector3.up * (speed * Time.deltaTime));
        }
        else {
            var direction = (Vector2)_targetedEnemy.gameObject.transform.position - _rb.position;
            direction.Normalize();
            _currentDirection = direction;
            _rb.velocity = up * speed;
            var rotateAmount = Vector3.Cross(direction, up).z;
            _rb.angularVelocity = -rotateAmount * rotateSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Asteroid"))
        {
            Destroy(gameObject);
        }
    }
}
