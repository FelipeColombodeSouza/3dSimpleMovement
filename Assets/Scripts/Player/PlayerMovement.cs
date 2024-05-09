using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    // Movimento
    [Header("Velocidade")]
    public float moveSpeed;
    public float groundDrag;

    // Pulo
    public float jumpForce;
    private float jumpCooldown;
    private float airMultiplier;
    private bool readyToJump = true;

    // Polimento Pulo
    public float coyoteTime = 0.15f;
    private float coyoteTimeCounter;

    private float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    [Header("Controles")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask Ground;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();

        MyInput();
        SpeedControl();

        Drag();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        Jump();
        /*
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
        */
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
        {
            //rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            rb.velocity = new Vector3(moveDirection.normalized.x * moveSpeed, 0f, moveDirection.normalized.z * moveSpeed);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * 0.5f, ForceMode.Force);
        }

    }

    int i = 0;

    private void Jump()
    {

        if (grounded)
        {
            coyoteTimeCounter = coyoteTime;
            readyToJump = true;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if(Input.GetKeyDown(jumpKey))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(jumpKey) && readyToJump && coyoteTimeCounter > 0f)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            readyToJump = false;
        }

        if (Input.GetKeyUp(jumpKey))
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.5f, rb.velocity.z);
            coyoteTimeCounter = 0f;
        }

        if(rb.velocity.y < 1f)
        {
            rb.velocity += Vector3.up * -2f * (2.5f) * Time.deltaTime;
        }

    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);
    }

    private void Drag()
    {
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

}
