﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform orientation;

    [Header("Detection")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float wallDistance = .5f;
    [SerializeField] private float minimumJumpHeight = 1.5f;

    [Header("Wall Running")]
    [SerializeField] private float wallRunGravity;
    [SerializeField] private float wallRunJumpForce;

    [Header("Camera")]
    [SerializeField] private float wallRunfov;
    [SerializeField] private float camTilt;
    [SerializeField] private float camTiltTime;

    public float tilt { get; private set; }

    public bool controlsEnabled = true;
    private bool wallLeft = false;
    private bool wallRight = false;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    private Rigidbody rb;

    bool CanWallRun()
    {
        if (!controlsEnabled)
            return false;
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void CheckWall()
    {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance, wallMask);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance, wallMask);
    }

    private void Update()
    {
        CheckWall();

        if (CanWallRun())
        {
            if (wallLeft)
            {
                StartWallRun();
//                Debug.Log("wall running on the left");
            }
            else if (wallRight)
            {
                StartWallRun();
//                Debug.Log("wall running on the right");
            }
            else
            {
                StopWallRun();
            }
        }
        else
        {
            StopWallRun();
        }
    }

    void StartWallRun()
    {
        PlayerMovement.isWallRunning = true;
        if (wallLeft) {
            PlayerMovement.wallDirection = -1;
        }
        if (wallRight) {
            PlayerMovement.wallDirection = 1;
        }

        rb.useGravity = false;

        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

        CameraFOVmanager.singleton.AddCommand (wallRunfov, FOVchangeSource.WallRun);

        if (wallLeft)
            tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);
        else if (wallRight)
            tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);


        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (wallLeft)
            {
                PlayerMovement.isWallRunning = false;
                Vector3 wallRunJumpDirection = transform.up + leftWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
            else if (wallRight)
            {
                PlayerMovement.isWallRunning = false;
                Vector3 wallRunJumpDirection = transform.up + rightWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); 
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
        }
    }

    void StopWallRun()
    {
        rb.useGravity = true;
        PlayerMovement.isWallRunning = false;

        tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
    }
}