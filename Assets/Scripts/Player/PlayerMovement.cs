using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementState {
    GroundMovement,
}


public class PlayerMovement : MonoBehaviour
{
    public bool working = true;
    public float moveSpeed = 1f;
    public List<float> movementMultipliers = new List<float>();
    public List<float> velocityLimits = new List<float>();
    public MovementState state = MovementState.GroundMovement;

    [SerializeField] Transform orientation;

    private Vector3 moveDirection;

    private float horizontalMovement;
    private float verticalMovement;

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
        ControlVelocity();
    }
    void ControlInput() {
        horizontalMovement = Input.GetAxis("Horizontal");
        verticalMovement = Input.GetAxis("Vertical");
        
        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
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
