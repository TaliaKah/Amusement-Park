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

    public Vector3 Get_entrance_position()
    {
        return transform.position;
    }

    public Vector3 Get_waiting_position_at_the_end_of_the_file(float distanceBehindLastVisitor)
    {
        return (queue_is_empty()) ? transform.position : Get_last_visitor().transform.position - (Get_last_visitor().transform.forward * distanceBehindLastVisitor);
    }

    public Visitor Get_last_visitor()
    {
        return (queue_is_empty()) ? null : visitor_queue[visitor_queue.Count - 1];
    }

}
