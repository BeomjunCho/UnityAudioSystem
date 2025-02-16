using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private float _walkSpeed; // Walk speed
    [SerializeField] private float _runSpeed; // Run speed
    [SerializeField] private float _jumpPower; // Jump power
    [SerializeField] private float _lookSpeed; // how fast player camera moves
    [SerializeField] private float _lookXLimit; // Up Down angle limit


    Vector3 moveDirection = Vector3.zero; // Direction for moving
    float rotationX = 0;

    public bool canMove; // Player only moves in true statement
    
    Rigidbody rb;

    void Start()
    {
        SetUp();
    }

    /// <summary>
    /// Set up rigid body, freeze rotation and can move statement
    /// </summary>
    /// <param name="inventory"></param>
    public void SetUp()
    {
        rb = GetComponent<Rigidbody>(); // Get player rigid body
        rb.freezeRotation = true;  // To avoid unwanted rotation
        canMove = true;
        // Hide and lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Player movement when it can move state
    /// </summary>
    void Update()
    {
        if (canMove)
        {
            // Get the forward and right directions relative to the player's transform
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            // Check if the player is running by holding the Left Shift key
            bool isRunning = Input.GetKey(KeyCode.LeftShift); // Running when left shift is pressed
            float speed = isRunning ? _runSpeed : _walkSpeed; // Set speed based on running state

            // Get movement input for forward/backward and left/right
            float moveForward = Input.GetAxis("Vertical") * speed; // Forward/backward movement input scaled by speed
            float moveSide = Input.GetAxis("Horizontal") * speed;  // Left/right movement input scaled by speed

            // Calculate the direction of movement based on input and the player's transform
            moveDirection = forward * moveForward + right * moveSide;

            // Update the Rigidbody's velocity, maintaining its current vertical velocity
            Vector3 velocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
            rb.linearVelocity = velocity;

            // Handle vertical camera rotation (looking up and down) with mouse input
            rotationX += -Input.GetAxis("Mouse Y") * _lookSpeed; // Adjust rotationX with inverted mouse Y input
            rotationX = Mathf.Clamp(rotationX, -_lookXLimit, _lookXLimit); // Clamp vertical rotation to prevent over-rotation
            _playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0); // Apply vertical rotation to the camera

            // Handle horizontal player rotation (turning left/right) with mouse input
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * _lookSpeed, 0);

            // Handle jumping input
            if (Input.GetButtonDown("Jump") && IsGrounded()) // Jump only if grounded and "Jump" button is pressed
            {
                rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse); // Apply upward force for jumping
            }
        }


    }


    // return true or false depending on player and ground distance
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    /// <summary>
    /// Camera look at target object in battle
    /// </summary>
    /// <param name="targetObject"></param>
    public void CameraLookAtObject(Transform targetObject)
    {
        _playerCamera.transform.LookAt(targetObject);
    }
}
