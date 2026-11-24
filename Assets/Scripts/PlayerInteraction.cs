using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Transform holdPoint;
    public float pickupRange = 3f;

    public GameObject crosshairImageDefault;
    public GameObject crosshairImagePickup;
    public PlayerInventory playerInventory;
    public Deploying deploying;

    private float heldX;
    private float heldY;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hit;
        WorldItem item = null;
        GameObject itemObject = null;
        float radius = 0.05f;
        if (Physics.SphereCast(ray, radius, out hit, pickupRange))
        {
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
                Destroy(itemObject);
                playerInventory.AddItem(item.data);
            }
        }

        // Consume item from hotbar
        if (Input.GetMouseButtonDown(0))
        {
            if (!deploying.GetIsDeploying())
            {
                if (playerInventory.currentHeldItem != null)
                {
                    playerInventory.Interaction();
                }
            }
        }
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
