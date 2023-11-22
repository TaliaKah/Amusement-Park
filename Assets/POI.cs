using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI : MonoBehaviour
{
    public float visitDuration = 5.0f; // en frame
    public int maxVisitors = 5;

    private Dictionary<Visitor,float> visitor_in_attraction = new Dictionary<Visitor,float>();

    Vector3 Get_position(string Tag){
        var childTransform = transform.Find(Tag);
        if (childTransform != null)
        {
            return childTransform.position;
        }
        else 
        {    
            Debug.Log(Tag + " doesn't exist.");
            return Vector3.zero;
        }
    }

    public Vector3 Get_entrance_position(){
        return Get_position("Entrance");
    }

    public Vector3 Get_exit_position(){
        return Get_position("Exit");
    }

    void Fill_the_attraction() {
        var Entrance = transform.Find("Entrance").GetComponent<Entrance>();

        // Vérifier si la capacité maximale d'attraction n'est pas encore atteinte.
        if (visitor_in_attraction.Count < maxVisitors)
        {
            foreach (var visitor in Entrance.Get_queue())
            {
                Entrance.Visitor_left_the_queue(visitor);
                visitor_in_attraction.Add(visitor, 0.0f);
                visitor.Update_state();
                break;
            }
        }
    }

    void Visitor_left_the_attraction(){
        List<Visitor> visitorsToRemove = new List<Visitor>();
        List<Visitor> visitorsToUpdateState = new List<Visitor>();

        List<Visitor> currentVisitors = new List<Visitor>(visitor_in_attraction.Keys);

        foreach (var key in currentVisitors)
        {
            visitor_in_attraction[key] += Time.deltaTime; // Increment the visit duration.

            if (visitor_in_attraction[key] >= visitDuration) // on rajoutera qu'il faudra que la sortie soit free peut-être
            {
                visitorsToRemove.Add(key);
                visitorsToUpdateState.Add(key);
            }
        }
        foreach (Visitor visitor in visitorsToRemove)
        {
            visitor_in_attraction.Remove(visitor);
        }
        foreach (Visitor visitor in visitorsToUpdateState)
        {
            visitor.Update_state();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Visitor_left_the_attraction();
        Fill_the_attraction();
    }
}


