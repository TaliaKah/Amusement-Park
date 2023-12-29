using System.Collections.Generic;
using UnityEngine;

public class POI : MonoBehaviour
{
    public float visitDuration = 5.0f; // en frames
    public int maxVisitors = 5;

    private Dictionary<Visitor, float> visitorsInAttraction = new Dictionary<Visitor, float>();
    private Entrance entrance;
    private Exit exit;

    private void Start()
    {
        entrance = GetComponentInChildren<Entrance>();
        Debug.Log("Entrance pos: " + entrance.transform.position);
        exit = GetComponentInChildren<Exit>();
        Debug.Log("Exit pos: " + exit.transform.position);
    }

    public Vector3 GetPosition(string tag)
    {
        var childTransform = transform.Find(tag);
        if (childTransform != null)
        {
            return childTransform.position;
        }
        else
        {
            Debug.Log(tag + " doesn't exist.");
            return Vector3.zero;
        }
    }

    public Vector3 GetEntrancePosition()
    {
        return GetPosition("Entrance");
    }

    public Vector3 GetExitPosition()
    {
        return GetPosition("Exit");
    }

    private void FillTheAttraction(Entrance visitorEntrance)
    {
        List<Visitor> visitorsToRemove = new List<Visitor>();
        List<Visitor> visitorsToUpdateState = new List<Visitor>();

        // Vérifier si la capacité maximale d'attraction n'est pas encore atteinte.
        if (visitorsInAttraction.Count < maxVisitors)
        {
            foreach (var visitor in visitorEntrance.GetQueue())
            {
                if (visitorsInAttraction.Count >= maxVisitors)
                {
                    visitor.SetTargetToNull();
                    break;
                }
                visitorsInAttraction.Add(visitor, 0.0f);
                visitorsToRemove.Add(visitor);
                visitorsToUpdateState.Add(visitor);
            }
            foreach (Visitor visitor in visitorsToRemove)
            {
                visitorEntrance.VisitorLeftTheQueue(visitor);
            }
            foreach (Visitor visitor in visitorsToUpdateState)
            {
                visitor.UpdateState();
            }
        }
    }

    private void VisitorsLeftTheAttraction(Exit visitorExit)
    {
        List<Visitor> visitorsToRemove = new List<Visitor>();
        List<Visitor> visitorsToUpdate = new List<Visitor>();

        List<Visitor> currentVisitors = new List<Visitor>(visitorsInAttraction.Keys);
        foreach (var key in currentVisitors)
        {
            visitorsInAttraction[key] += Time.deltaTime; // Increment the visit duration.

            if (visitorsInAttraction[key] >= visitDuration) // On ajoutera qu'il faudra que la sortie soit libre peut-être
            {
                visitorsToRemove.Add(key);
                visitorsToUpdate.Add(key);
            }
        }
        foreach (Visitor visitor in visitorsToRemove)
        {
            visitorsInAttraction.Remove(visitor);
        }
        foreach (Visitor visitor in visitorsToUpdate)
        {
            visitor.SetPosition(visitorExit.transform.position);
            visitor.UpdateState();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        VisitorsLeftTheAttraction(exit);
        FillTheAttraction(entrance);
    }
}
