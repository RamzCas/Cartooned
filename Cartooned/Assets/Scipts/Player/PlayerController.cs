using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public Transform groundCheck;
    public LayerMask groundMask;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference lookAction;
    public InputActionReference jumpAction;
    public InputActionReference sprintAction;
    public InputActionReference shootAction;

    [Header("Speed")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8.5f;

    [Header("Momentum")]
    public bool CanWalk;
    public bool CanSprint;
    public float acceleration = 45f;
    public float deceleration = 12f;
    [Range(0f, 1f)]
    public float airControlMultiplier = 0.35f;

    [Header("Jumping")]
    public float jumpForce = 6.5f;
    public float groundCheckRadius = 0.25f;
    public float extraGravityForce = 12f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.1f;
    public float maxLookAngle = 85f;

    [Header("Shooting")]
    public bool canShoot;
    public GameObject Bullet;
    //public Rigidbody bulletRigidbody;
    //public float bulletSpeed;
    //public Transform gunHoldPt;
    public Transform firePt;
    
    private Rigidbody rb;
    private Vector2 moveInput;
    private float cameraPitch;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        CanWalk = true;
        CanSprint = true;
    }

    void Start()
    {
    
    }

    void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        jumpAction.action.Enable();
        shootAction.action.Enable();
        jumpAction.action.performed += OnJumpPerformed;
        shootAction.action.performed += OnShootPerformed;
        if (sprintAction != null) sprintAction.action.Enable();

    }

    void OnDisable()
    {
        jumpAction.action.performed -= OnJumpPerformed;
        shootAction.action.performed -= OnShootPerformed;
        moveAction.action.Disable();
        lookAction.action.Disable();
        jumpAction.action.Disable();
        shootAction.action.Disable();
        if (sprintAction != null) sprintAction.action.Disable();
    }

    void Update()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();
        HandleMouseLook();
    }

    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
        HandleMovement();

        if (!isGrounded)
            rb.AddForce(Vector3.down * extraGravityForce, ForceMode.Acceleration);
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (!isGrounded) return;

        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }

    public void OnShootPerformed(InputAction.CallbackContext context) 
    {
        if(canShoot) 
        {
            Debug.Log("Shoot");
            Instantiate(Bullet, firePt.transform.position, firePt.rotation);
            //bulletRigidbody.AddForce(transform.forward * bulletSpeed * Time.deltaTime, ForceMode.Acceleration);
            
        }
    }

    private void HandleMovement()
    {
        if(CanWalk) 
        {
            Vector3 inputDir = Vector3.ClampMagnitude(transform.right * moveInput.x + transform.forward * moveInput.y, 1f);

            bool sprinting = sprintAction != null && sprintAction.action.IsPressed() && CanSprint;
            Vector3 targetVelocity = inputDir * (sprinting ? sprintSpeed : walkSpeed);

            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            Vector3 velocityDelta = targetVelocity - horizontalVelocity;

            float forceStrength = inputDir.sqrMagnitude > 0.01f ? acceleration : deceleration;
            if (!isGrounded) forceStrength *= airControlMultiplier;

            rb.AddForce(velocityDelta * forceStrength, ForceMode.Acceleration);
        }
    }

    private void HandleMouseLook()
    {
        Vector2 lookDelta = lookAction.action.ReadValue<Vector2>() * mouseSensitivity;

        transform.Rotate(Vector3.up * lookDelta.x);

        cameraPitch = Mathf.Clamp(cameraPitch - lookDelta.y, -maxLookAngle, maxLookAngle);
        cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
    }



    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
