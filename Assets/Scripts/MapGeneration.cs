using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public List<GameObject> corridors;
    public List<GameObject> turns;
    public List<GameObject> junctions;
    public List<GameObject> rooms;

    public GameObject startingCorridor;

    private List<GameObject> emptyConnectors = new List<GameObject>();

    private int totalObjects = 0;

    public int halfGridSize = 100;

    bool[,] occupied;

    public void Generate()
    {
        occupied = new bool[(halfGridSize*2) + 1, (halfGridSize * 2) + 1];
        GameObject startCorridor = Instantiate(startingCorridor, Vector3.zero, Quaternion.identity);
        occupied[halfGridSize, halfGridSize] = true;
        emptyConnectors.Add(startCorridor);

        while (emptyConnectors.Count > 0 && totalObjects < 500)
        {
            GameObject workingObject = emptyConnectors[0];
            emptyConnectors.RemoveAt(0);

            Transform connectors = workingObject.transform.Find("Connectors");
            if (connectors == null) continue;

            foreach (Transform connectorPoint in connectors)
            {
                SetupPrefab(connectorPoint, workingObject);
            }
        }
    }

    private void SetupPrefab(Transform connectorPoint, GameObject workingObject)
    {
        Vector3 location = workingObject.transform.position + connectorPoint.forward * 16f;

        var (xPos, zPos) = GetXZ(location);

        if (occupied[xPos, zPos])
        {
            return;
        }

        GameObject newObject;

        int randomNum = Random.Range(0, 99);

        if (randomNum < 75) // spawn corridor
        {
            newObject = Instantiate(corridors[Random.Range(0, corridors.Count)], Vector3.zero, Quaternion.identity);
        }
        else if (randomNum < 92) // spawn turn
        {
            newObject = Instantiate(turns[Random.Range(0, turns.Count)], Vector3.zero, Quaternion.identity);
        }
        else // spawn junction
        {
            newObject = Instantiate(junctions[Random.Range(0, junctions.Count)], Vector3.zero, Quaternion.identity);
        }

        Quaternion targetRotation = Quaternion.Euler(0, SnapAngle(workingObject.transform.eulerAngles.y + connectorPoint.transform.localEulerAngles.y), 0);
        newObject.transform.rotation = targetRotation;
        Transform storeDirections = newObject.transform.Find("Store direction");

        if (storeDirections != null)
        {
            bool hasSetUpShops = setUpShops(newObject, storeDirections, location);
            if (hasSetUpShops == false)
            {
                Destroy(newObject);
                SetupPrefab(connectorPoint, workingObject);
                return;
            }
        }

        newObject.transform.position = location;
        occupied[xPos, zPos] = true;

        emptyConnectors.Add(newObject);
        totalObjects++;    
    }

    bool setUpShops(GameObject newObject, Transform storeDirections, Vector3 location)
    {
        List<GameObject> currentShops = new List<GameObject>();
        foreach (Transform direction in storeDirections)
        {
            Vector3 facing = direction.forward;
            Vector3 shopLocation = location + facing * 16f;
            var (xPos, yPos) = GetXZ(shopLocation);

            if (!occupied[xPos, yPos])
            {
                GameObject newShop = Instantiate(rooms[Random.Range(0, rooms.Count)], shopLocation, Quaternion.identity);
                Transform shopFrontDirection = newShop.transform.Find("Shop front direction");
                newShop.transform.rotation = Quaternion.Euler(0, SnapAngle(direction.transform.localEulerAngles.y + shopFrontDirection.transform.localEulerAngles.y), 0);
                occupied[xPos, yPos] = true;
                currentShops.Add(newShop);
            }
            else
            {
                foreach (GameObject shop in currentShops)
                {
                    Destroy(shop);
                }
                return false;
            }
        }

        return true;
    }

    float SnapAngle(float angle)
    {
        return Mathf.Round(angle / 90f) * 90f;
    }

    private (int x, int z) GetXZ(Vector3 location)
    {
        int xPos = Mathf.RoundToInt(location.x / 16) + halfGridSize;
        int zPos = Mathf.RoundToInt(location.z / 16) + halfGridSize;
        return (xPos, zPos);
    }

}