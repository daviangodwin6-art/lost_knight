using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public LayerMask whatisgrd;
    public float playerhigt;

    [Header("Settings")]
    public float speed;
    public float jumpforce;
    public float jumpmutilier;
    public float jumpcooldown;
    public float fallMultiplier;
    bool readyToJump = true;
    public float groundrag;
    bool grounded;
    private Vector3 moveDirection;
    private Vector2 moveInput;
    private Rigidbody rb;
    private InputAction moveAction;
    private InputAction jumpAction;

    private void Awake()
    {
        moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.AddBinding("<Gamepad>/leftStick");

        jumpAction = new InputAction("Jump", InputActionType.Button, binding: "<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (grounded)
        {
            rb.linearDamping = groundrag;
        }
        else
        {
            rb.linearDamping = 0;
        }

    }

    private void Update()
    {
        ReadInput();

        if (jumpAction.WasPressedThisFrame() && readyToJump && grounded)
        {
            readyToJump = false;
            jump();
            Invoke(nameof(resetjump), jumpcooldown);
        }
    }

    private void FixedUpdate()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerhigt * 0.5f + 0.2f, whatisgrd);
        rb.linearDamping = grounded ? groundrag : 0f;
        MovePlayer();
        controlspd();
        applyFallForce();
    }

    private void ReadInput()
    {
        moveInput = moveAction.ReadValue<Vector2>();
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;
        rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * speed * 10f ,ForceMode.Force);

        }
        else if(!grounded)
        {
            
        }
    }
    private void controlspd()
    {
        Vector3 flatvel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatvel.magnitude > speed)
        {
            Vector3 limitedVel = flatvel.normalized * speed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
    private void jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpforce * jumpmutilier, ForceMode.Impulse);
    }
    private void resetjump()
    {
        readyToJump = true; 
    }
    private void applyFallForce()
    {
        if (!grounded && rb.linearVelocity.y < 0f)
        {
            rb.AddForce(Vector3.down * fallMultiplier, ForceMode.Acceleration);
        }
    }
}