using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

// player with charge jump
// hold space to charge, let go to jump
// cant move or look while in the air after jumping
[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class ChargeJumpPlayer : MonoBehaviour
{
    public enum JumpState { Ready, Charging, Airborne }

    [Header("References")]
    public Transform cameraRef;
    public GameObject chargeIndicator;
    GameManager gm;
    JumpMeter jm;
    public GameObject jumpArrowIndicator;
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float mouseSensitivity = 2f;
    public float pitchClamp = 20f;

    [Header("Ground Check")]
    public LayerMask groundMask = ~0;
    public float groundCheckExtra = 0.15f;

    [Header("Charge")]
    public float maxChargeTime = 1.2f;

    [Header("Launch")]
    public float minUp = 3f;
    public float maxUp = 9f;
    public float minForward = 2f;
    public float maxForward = 10f;
    public float powerMultiplier = 1f;

    [Header("Air")]
    public float fastFallMultiplier = 1.75f;
    [Range(0, 1)] public float wallBounciness = 0.6f;
    public float wallAngle = 60f;
    public float landIgnoreTime = 0.15f;

    [Header("Debug")]
    public JumpState state = JumpState.Ready;
    [SerializeField] bool isGrounded;
    public float ChargePercent => Mathf.Clamp01(chargeTimer / maxChargeTime);

    Rigidbody rb;
    CapsuleCollider capsule;
    Vector2 moveInput;
    float yaw, pitch;
    float chargeTimer;
    bool launchQueued;
    float launchTime;
    Vector3 lastVelocity;

    [Header("Flags")]
    public bool isDead = false;
    public bool touchedGrass = false;
    public bool onSpring = false;
    

    // velocity is linearVelocity in unity 6
    Vector3 Velocity
    {
#if UNITY_6000_0_OR_NEWER
        get => rb.linearVelocity;
        set => rb.linearVelocity = value;
#else
        get => rb.velocity;
        set => rb.velocity = value;
#endif
    }

    bool CanLook => state != JumpState.Airborne;
    bool CanWalk => state == JumpState.Ready;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        jm = GameObject.Find("JumpMeter").GetComponent<JumpMeter>();
        yaw = transform.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
        jumpArrowIndicator.SetActive(false);
    }

    void Update()
    {
        Debug.Log($"Charge Percent: {ChargePercent}");
        // Touching Grass does Wonders
        if (touchedGrass == true)
        {
            return;
        }
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (CanLook) HandleLook();
        HandleChargeInput();

        if (chargeIndicator != null)
            chargeIndicator.SetActive(state == JumpState.Charging);
    }

    void FixedUpdate()
    {
        // Touching Grass does Wonders
        if (touchedGrass==true)
        {
            return;
        }
        CheckGround();

        if (launchQueued) Launch();

        if (CanWalk) HandleWalk();

        CheckLanding();

        // fall faster
        if (!isGrounded && Velocity.y < 0f)
            rb.AddForce(Physics.gravity * (fastFallMultiplier - 1f), ForceMode.Acceleration);

        // save velocity for wall bounce
        lastVelocity = Velocity;
        CheckJumpIndicator();
    }

    // ground check
    void CheckGround()
    {
        Vector3 origin = transform.TransformPoint(capsule.center);
        float reach = capsule.height * 0.5f * transform.lossyScale.y + groundCheckExtra;
        isGrounded = Physics.Raycast(origin, Vector3.down, reach, groundMask, QueryTriggerInteraction.Ignore);
    }

    // mouse look
    void HandleLook()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -pitchClamp, pitchClamp);

        Quaternion body = Quaternion.Euler(0f, yaw, 0f);
        rb.rotation = body;
        transform.rotation = body;
        if (cameraRef != null) cameraRef.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    // walking
    void HandleWalk()
    {
        Vector3 wish = transform.right * moveInput.x + transform.forward * moveInput.y;
        wish = Vector3.ClampMagnitude(wish, 1f) * moveSpeed;
        Velocity = new Vector3(wish.x, Velocity.y, wish.z);
    }

    // charging
    void HandleChargeInput()
    {
        if (state == JumpState.Ready && isGrounded && Input.GetButtonDown("Jump"))
        {
            state = JumpState.Charging;
            chargeTimer = 0f;
            Velocity = new Vector3(0f, Velocity.y, 0f);   // stop moving
        }
        else if (state == JumpState.Charging && !launchQueued)
        {
            chargeTimer = Mathf.Min(chargeTimer + Time.deltaTime, maxChargeTime);

            if (!isGrounded)                        // fell off so cancel
                ResetJump();
            else if (Input.GetButtonUp("Jump"))
                launchQueued = true;                // jump next fixed update
        }
    }

    void Launch()
    {
        launchQueued = false;

        float t = ChargePercent;
        float up = Mathf.Lerp(minUp, maxUp, t) * powerMultiplier;
        float forward = Mathf.Lerp(minForward, maxForward, t) * powerMultiplier;

        Velocity = transform.forward * forward + Vector3.up * up;

        launchTime = Time.time;
        state = JumpState.Airborne;
    }

    // landing
    void CheckLanding()
    {
        if (state != JumpState.Airborne) return;

        bool pastLaunch = Time.time - launchTime > landIgnoreTime;
        if (isGrounded && pastLaunch && Velocity.y <= 0.1f)
        {
            Velocity = new Vector3(0f, Velocity.y, 0f);   // stop sliding
            ResetJump();
        }
    }

    void ResetJump()
    {
        state = JumpState.Ready;
        chargeTimer = 0f;
        launchQueued = false;
    }

    void CheckJumpIndicator()
    {
        if (state == JumpState.Charging)
        {
            jumpArrowIndicator.SetActive(true);
        }
        else
        {
            jumpArrowIndicator.SetActive(false);
        }
    }

    // bounce off walls
    void OnCollisionEnter(Collision collision)
    {
        if (state != JumpState.Airborne) return;

        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            float angle = Vector3.Angle(normal, Vector3.up);
            if (angle < wallAngle || angle > 180f - wallAngle) continue;   // skip floor and ceiling

            Vector3 wall = new Vector3(normal.x, 0f, normal.z).normalized;
            Vector3 incoming = new Vector3(lastVelocity.x, 0f, lastVelocity.z);
            if (Vector3.Dot(incoming, wall) >= 0f) return;

            Vector3 outgoing = Vector3.Reflect(incoming, wall) * wallBounciness;
            Velocity = new Vector3(outgoing.x, lastVelocity.y, outgoing.z);
            return;
        }
    }

    void OnDrawGizmosSelected()
    {
        var c = GetComponent<CapsuleCollider>();
        if (c == null) return;
        Vector3 origin = transform.TransformPoint(c.center);
        float reach = c.height * 0.5f * transform.lossyScale.y + groundCheckExtra;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(origin, origin + Vector3.down * reach);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Kill the Player if they are touching an Obstacle
        if (other.gameObject.tag == "Obstacle")
        {
            gm.previousScene = SceneManager.GetActiveScene().name;
            isDead = true;
        }
        if (other.gameObject.tag == "Spring")
        {
            // In the Jump Calculations, if this is true, jump will increase by 100% of base
            onSpring = true;
        }
        if (other.gameObject.tag == "Grass")
        {
            NextLevel tl = other.gameObject.GetComponent<NextLevel>();
            gm.nextScene = tl.nextLevel;
            touchedGrass = true;
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
