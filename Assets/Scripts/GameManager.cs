using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MapGeneration mapGeneration;
    // Start is called before the first frame update
    void Start()
    {
        mapGeneration.Generate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
