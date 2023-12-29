using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    public int numberOfVisitorsAtTheBeginning = 10;
    public GameObject visitorPrefab;
    public GameObject door;

    private int numberOfVisitors = 0;

    public void CreateVisitor()
    {
        visitorPrefab.SetActive(true);
        GameObject newVisitor = Instantiate(visitorPrefab, visitorPrefab.transform.position, Quaternion.identity);
        newVisitor.transform.position = door.transform.position;
        newVisitor.transform.rotation = door.transform.rotation;
        numberOfVisitors++;
        newVisitor.name = "Visitor " + numberOfVisitors;
        newVisitor.transform.parent = transform;
        visitorPrefab.SetActive(false);
    }

    public void Create25Visitors()
    {
        for (int i = 0; i < 25; i++)
        {
            CreateVisitor();
        }
    }

    void Start()
    {
        for (int i = 0; i < numberOfVisitorsAtTheBeginning - 1; i++)
        {
            CreateVisitor();
        }
    }
}
