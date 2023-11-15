using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI : MonoBehaviour
{
    public float visitDuration = 5.0f;
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

    // void Visitor_enter_the_attraction (Visitor visitor){
    //     if (visitor_in_attraction.Count < maxVisitors)
    //     {
    //         visitor_in_attraction.Add(visitor,0.0f);
    //         transform.Find("Entrance").GetComponent<Entrance>().Visitor_left_the_queue(visitor);
    //     }
    //     else
    //     {
    //         Debug.Log("Attraction is full. Cannot add more visitors.");
    //     }
    // }

    void Fill_the_attraction() {
        var Entrance = transform.Find("Entrance").GetComponent<Entrance>();

        List<Visitor> visitors_in_waiting_queue = new List<Visitor>(Entrance.Get_queue());
        // Vérifier si la capacité maximale d'attraction n'est pas encore atteinte.
        while (visitor_in_attraction.Count < maxVisitors && !Entrance.queue_is_empty())
        {
            foreach (Visitor visitor in visitors_in_waiting_queue)
            {
                // Vérifier si le visiteur n'est pas déjà dans l'attraction.
                if (!visitor_in_attraction.ContainsKey(visitor))
                {
                    visitor_in_attraction.Add(visitor, 0.0f);
                    Entrance.Visitor_left_the_queue(visitor);
                    visitor.Update_state();
                }
            }

            // Mettre à jour la liste des visiteurs en attente après chaque itération.
            visitors_in_waiting_queue = new List<Visitor>(Entrance.Get_queue());
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

    // void Update_visitor_list(){
        
    // }


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


