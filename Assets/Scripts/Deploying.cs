using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deploying : MonoBehaviour
{
    private bool isDeploying;

    private GameObject prefab;
    private float placeRange = 5f;
    private Vector3 distanceInFront = new Vector3(0, 0, 2f);
    public GameObject player;
    public PlayerInventory playerInventory;

    private void Update()
    {
        if (isDeploying)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);
                Destroy(prefab);
                playerInventory.RemoveItem(playerInventory.currHotbar);
                isDeploying = false;
                Debug.Log("Changed to false");
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, placeRange))
            {
                prefab.transform.position = hit.point;
            }
        }
    }

    public void StartDeploy()
    {
        isDeploying = true;
        Debug.Log("Starting new deploy");
        prefab = playerInventory.GetHeldItem();
        prefab.GetComponent<Collider>().enabled = false;
        prefab.transform.position = player.transform.position + distanceInFront;
    }

    public bool GetIsDeploying()
    {
        return isDeploying;
    }
}