using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI : MonoBehaviour
{
    public float visitDuration = 5.0f; // en frame
    public int maxVisitors = 5;

    private Dictionary<Visitor,float> visitor_in_attraction = new Dictionary<Visitor,float>();

    private Entrance entrance;
    private Exit exit;

    void Start()
    {
        entrance = GetComponentInChildren<Entrance>();
        Debug.Log("entrance pos : "+ entrance.transform.position);
        exit = GetComponentInChildren<Exit>();
        Debug.Log("exit pos : "+ exit.transform.position);
    }

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

    void Fill_the_attraction(Entrance visitorEntrance)
    {        
        List<Visitor> visitorsToRemove = new List<Visitor>();
        List<Visitor> visitorsToUpdateState = new List<Visitor>();


        // Vérifier si la capacité maximale d'attraction n'est pas encore atteinte.
        if (visitor_in_attraction.Count < maxVisitors)
        {
            foreach (var visitor in visitorEntrance.Get_queue())
            {
                visitor_in_attraction.Add(visitor, 0.0f);
                visitorsToRemove.Add(visitor);
                visitorsToUpdateState.Add(visitor);
            }
            foreach (Visitor visitor in visitorsToRemove)
            {
                visitorEntrance.Visitor_left_the_queue(visitor);
            }
            foreach (Visitor visitor in visitorsToUpdateState)
            {
                visitor.Update_state();
                visitor.gameObject.SetActive(false);
            }
        }
    }

    void Visitor_left_the_attraction(Exit visitorExit){

        List<Visitor> visitorsToRemove = new List<Visitor>();
        List<Visitor> visitorsToUpdate = new List<Visitor>();

        List<Visitor> currentVisitors = new List<Visitor>(visitor_in_attraction.Keys);
        foreach (var key in currentVisitors)
        {
            visitor_in_attraction[key] += Time.deltaTime; // Increment the visit duration.

            if (visitor_in_attraction[key] >= visitDuration) // on rajoutera qu'il faudra que la sortie soit free peut-être
            {
                visitorsToRemove.Add(key);
                visitorsToUpdate.Add(key);
            }
        }
        foreach (Visitor visitor in visitorsToRemove)
        {
           visitor_in_attraction.Remove(visitor); 
        }
        foreach (Visitor visitor in visitorsToUpdate)
        {
            visitor.gameObject.SetActive(true);
            visitor.Set_position(visitorExit.transform.position);
            visitor.Update_state();            
        }
    }

    // Update is called once per frame
    void Update()
    {
        Visitor_left_the_attraction(exit);
        Fill_the_attraction(entrance);
    }
}


