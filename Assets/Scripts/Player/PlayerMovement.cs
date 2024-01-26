using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementState {
    GroundMovement,
    AirMovement,
}


public class PlayerMovement : MonoBehaviour
{
    public bool working = true;
    public float moveSpeed = 1f;
    public List<float> movementMultipliers = new List<float>();
    public List<float> drags = new List<float>();
    public List<float> velocityLimits = new List<float>();
    public MovementState state = MovementState.GroundMovement;

    [SerializeField] Transform orientation;
    [SerializeField] Transform groundPos;
    [SerializeField] float groundDistance = 0.1f;
    [SerializeField] float jumpForce = 1;
    [SerializeField] LayerMask groundMask;

    private Vector3 moveDirection;

    private float horizontalMovement;
    private float verticalMovement;
    private bool isGrounded = false;

    private Rigidbody rb;


    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        if (!working) {
            return;
        }
        ControlInput();
    }
    private void FixedUpdate() {
        if (!working) {
            return;
        }
        Move();
        ControlDrag();
//        ControlVelocity();
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(groundPos.position, groundDistance);
    }

    void UpdateGrounded() {
        bool newGrounded = Physics.CheckSphere(groundPos.position, groundDistance, groundMask);
        isGrounded = newGrounded;

        if (!isGrounded)
            state = MovementState.AirMovement;
        else
            state = MovementState.GroundMovement;
    }

    private void ControlDrag() {
        UpdateGrounded();
        if (state == MovementState.GroundMovement)
            rb.drag = drags[0];
        if (state == MovementState.AirMovement)
            rb.drag = drags[1];
    }

    void ControlInput() {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");
        
        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;

        if (Input.GetKeyDown(KeyCode.Space)) {
            UpdateGrounded();
            Jump();
        }
    }

    private void Jump() {
        Debug.Log(isGrounded);
        if (isGrounded) {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(orientation.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void Move() {
        rb.AddForce(moveDirection.normalized * moveSpeed * movementMultipliers[(int)state], ForceMode.Acceleration);
    }
    void ControlVelocity() {
        float speedLimit = velocityLimits[(int)state];
//        Debug.Log(rb.velocity.magnitude);
        if (rb.velocity.magnitude > speedLimit) {
            // Calculate the force to apply in the opposite direction
            Vector3 vel = rb.velocity.normalized * (speedLimit);

            Debug.Log(vel);

            // Apply the force to slow down the rigidbody
            rb.velocity = vel;
        }
    }
}
