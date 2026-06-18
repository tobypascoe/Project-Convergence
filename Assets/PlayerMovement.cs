using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Camera")]
    public Transform cameraTarget; // Cinemachine camera on player

    [Header("Rotation")]
    public float rotationSmoothTime = 0.02f;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 airVelocity;
    private float maxAirSpeed = 0.6f;

    public CinemachineCamera cinemachineCam;

    public bool isInspecting = false;
    public CinemachineInputAxisController inputProvider;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        isInspecting = Input.GetKey(KeyCode.LeftControl);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 7.5f;
            cinemachineCam.Lens.FieldOfView = 65f;
        }
        else
        {
            speed = 5f;
            cinemachineCam.Lens.FieldOfView = 60f;
        }

        // --- Movement input ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        if (controller.isGrounded)
        {
            controller.Move(move * speed * Time.deltaTime);
        }
        else
        {
            airVelocity = Vector3.ClampMagnitude(airVelocity, maxAirSpeed);
            controller.Move((airVelocity + (move / 3)) * speed * Time.deltaTime);
        }
 
        // --- Gravity ---
        velocity.y += gravity * Time.deltaTime;

        // --- Move vertical ---
        controller.Move(velocity * Time.deltaTime);
        if ((controller.collisionFlags & CollisionFlags.Above) != 0 && velocity.y > 0)
        {
            velocity.y = 0f;
        }

        // --- Ground check AFTER move ---
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f; // important (not 0)
        }

        // --- Jump ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            airVelocity = transform.right * x + transform.forward * z;
        }

        // --- Rotate player ---
        if (cameraTarget != null && !isInspecting)
        {
            float targetYaw = cameraTarget.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetYaw, 0f);
            inputProvider.enabled = true;

        }
        else
            inputProvider.enabled = false;
    }
}