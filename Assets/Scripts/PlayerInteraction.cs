using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Transform holdPoint;
    public float pickupRange = 3f;
    public float throwForce = 5f;

    private GameObject heldObject;
    private Rigidbody heldRb;
    public float rotationSpeed = 0.01f;
    private Vector2 rotationInput;

    public GameObject crosshairImageDefault;
    public GameObject crosshairImagePickup;
    public PlayerInventory playerInventory;

    private float heldX;
    private float heldY;

    public Transform player;

    private Quaternion initialRotation;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hit;
        Rigidbody rb = null;
        WorldItem item = null;
        GameObject itemObject = null;
        float radius = 0.05f;
        if (Physics.SphereCast(ray, radius, out hit, pickupRange))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                rb = hit.collider.GetComponent<Rigidbody>();
                crosshairImageDefault.SetActive(false);
                crosshairImagePickup.SetActive(true);
            }
            if (hit.collider.CompareTag("Item"))
            {
                item = hit.collider.GetComponent<WorldItem>();
                itemObject = hit.collider.gameObject;
                crosshairImageDefault.SetActive(false);
                crosshairImagePickup.SetActive(true);
            }
            else
            {
                crosshairImageDefault.SetActive(true);
                crosshairImagePickup.SetActive(false);
            }
        }
        else
        {
            crosshairImageDefault.SetActive(true);
            crosshairImagePickup.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (itemObject != null)
            {
                int itemID = item.itemID;
                playerInventory.AddItem(itemID, itemObject);
            }
            if (heldObject == null)
            {  
                if (rb != null) TryPickup(rb);
            }
            else
            {
                DropObject();
            }
        }

        // Rotate held object
        if (heldObject != null)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                Vector3 currentRotation = heldObject.transform.localEulerAngles;
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                heldX = mouseX * rotationSpeed * Time.deltaTime;
                heldY = mouseY * rotationSpeed * Time.deltaTime;

                Quaternion rotY = Quaternion.AngleAxis(-heldX, player.up);
                Quaternion rotX = Quaternion.AngleAxis(heldY, player.right);

                heldObject.transform.rotation = rotY * rotX * heldObject.transform.rotation;
            }
        }

        // Consume item from hotbar
        if (Input.GetMouseButtonDown(0))
        {
            if (playerInventory.currentHeldItem != null)
            {
                playerInventory.Interact();
            }
        }
    }

    void TryPickup(Rigidbody rb)
    {
        if (rb != null)
        {
            heldObject = rb.gameObject;
            heldRb = rb;

            initialRotation = heldObject.transform.rotation;

            heldRb.useGravity = false;
            heldRb.velocity = Vector3.zero;
            heldRb.angularVelocity = Vector3.zero;

            heldObject.transform.SetParent(holdPoint);
            heldRb.isKinematic = true;
            heldRb.detectCollisions = false;
            heldObject.transform.position = holdPoint.position;

            heldObject.transform.rotation = initialRotation;
        }
    }

    void DropObject()
    {
        heldObject.transform.SetParent(null);

        heldRb.useGravity = true;
        heldRb.detectCollisions = true;
        heldRb.isKinematic = false;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float forceX = Mathf.Sqrt(mouseX * throwForce);
        float forceY = Mathf.Sqrt(mouseY * throwForce);
        forceX = Mathf.Clamp(mouseX * throwForce, -20f, 20f);
        forceY = Mathf.Clamp(mouseY * throwForce, -20f, 20f);

        heldRb.AddForce(new Vector3(forceX * 3 / 2, forceY, 0), ForceMode.VelocityChange);

        heldObject = null;
        heldRb = null;
        heldX = 0f;
        heldY = 0f;
    }

    void setCrosshairActive(bool defaultActive)
    {
        if (defaultActive == true)
        {
            crosshairImageDefault.SetActive(true);
            crosshairImagePickup.SetActive(false);
        }
        else
        {
            crosshairImageDefault.SetActive(false);
            crosshairImagePickup.SetActive(true);
        }
    }
}
