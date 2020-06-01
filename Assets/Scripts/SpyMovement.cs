using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpHeight = 2f;
    public float distanceToTheGround;
    public bool isOnGround;

    [SerializeField]
    private Rigidbody playerBody;
    private Vector3 inputVector;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        HandlePlayerMovement();
        HandleJump();
    }

    void HandlePlayerMovement()
    {
        ////Move in absolute directions
        //inputVector = new Vector3(Input.GetAxis("Horizontal") * speed, playerBody.velocity.y, Input.GetAxis("Vertical") * speed);
        ////Look at moving direction
        //transform.LookAt(transform.position + new Vector3(inputVector.x, 0f, inputVector.z));
        //playerBody.velocity = inputVector;

        float x = Input.GetAxis("Horizontal") * speed;
        float z = Input.GetAxis("Vertical") * speed;

        inputVector = transform.right * x + transform.up * playerBody.velocity.y + transform.forward * z;

        playerBody.velocity = inputVector;
    }

    void HandleJump()
    {
        distanceToTheGround = GetComponent<Collider>().bounds.extents.y;

        isOnGround = Physics.Raycast(transform.position, Vector3.down, distanceToTheGround + 0.1f);

        if (Input.GetButtonDown("Jump") && isOnGround)
        {
            playerBody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }
    }
}
