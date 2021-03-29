using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float _speed = 3.5f;
    void Start()
    {
        //take the current position and assign a start position of (0,0,0)
        transform.position = new Vector3(0,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        //get our horizontal input from input manager
        float horizontalInput = Input.GetAxis("Horizontal");
        //get our vertical input from input manager
        float verticalInput = Input.GetAxis("Vertical");
        //Determine dirction. Normalize the direction to prevent exponential speed increase on diagonal
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0).normalized;       

        //use Translate - movement is modified by speed and delta time Vector3 * speed * deltatime
        transform.Translate(direction * _speed * Time.deltaTime);

        // clamp the Y positionn. 
        float yClamp = Mathf.Clamp(transform.position.y, -4f, 6f);
        transform.position = new Vector3(transform.position.x, yClamp, 0);

        // use 11.5, anything less seems to cause a bug if the player tries to shift back in the other
        // direction too quickly. Multiple both by *1 to handle both in one line of code. 
        if (transform.position.x >= 11.5 || (transform.position.x <= -11.5))
        {           
            transform.position = new Vector3(transform.position.x * -1, transform.position.y,0);
        }
     

    }

    

   


}
