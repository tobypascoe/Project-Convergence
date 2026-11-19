using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deploying : MonoBehaviour
{
    private bool start = false; // enables the ghost
    private bool isDeploying = false; // enables placing down of item
    private bool toScale = false;

    private GameObject prefab;
    private float placeRange = 5f;
    private Vector3 distanceInFront = new Vector3(0, 0, 2f);
    public GameObject player;
    public PlayerInventory playerInventory;

    private void Update()
    {
        if (!start)
            return;

        if (!isDeploying)
        {
            isDeploying = true;
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0)
            );

        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit, placeRange);
        UpdatePosition(hit, hasHit);

        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);
            Destroy(prefab);
            playerInventory.RemoveItem(playerInventory.currHotbar);
            start = false;
            isDeploying = false;
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            start = false;
            isDeploying = false;
            Destroy(prefab);
            BackInHand();
        }
    }

    private void UpdatePosition(RaycastHit hit, bool hasHit)
    {
        if (prefab != null)
        {
            if (!hasHit)
            {
                BackInHand();
                toScale = false;
            }
            else
            {
                prefab.transform.position = hit.point;
                if (!toScale)
                {
                    prefab.transform.localScale /= 0.6f;
                    toScale = true;
                }
            }
            return;
        }
        if (hasHit)
        {
            InitPrefab();
        }
    }
    
    private void BackInHand() // puts the item back in hand if there placing is out of range
    {
        Destroy(prefab);
        playerInventory.ChangeActiveHotbar();
    }

    private void InitPrefab()
    {
        prefab = playerInventory.GetHeldItem();
        prefab.GetComponent<Collider>().enabled = false;
        prefab.transform.position = player.transform.position + distanceInFront;
    }

    public void StartDeploy()
    {
        start = true;
        InitPrefab();
    }

    public bool GetIsDeploying()
    {
        return isDeploying;
    }
}