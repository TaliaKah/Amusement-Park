using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    public int numberOfVisitorsAtTheBeginning = 10;
    public GameObject visitorPrefab;
    private int numberOfVisitors = 1; // Car notre modèle est un visiteur aussi

    public void Create_visitor()
    {
        GameObject newVisitor = Instantiate(visitorPrefab, visitorPrefab.transform.position, Quaternion.identity);
        numberOfVisitors++;
        newVisitor.name = "Visitor " + numberOfVisitors;
        newVisitor.transform.parent = transform; 
    }

    public void Create_25_visitors()
    {
        for (int i = 0; i < 25; i++)
        {
            Create_visitor();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberOfVisitorsAtTheBeginning - 1; i++)
        {
            Create_visitor();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
