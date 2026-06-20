using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public List<GameObject> corridors;     // DO NOT RUN YET INFINITE LOOP
    public List<GameObject> rooms;

    public GameObject startingCorridor;

    private List<GameObject> emptyConnectors = new List<GameObject>();

    private int totalObjects = 0;

    bool[,] occupied;

    public void Generate()
    {
        occupied = new bool[101, 101];
        GameObject startCorridor = Instantiate(startingCorridor, Vector3.zero, Quaternion.identity);
        occupied[50, 50] = true;
        emptyConnectors.Add(startCorridor);

        while (emptyConnectors.Count > 0 && totalObjects < 50)
        {
            GameObject workingObject = emptyConnectors[0];
            emptyConnectors.RemoveAt(0);

            Transform connectors = workingObject.transform.Find("Connectors");
            if (connectors == null) continue;

            foreach (Transform connectorPoint in connectors)
            {
                bool hasSetupPrefab = false;
                while (hasSetupPrefab = false)
                {
                    if (SetupPrefab(connectorPoint, workingObject))
                    {
                        hasSetupPrefab = true;
                    }
                }
            }
        }
    }

    private bool SetupPrefab(Transform connectorPoint, GameObject workingObject)
    {

        GameObject prefab = corridors[Random.Range(0, corridors.Count)];
        GameObject newObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);

        Quaternion targetRotation = Quaternion.Euler(0, SnapAngle(workingObject.transform.eulerAngles.y + connectorPoint.transform.localEulerAngles.y), 0);

        newObject.transform.rotation = targetRotation;
        Vector3 location = workingObject.transform.position + connectorPoint.forward * 16f;

        var (xPos, zPos) = GetXZ(location);

        if (occupied[xPos, zPos])
        {
            Destroy(newObject);
            // needs to return something
        }

        Transform storeDirections = newObject.transform.Find("Store direction");

        if (storeDirections != null)
        {
            bool hasSetUpShops = setUpShops(newObject, storeDirections, location);
            if (hasSetUpShops == false)
            {
                Destroy(newObject);
                return false;
            }
        }

        newObject.transform.position = location;
        occupied[xPos, zPos] = true;

        emptyConnectors.Add(newObject);
        totalObjects++;
        return true;        
    }

    bool setUpShops(GameObject newObject, Transform storeDirections, Vector3 location)
    {
        foreach (Transform direction in storeDirections)
        {
            Vector3 facing = direction.forward;
            Vector3 shopLocation = location + facing * 16f;
            var (xPos, yPos) = GetXZ(location);

            if (!occupied[xPos, yPos])
            {
                Instantiate(rooms[Random.Range(0, rooms.Count)], shopLocation, Quaternion.Euler(0, SnapAngle(newObject.transform.eulerAngles.y + 180f), 0));
                occupied[xPos, yPos] = true;
            }
            else
            {
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
        int xPos = Mathf.RoundToInt(location.x / 16) + 50;
        int zPos = Mathf.RoundToInt(location.z / 16) + 50;
        return (xPos, zPos);
    }

}