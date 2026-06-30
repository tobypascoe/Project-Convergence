using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public List<GameObject> corridors;
    public List<GameObject> rightTurns;
    public List<GameObject> leftTurns;
    public List<GameObject> junctions;
    public List<GameObject> rooms;

    public GameObject startingCorridor;
    public GameObject deadEndCorridor;

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

        while (emptyConnectors.Count > 0 && totalObjects < 5000)
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

        List<Vector3Int> occupiedNeighbours = new List<Vector3Int>();

        if (occupied[xPos, zPos + 1]) occupiedNeighbours.Add(new Vector3Int(0, 0, 1));

        if (occupied[xPos, zPos - 1]) occupiedNeighbours.Add(new Vector3Int(0, 0, -1));

        if (occupied[xPos + 1, zPos]) occupiedNeighbours.Add(new Vector3Int(1, 0, 0));
        
        if (occupied[xPos - 1, zPos]) occupiedNeighbours.Add(new Vector3Int(-1, 0, 0));

        Vector3 dir = -connectorPoint.forward;
        Vector3Int conPointNeg = new Vector3Int(Mathf.RoundToInt(dir.x), 0, Mathf.RoundToInt(dir.z));
        occupiedNeighbours.Remove(conPointNeg);
        

        bool isValidPrefab = false;
        GameObject newObject = null;

        Quaternion targetRotation = Quaternion.Euler(0, SnapAngle(workingObject.transform.eulerAngles.y + connectorPoint.transform.localEulerAngles.y), 0);

        while (!isValidPrefab)
        {
            if (newObject != null)
            {
                GameObject.Destroy(newObject);
            }

            if (occupiedNeighbours.Count >= 3)
            {
                Debug.Log(string.Join(", ", occupiedNeighbours));
                Debug.Log("-connectorPoint.forward = " + -connectorPoint.forward);
                newObject = Instantiate(deadEndCorridor, location, targetRotation);
                isValidPrefab = true;
                break;
            }

            int randomNum = Random.Range(0, 100);

            if (randomNum < 75) // spawn corridor
            {
                newObject = Instantiate(corridors[Random.Range(0, corridors.Count)], location, targetRotation);
                isValidPrefab = isDeadEnd(newObject, occupiedNeighbours);
            }
            else if (randomNum < 92) // spawn turn
            {
                if (Random.Range(0, 2) == 0)
                {
                    newObject = Instantiate(rightTurns[Random.Range(0, rightTurns.Count)], location, targetRotation);
                    isValidPrefab = isDeadEnd(newObject, occupiedNeighbours);
                }
                else
                {
                    newObject = Instantiate(leftTurns[Random.Range(0, leftTurns.Count)], location, targetRotation);
                    isValidPrefab = isDeadEnd(newObject, occupiedNeighbours);
                }
            }
            else // spawn junction
            {
                newObject = Instantiate(junctions[Random.Range(0, junctions.Count)], location, targetRotation);
                isValidPrefab = isDeadEnd(newObject, occupiedNeighbours);
            }
        }


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

        
        occupied[xPos, zPos] = true;

        emptyConnectors.Add(newObject);
        totalObjects++;    
    }

    bool isDeadEnd(GameObject newObject, List<Vector3Int> occupiedNeighbours)
    {
        Transform connectorPoints = newObject.transform.Find("Connectors");
        foreach (Transform connector in connectorPoints)
        {
            Vector3 dir = connector.forward;
            Vector3Int gridDir = new Vector3Int(Mathf.RoundToInt(dir.x), 0, Mathf.RoundToInt(dir.z));

            if (occupiedNeighbours.Contains(gridDir))
            {
                return false;
            }
        }
        return true;
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
                newShop.transform.rotation = Quaternion.Euler(0, SnapAngle(newObject.transform.eulerAngles.y + direction.transform.localEulerAngles.y), 0);
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