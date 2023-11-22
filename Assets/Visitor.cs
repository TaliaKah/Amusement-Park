using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visitor : MonoBehaviour
{
    public GameObject door;

    private UnityEngine.AI.NavMeshAgent agent;

    private Vector3 destination;

    enum State{
        In_attraction,
        Waiting,
        Leaving,
        On_their_way,
    }

    private State state;

    public void Update_state()
    {
        switch(state)
        {
            case State.On_their_way :
                state = State.Waiting;
                break;
            case State.Waiting : 
                state = State.In_attraction;
                break;
            case State.In_attraction :
                state = State.Leaving;
                break;
            case State.Leaving :
                state = State.On_their_way;
                break;
        }
    }

    private void Set_destination(){
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        GameObject poiManagerObject = GameObject.Find("POIManager");
        if (poiManagerObject != null) {
            POIManager poiManager = poiManagerObject.GetComponent<POIManager>();
            if (poiManager != null) {
                List<Vector3> POIs_position = poiManager.POIs_position;
                if (POIs_position.Count > 0) {
                    int indexAleatoire = new System.Random().Next(0, POIs_position.Count);
                    destination = POIs_position[indexAleatoire];
                    agent.SetDestination(destination);
                }
            }
        }
    }

    private void Go_to_waiting_queue()
    {
        POIManager poiManager = FindObjectOfType<POIManager>();

        if (poiManager != null)
        {
            Entrance entranceScript = poiManager.GetComponentInChildren<Entrance>();
            if (entranceScript != null)
            {
                entranceScript.Visitor_reach_the_queue(this);
                Debug.Log("Visitor in waiting queue");
            }
            else
            {
                Debug.LogError("Entrance script not found!");
            }
        }
    }

    double threshold = 1.0;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = door.transform.position;
        transform.rotation = door.transform.rotation;
        Set_destination();
        state = State.On_their_way;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Leaving){
            Set_destination();
            Debug.Log("State : On_their_Way");
            Update_state();
        }
        if (transform.position.x - destination.x < threshold &&
            transform.position.z - destination.z < threshold &&
            state == State.On_their_way)
        {
            Go_to_waiting_queue();
            Debug.Log("State : Waiting");
            Update_state();
        }
        // if (state == State.Waiting){
        //     // si place dans l'attraction s'y mettre
        //     Debug.Log("State : In_attraction");
        //     state = State.In_attraction;
            // se placer derrière le visiteur d'avant dans la file
        // }
        // Test
        // if (state == State.In_attraction)
        // {
        //     // if () temps du visiteur a dépassé celui prévu de l'attraction le faire sortir
        //     Debug.Log("State : Leaving");
        //     state = State.Leaving;
        // faire disparaitre le perso dans l'attraction et quand il part le faire réaparaître à la sortie
        // }
    }

        // visiteur se place dans la filequand il arrive près de la fin de la file
    // visiteur avec GameObject.Find() va trouver mes POIs qui seront gerer par un managerPOI où j'aurais le nombre de POI pour faire une boucle dessus
// le visiteur regarde si entr�e libre sinon se met dans la file, pour la sortie doit se casser pour laisser la place aux autres
// la sortie doit dire si elle est libre
}
