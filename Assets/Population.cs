using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    public int numberOfVisitors = 10;
    public GameObject visitorPrefab;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberOfVisitors - 1; i++)
        {
            GameObject newVisitor = Instantiate(visitorPrefab, visitorPrefab.transform.position, Quaternion.identity);
            newVisitor.name = "Visitor " + i;
            newVisitor.transform.parent = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
