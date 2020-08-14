using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    // Start is called before the first frame update
    public float walkSpeed = 2f;
    public float runSpeed = 6f;
    public float gravity = -30f;
    public float jumpHeight = 1f;
    [Range(0,1)]
    public float airControlPercent;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;

    float velocityY;

    Transform cameraT;

    Animator animator;

    CharacterController controller;
    void Start()
    {
        cameraT = Camera.main.transform;
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //2D Vector to represent our mouse movement
        Vector2 inputDir = input.normalized; // Normalize the mouse movement

        if (inputDir != Vector2.zero) 
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y; // the Target rotation relative to inputDir (using Triginometry, google ArcTan)
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime)); // Smoothing to our target location using SmoothDampAngle

        }

        bool running = Input.GetKey(KeyCode.LeftShift); // Get if we are holding down shift or not

        float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude; // if running, set it to our desired run speed, else, set it to walk speed (inputDir.magnitude?)
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime)); // Smooth our transition from one speed to another


        if (!controller.isGrounded)
        {
            velocityY += Time.deltaTime * gravity;
        } else
        {
            velocityY = -Mathf.Epsilon;
        }

        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY; // move along the x axis (transform.forward)
        controller.Move(velocity * Time.deltaTime);

        currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        // Control the Animator Variable SpeedPercent (edit by changing speedSmoothTime in editor)
        // NOTE: inputDir.magnitude is essentially the farthest we can travel along the ground
        float animationSpeedPercent = ((running)? currentSpeed/runSpeed : currentSpeed/walkSpeed * 0.5f); 
        animator.SetFloat("SpeedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
    }

    void Jump()
    {
       if (isGrounded())
       {
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            velocityY = jumpVelocity;
       }
    }

    float GetModifiedSmoothTime(float smoothTime)
    {
        if (isGrounded())
        {
            return smoothTime;
        }
        if (airControlPercent == 0)
        {
            return float.MaxValue;
        }
        return smoothTime / airControlPercent;
    }

    private bool isGrounded()
    {

        return Physics.Raycast(transform.position, Vector3.down, 0.15f);
        
    }
}
