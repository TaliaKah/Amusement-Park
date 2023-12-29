using System.Collections.Generic;
using UnityEngine;

public class Entrance : MonoBehaviour
{
    private List<Visitor> visitorQueue = new List<Visitor>();

    public List<Visitor> GetQueue()
    {
        return visitorQueue;
    }

    public void VisitorReachTheQueue(Visitor visitor)
    {
        visitorQueue.Add(visitor);
    }

    public void VisitorLeftTheQueue(Visitor visitor)
    {
        visitorQueue.Remove(visitor);
    }

    public bool QueueIsEmpty()
    {
        return (visitorQueue.Count == 0);
    }

    public Vector3 GetEntrancePosition()
    {
        return transform.position;
    }

    public Vector3 GetWaitingPositionAtTheEndOfTheQueue(float distanceBehindLastVisitor)
    {
        return (QueueIsEmpty()) ? transform.position : GetLastVisitor().transform.position - (GetLastVisitor().transform.forward * distanceBehindLastVisitor);
    }

    public Visitor GetLastVisitor()
    {
        return (QueueIsEmpty()) ? null : visitorQueue[visitorQueue.Count - 1];
    }
}
