using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyStates {
    grounded,
    WallRunning,
    Jumping,
}

public class PlayerMovement : NetworkBehaviour
{
    public static float wallDirection = 1;
    public static bool isWallRunning = false;

    float playerHeight = 2f;

    [SerializeField] Transform orientation;
    [SerializeField] Transform spawnPoint;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float airMultiplier = 0.4f;
    float movementMultiplier = 10f;

    [Header("Sprinting")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float sprintSpeed = 6f;
    [SerializeField] float acceleration = 10f;

    [Header("Jumping")]
    public float jumpForce = 5f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Drag")]
    [SerializeField] float groundDrag = 6f;
    [SerializeField] float airDrag = 2f;

    float horizontalMovement;
    float verticalMovement;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 0.2f;

    [Header ("Animations")]
    [SerializeField] Animator bodyAnim;

    public bool isGrounded { get; private set; }
    [SyncVar]
    public bool controlsEnabled = true;

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    Rigidbody rb;

    RaycastHit slopeHit;

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
    private void Start()
    {
        spawnPoint = NetworkLevelData.singleton.SpawnPoint;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update() {

        ControlGrounded ();
        MyInput();
        ControlDrag();
        ControlSpeed();

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }

        HandleAnimations ();
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    void MyInput()
    {
        if (controlsEnabled)
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");
            verticalMovement = Input.GetAxisRaw("Vertical");
        }
        else
        {
            horizontalMovement = 0;
            verticalMovement = 0;
        }

        if (Cursor.lockState != CursorLockMode.Locked) {
            horizontalMovement = 0;
            verticalMovement = 0;
        }

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    void Jump()
    {
        if (isGrounded && controlsEnabled)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    void ControlGrounded () {
        bool newGrounded = Physics.CheckSphere (groundCheck.position, groundDistance, groundMask);
        isGrounded = newGrounded;
    }
    void ControlSpeed()
    {
        if (Input.GetKey(sprintKey) && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }

    void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
    }

    private void HandleAnimations () {
        bodyAnim.SetFloat ("SpeedHorizontal", horizontalMovement, 0.2f, Time.deltaTime);
        bodyAnim.SetFloat ("SpeedVertical", verticalMovement, 0.2f, Time.deltaTime);
        bodyAnim.SetFloat ("wallDirection", (int)wallDirection);
        
        if (isWallRunning) {
            bodyAnim.SetInteger ("state", (int)BodyStates.WallRunning);
        } else if (isGrounded) {
            bodyAnim.SetInteger ("state", (int)BodyStates.grounded);
        } else if (!isGrounded) {
            bodyAnim.SetInteger ("state", (int)BodyStates.Jumping);
        }
    }
}