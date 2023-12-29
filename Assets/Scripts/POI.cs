using System.Collections.Generic;
using UnityEngine;

public class POI : MonoBehaviour
{
    public float visitDuration = 5.0f;
    public int maxVisitors = 5;

    public float threshold = 6f;

    private Dictionary<Visitor, float> visitorsInAttraction = new Dictionary<Visitor, float>();
    private Entrance entrance;
    private Exit exit;

    private void Start()
    {
        entrance = GetComponentInChildren<Entrance>();
        exit = GetComponentInChildren<Exit>();
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
                visitor.SetTargetToNull();
                visitor.UpdateState();
            }
        }
    }


    private void AddAVisitorInAttraction(Entrance visitorEntrance)
    {
        if (visitorsInAttraction.Count < maxVisitors)
        {
            if (!visitorEntrance.QueueIsEmpty())
            {
                Visitor visitor = visitorEntrance.GetQueue()[0];
                if (Mathf.Abs(visitor.transform.position.x - visitorEntrance.GetEntrancePosition().x) < threshold &&
                    Mathf.Abs(visitor.transform.position.z - visitorEntrance.GetEntrancePosition().z) < threshold)
                {                
                    visitorsInAttraction.Add(visitor, 0.0f);
                    visitorEntrance.VisitorLeftTheQueue(visitor);
                    visitor.UpdateState();
                }
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
            visitorsInAttraction[key] += Time.deltaTime; // Incrémenter la durée de visite du visiteur

            if (visitorsInAttraction[key] >= visitDuration)
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

    private void Update()
    {
        VisitorsLeftTheAttraction(exit);
        AddAVisitorInAttraction(entrance);
    }
}
