using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visitor : MonoBehaviour
{
    public GameObject door;

    private UnityEngine.AI.NavMeshAgent agent;
    private Entrance entranceScript;

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
                Debug.Log("State : Waiting");
                break;
            case State.Waiting : 
                state = State.In_attraction;
                gameObject.SetActive(false);
                Debug.Log("State : In attraction");
                break;
            case State.In_attraction :
                state = State.Leaving;
                gameObject.SetActive(true);
                Debug.Log("State : Leaving");
                break;
            case State.Leaving :
                state = State.On_their_way;
                Debug.Log("State : On_their_Way"); 
                break;
        }
    }

    public void Set_position(Vector3 position)
    {
        transform.position = position;
    }

    private int poiIndex;

    private void Set_destination(){
        GameObject poiManagerObject = GameObject.Find("POIManager");
        if (poiManagerObject != null) {
            POIManager poiManager = poiManagerObject.GetComponent<POIManager>();
            if (poiManager != null) {
                List<Vector3> POIs_position = poiManager.POIs_position;
                if (POIs_position.Count > 0) {
                    poiIndex = new System.Random().Next(0, POIs_position.Count);
                    destination = POIs_position[poiIndex];
                    agent.SetDestination(destination);
                }
            }
        }
    }

    private void Go_to_waiting_queue()
    {
        GameObject poiManagerObject = GameObject.Find("POIManager");
        if (poiManagerObject != null) {
            POIManager poiManager = poiManagerObject.GetComponent<POIManager>();
            if (poiManager != null) {
                int index = poiIndex + 1;
                POI poi = poiManager.transform.Find("POI "+ index).GetComponent<POI>();
                if (poi != null)
                {
                    entranceScript = poi.GetComponentInChildren<Entrance>();

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
        }
    }

    private void Stopmoving()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.Stop();
    }

    double threshold = 1.0;

    // Start is called before the first frame update
    void Start()
    {
        GameObject poiManagerObject = GameObject.Find("POIManager");
        if (poiManagerObject != null) {
            POIManager poiManager = poiManagerObject.GetComponent<POIManager>();
            if (poiManager != null) {
                int index = poiIndex + 1;
                POI poi = poiManager.transform.Find("POI "+ index).GetComponent<POI>();
                if (poi != null)
                {
                    entranceScript = poi.GetComponentInChildren<Entrance>();
                }
            }
        }
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        transform.position = door.transform.position;
        transform.rotation = door.transform.rotation;
        Set_destination();
        state = State.On_their_way;
    }

    private float distanceBehindLastVisitor = 2f;

    // Update is called once per frame
    void Update()
    {
        if (state == State.Leaving){
            Set_destination();
            Update_state();
        }
        if (transform.position.x - destination.x < threshold &&
            transform.position.z - destination.z < threshold &&
            state == State.On_their_way)
        {
            Go_to_waiting_queue();
            Update_state();
        }
        if (state == State.Waiting)
        {
            Vector3 waiting_destination = entranceScript.Get_waiting_position() - (transform.forward * distanceBehindLastVisitor);
            agent.SetDestination(waiting_destination);
            if (transform.position.x - destination.x < distanceBehindLastVisitor &&
                transform.position.z - destination.z < distanceBehindLastVisitor)
            {
                Stopmoving();
            }
        }
    }
}
