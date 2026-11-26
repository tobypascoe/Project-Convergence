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

    public PlayerInventory playerInventory;
    private CharacterController controller;

    public CinemachineCamera cinemachineCam;
    public CinemachineInputAxisController inputProvider;

    private Vector3 currAirVelocity = Vector3.zero;

    private float maxAirSpeed = 0.6f;
    private bool inInventory = false;

    public float stamina = 100f;
    public float staminaLossRate = 5f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
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

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        if (controller.isGrounded)
        {
            if (currAirVelocity.y < 0f)
            {
                currAirVelocity.y = -2f;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                currAirVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            currAirVelocity.y += gravity * Time.deltaTime;
        }

        Vector3 velocity = move * speed + currAirVelocity;

        controller.Move(velocity * Time.deltaTime);

        if (cameraTarget != null && !playerInventory.isInventoryOpen)
        {
            float targetYaw = cameraTarget.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetYaw, 0f);
            inputProvider.enabled = true;

        }
        else
            inputProvider.enabled = false;
    }
}