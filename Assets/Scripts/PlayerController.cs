using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Base Player Script from Mark Shennelly 
public class Player : MonoBehaviour
{
    public Transform playerRef;
    GameManager gm;
    [Header("Movement Stuff")]
    [Range(0, 20)]
    public float moveSpeed = 6f;
    public float sprintScale = 1.5f;
    [Range(0, 10)]
    public float mouseSensitivity = 5;
    private float increment = 0.25f;    //Sliders move in 0.25 incremenets.
    [Header("Jump Stuff")]
    [Range(1, 10)]
    public float jumpHeight = 1.5f;
    public float baseJumpHeight;
    public float jumpMod;
    [Tooltip("This uses the default Unity gravity of -9.8, but change ONLY THE Y VALUE here if you want to adjust gravity")]
    public Vector3 gravity = Physics.gravity;   //Default will be Unity's gravity (-9.8) but can be changed here.

    private float coyoteTime;
    public float coyoteTimeMax = 0.1f; // 100ms coyote time

    [Tooltip("Percentage of jump height for a tap vs hold of the button (i.e., 0.5f = a tap of the jump button is 50% jump height)")]
    public float lowJumpFraction = 0.5f;
    [Tooltip("Speed up the player on descent. These numbers can be adjusted.")]
    public float fastFallMultiplier = 1.75f;

    [Header("DRAG THE CAMERA IN HERE OR THIS DOESN'T WORK AND YOU WILL GET VERY DIZZY!")]
    public Transform CameraRef;

    private CharacterController controller;
    private Vector3 velocity;

    [Header("Flags")]
    public bool isDead = false;
    public bool gotGrass = false;
    public bool chargingUp = false;
    public bool onSpring = false;
    void OnValidate()
    {
        // Snap values to increments of 0.5
        moveSpeed = Mathf.Round(moveSpeed / increment) * increment;
        mouseSensitivity = Mathf.Round(mouseSensitivity / increment) * increment;
        jumpHeight = Mathf.Round(jumpHeight / increment) * increment;
    }   //This is only for the slider snapping. Don't worry about it ;)

    void Start()
    {
        playerRef = GetComponent<Transform>();
        //Probably unnecessary, but just to make gravity gets assigned properly.
        gravity = Physics.gravity;
        baseJumpHeight = jumpHeight;
        //This sucker uses the CC instead of the RB.
        controller = GetComponent<CharacterController>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        //Hide the cursor. Hit ESC to bring it back.
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (isDead==true)
        {
            return;
        }
        HandleMovement();
    }
    void HandleMovement()
    {
        //if the player is on the ground, reset coyoteTime. If they are not on the ground, count down.
        if (controller.isGrounded)
            coyoteTime = coyoteTimeMax;
        else
            coyoteTime -= Time.deltaTime;

        //This makes sure the player stays on the ground. Sometimes physics are weird and you can get air when moving quickly.
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        //Input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //Calculate target movement direction
        Vector3 move = transform.right * x + transform.forward * z;

        // Determine current speed
        float currentSpeed = moveSpeed;

        //Go there!
        controller.Move(move * currentSpeed * Time.deltaTime);
        if (Input.GetButton("Jump"))
        {
            chargingUp = true;
        }
        else
        {
            chargingUp = false;
            jumpMod = 1 + Mathf.Pow(gm.jumpMeter.transform.localScale.x+0.5f,2);
            Debug.Log($"Jump Mod: {jumpMod}");
        }
        // While not charging up,
        if (chargingUp==false)
        {
            //If the player hits spacebar and the coyoteTime is reset, you can jump
            if (Input.GetButtonUp("Jump") && coyoteTime > 0f)
            {
                //Ooo...kinematics! v^2 = u^2 + 2(as) - AKA the third suvat equation
                //i.e., final velocity calculation is initial velocity squared + 2 * (acceleration * vector displacement [how far, and in what direction an object has moved
                //from its initial point to its end point])
                //0 = u^2 + 2 * a * s
                //u^2 = -2 * a * s
                //Therefore, u = √-2 * a * s

                //Neat, eh?
                jumpHeight = baseJumpHeight * jumpMod;
                if (onSpring==true)
                {
                    jumpHeight *= 2;
                }
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
                coyoteTime = 0f; // reset coyote time
            }
        }
        //Apply extra gravity when falling
        if (velocity.y < 0f)
        {
            velocity.y += Physics.gravity.y * (fastFallMultiplier - 1f) * Time.deltaTime;
        }

        //Apply normal gravity by default.
        velocity.y += Physics.gravity.y * Time.deltaTime;

        //No matter what the jumping scenario (?) is, apply that to the character controller. If no jump is being pressed, then we are scaling by 0 so nothing happens.
        controller.Move(Vector3.up * velocity.y * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Kill the Player if they are touching an Obstacle
        if (other.gameObject.tag == "Obstacle")
        {
            isDead = true;
        }
        if (other.gameObject.tag == "Spring")
        {
            // In the Jump Calculations, if this is true, jump will be doubled
            onSpring = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Spring")
        {
            // No longer on Spring
            onSpring = false;
        }
    }




}

