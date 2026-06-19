using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public List<GameObject> corridors;
    public List<GameObject> rooms;

    private List<GameObject> emptyConnectors = new List<GameObject>();

    bool[,] occupied;

    public void Generate()
    {
        occupied = new bool[101, 101];
        GameObject startCorridor = Instantiate(corridors[0], Vector3.zero, Quaternion.identity);
        occupied[50, 50] = true;
        emptyConnectors.Add(startCorridor);

        while (emptyConnectors.Count > 0)
        {
            GameObject workingObject = emptyConnectors[0];
            emptyConnectors.RemoveAt(0);

            Transform connectors = workingObject.transform.Find("Connectors");
            if (connectors == null) continue;

            GameObject prefab = corridors[Random.Range(0, corridors.Count)];
            GameObject newObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);

            foreach (Transform connectorPoint in connectors)
            {
                if (connectorPoint.transform.position.x > 0) // connector point at right of cell
                {
                    float currXpos = workingObject.transform.position.x;
                    newObject.transform.position = new Vector3(currXpos + 16f, 0f, workingObject.transform.position.z);
                    newObject.transform.rotation = workingObject.transform.rotation * connectorPoint.transform.rotation;
                }
                else if (connectorPoint.transform.position.x < 0) // connector point at left of cell
                {
                    float currXpos = workingObject.transform.position.x;
                    newObject.transform.position = new Vector3(currXpos - 16f, 0f, workingObject.transform.position.z);
                    newObject.transform.rotation = workingObject.transform.rotation * connectorPoint.transform.rotation;
                }
                else if (connectorPoint.transform.position.z > 0) // connector point at top of cell
                {
                    float currZpos = workingObject.transform.position.z;
                    newObject.transform.position = new Vector3(workingObject.transform.position.x, 0f, currZpos + 16f);
                    newObject.transform.rotation = workingObject.transform.rotation * connectorPoint.transform.rotation;
                }
                else // connector point at bottom of cell
                {
                    float currZpos = workingObject.transform.position.z;
                    newObject.transform.position = new Vector3(workingObject.transform.position.x, 0f, currZpos - 16f);
                    newObject.transform.rotation = workingObject.transform.rotation * connectorPoint.transform.rotation;
                }

                emptyConnectors.Add(newObject);
            }
        }
    }

}