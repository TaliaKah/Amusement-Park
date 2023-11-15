using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : MonoBehaviour
{

    private List<Visitor> visitor_queue = new List<Visitor>();

    public List<Visitor> Get_queue()
    {
        return visitor_queue;
    }

    public void Visitor_reach_the_queue(Visitor visitor){
        visitor_queue.Add(visitor);
    }

    public void Visitor_left_the_queue(Visitor visitor){
        visitor_queue.Remove(visitor);
    }

    public bool queue_is_empty(){
        return (visitor_queue.Count == 0);
    }


}
