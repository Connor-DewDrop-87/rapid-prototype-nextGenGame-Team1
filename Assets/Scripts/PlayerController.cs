using System.Collections;
using System.Collections.Generic;
using System.Transactions;
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
    public float jumpHeight = 1.5f;
    public float baseJumpHeight;
    public float jumpMod;
    public float xPoint;
    public float zPoint;
    public Vector3 jumpTarget;
    public Vector3 jumpMidPoint;
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
    private float xRotation = 0;
    [Header("Flags")]
    public bool isDead = false;
    public bool touchedGrass = false;
    public bool chargingUp = false;
    public bool onSpring = false;
    public bool startedJumping = false;
    public enum State
    {
        NORMAL,
        JUMPING,
        DEAD,
        WON
    }
    public State currentState;
    void OnValidate()
    {
        // Snap values to increments of 0.5
        moveSpeed = Mathf.Round(moveSpeed / increment) * increment;
        mouseSensitivity = Mathf.Round(mouseSensitivity / increment) * increment;
        jumpHeight = Mathf.Round(jumpHeight / increment) * increment;
    }   //This is only for the slider snapping. Don't worry about it ;)

    void Start()
    {
        currentState = State.NORMAL;
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
        switch(currentState)
        {
            case State.NORMAL:
                HandleMovement();
                HandleGravity();
                HandleMouseLook();
                break;
            case State.JUMPING:
                StartCoroutine(JumpUpGoat());
                
                break;
            case State.DEAD:
                // Nothing...
                break;
            case State.WON:
                transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
                CameraRef.transform.Rotate(10 * Time.deltaTime, 10 * Time.deltaTime, 10 * Time.deltaTime);
                break;
        }
        
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

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButton("Jump"))
        {
            chargingUp = true;
        }
        else
        {
            chargingUp = false;
            jumpMod = 1 + Mathf.Pow(gm.jumpMeter.transform.localScale.x+0.5f,2);
            if (onSpring==true)
            {
                jumpMod += 1;
            }
            Debug.Log($"Jump Mod: {jumpMod}");
        }
        // While not charging up,
        if (chargingUp==false)
        {
            //If the player hits spacebar and the coyoteTime is reset, you can jump
            if (Input.GetButtonUp("Jump") && coyoteTime > 0f)
            {
                jumpHeight = baseJumpHeight * jumpMod;
                CheckAngle();
                jumpTarget = new Vector3(transform.position.x+xPoint, transform.position.y, transform.position.z + zPoint);
                jumpMidPoint = new Vector3(transform.position.x+xPoint/2, jumpHeight, transform.position.z + zPoint / 2);
                coyoteTime = 0f; // reset coyote time
                currentState = State.JUMPING;
            }
        }
        
        
    }

    void HandleGravity()
    {
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

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity; //These are not scaled by deltaTime. If things get jittery, add * Time.deltaTime and increase sensivity by 100x
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -20, 20);  

        CameraRef.localRotation = Quaternion.Euler(xRotation, 0f, 0f);  //Oy-ler. Only the camera can move up and down.
        
        transform.Rotate(Vector3.up * mouseX);
    }

    void CheckAngle()
    {
        float angleTheta = transform.rotation.y;
        Debug.Log($"{angleTheta}");
        xPoint = Mathf.Sin(angleTheta) * jumpHeight*jumpHeight;
        zPoint = Mathf.Tan(angleTheta) * xPoint*jumpHeight;
        if (angleTheta > Mathf.PI/4 || angleTheta < -Mathf.PI/4)
        {
            Debug.Log("Down");
            zPoint *= -1;
        }
        Debug.Log($"X: {xPoint}, Z: {zPoint}");
    }
    private IEnumerator JumpUpGoat()
    {
        if (startedJumping == true)
        {
            yield break;
        }

        startedJumping = true;

        Vector3 target = jumpMidPoint;
        while(MoveGoat(target))
        {
            yield return null;
        }
        target = jumpTarget;
        while(MoveGoat(jumpTarget))
        {
            yield return null;
        }
        startedJumping = false;
        jumpHeight = baseJumpHeight;
        currentState = State.NORMAL;
    }

    private bool MoveGoat(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, gm.animspeed*Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        // Kill the Player if they are touching an Obstacle
        if (other.gameObject.tag == "Obstacle")
        {
            currentState = State.DEAD;
        }
        if (other.gameObject.tag == "Spring")
        {
            // In the Jump Calculations, if this is true, jump will increase by 100% of base
            onSpring = true;
        }
        if (other.gameObject.tag == "Grass")
        {
            currentState = State.WON;
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

