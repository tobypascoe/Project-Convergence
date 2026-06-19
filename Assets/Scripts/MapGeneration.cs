using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public List<GameObject> corridors;
    public List<GameObject> rooms;

    private List<GameObject> emptyConnectors = new List<GameObject>();

    private int totalObjects = 0;

    public void Generate()
    {
        GameObject startCorridor = Instantiate(corridors[0], Vector3.zero, Quaternion.identity);
        emptyConnectors.Add(startCorridor);

        while (emptyConnectors.Count > 0 && totalObjects < 50)
        {
            GameObject workingObject = emptyConnectors[0];
            emptyConnectors.RemoveAt(0);

            Transform connectors = workingObject.transform.Find("Connectors");
            if (connectors == null) continue;

            foreach (Transform connectorPoint in connectors)
            {
                GameObject prefab = corridors[Random.Range(0, corridors.Count)];
                GameObject newObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                Transform connectionPoint = newObject.transform.Find("Connection point");

                newObject.transform.rotation = connectorPoint.rotation * Quaternion.Inverse(connectionPoint.rotation);

                newObject.transform.position = connectorPoint.position - (connectionPoint.position - newObject.transform.position);

                emptyConnectors.Add(newObject);
                totalObjects++;
            }
        }
    }
}