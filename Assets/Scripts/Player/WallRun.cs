using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour {
    public bool working = true;

    [Header("Movement")]
    [SerializeField] private Transform orientation;

    [Header("Detection")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float wallDistance = .5f;
    [SerializeField] private float minimumJumpHeight = 1.5f;

    [Header("Wall Running")]
    [SerializeField] private float wallRunGravity;
    [SerializeField] private float wallRunJumpForce;

    [SerializeField] private PlayerCamera playerCam;


    public float tilt { get; private set; }

    public bool isWallRuning = false;

    private bool wallLeft = false;
    private bool wallRight = false;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    private Rigidbody rb;

    bool CanWallRun() {
        if (!working)
            return false;
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void CheckWall() {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance, wallMask);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance, wallMask);
    }

    private void FixedUpdate() {
        CheckWall();

        if (CanWallRun()) {
            if (wallLeft) {
                StartWallRun();
                //Debug.Log("wall running on the left");
            }
            else if (wallRight) {
                StartWallRun();
                //Debug.Log("wall running on the right");
            }
            else {
                StopWallRun();
            }
        }
        else {
            StopWallRun();
        }
        ControlWallRun();
    }
    private void Update() {
        ControlWallRunJump();
    }

    private void ControlWallRun() {
        if (!isWallRuning)
            return;


        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);
    }
    private void ControlWallRunJump() {
        if (!isWallRuning)
            return;


        if (Input.GetKeyDown(KeyCode.Space)) {
            if (wallLeft) {
                Vector3 wallRunJumpDirection = transform.up + leftWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce, ForceMode.Impulse);
            }
            else if (wallRight) {
                Vector3 wallRunJumpDirection = transform.up + rightWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce, ForceMode.Impulse);
            }
        }
    }

    void StartWallRun() {
        isWallRuning = true;
        rb.useGravity = false;

        playerCam.StartWallRun(wallLeft, wallRight);
    }

    void StopWallRun() {
        isWallRuning = false;
        rb.useGravity = true;

        playerCam.StopWallRun();
    }
}