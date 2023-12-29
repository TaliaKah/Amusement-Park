using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Visitor : MonoBehaviour
{
    public GameObject door;
    private NavMeshAgent agent;
    private Entrance entranceScript;
    private POIManager poiManager;

    private Vector3 destination;
    public State state;
    private Visitor target;
    private int poiIndex;

    public double threshold = 3.0;
    public float distanceBehindLastVisitor = 3f;
    public float wanderingProbability = 0.1f;
    public float wanderingRadius = 100f;
    public float wanderingUpdatePath = 2f;
    public float wanderingTime = 10f;

    private float intervalTimer = 0f;
    private float wanderingTimer = 0f;

    public enum State
    {
        InAttraction,
        Waiting,
        Leaving,
        OnTheirWay,
        Wandering,
    }

    public void UpdateState()
    {
        switch (state)
        {
            case State.OnTheirWay:
                state = State.Waiting;
                break;
            case State.Waiting:
                state = State.InAttraction;
                SetTargetToNull();
                gameObject.SetActive(false);
                break;
            case State.InAttraction:
                state = State.Leaving;
                gameObject.SetActive(true);
                break;
            case State.Leaving:
                state = (IsItTimeToWander()) ? State.Wandering : State.OnTheirWay;
                SetDestination();
                break;
            case State.Wandering:
                if (IsItTimeToStopWandering())
                {
                    state = State.OnTheirWay;
                    wanderingTimer = 0f;
                }
                SetDestination();
                break;
        }
    }

    public State GetState()
    {
        return state;
    }

    public bool IsItTimeToWander()
    {
        return wanderingProbability < Random.Range(0f, 1f);
    }

    public bool IsItTimeToStopWandering()
    {
        return wanderingTimer > wanderingTime;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetTargetToNull()
    {
        target = null;
    }

    private void SetDestination()
    {
        if (state == State.OnTheirWay)
        {
            SetDestinationToPOI();
        }
        if (state == State.Wandering)
        {
            SetRandomDestination();
        }
    }

    private void SetDestinationToPOI()
    {
        List<Vector3> POIsPosition = poiManager.POIsPosition;
        if (POIsPosition.Count > 0)
        {
            poiIndex = Random.Range(0, POIsPosition.Count);
            destination = POIsPosition[poiIndex];
            agent.SetDestination(destination);
        }
    }

    private void SetRandomDestination()
    {
        NavMeshHit hit;
        Vector3 randomPosition = transform.position + Random.insideUnitSphere * wanderingRadius;

        if (NavMesh.SamplePosition(randomPosition, out hit, wanderingRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void StopMoving()
    {
        agent.isStopped = true;
    }

    private void UpdateEntranceScript()
    {
        int index = poiIndex + 1;
        POI poi = poiManager.transform.Find("POI " + index).GetComponent<POI>();
        entranceScript = poi?.GetComponentInChildren<Entrance>();
    }

    private void SetDestinationToTheEndOfTheQueue()
    {
        UpdateEntranceScript();
        if (entranceScript != null)
        {
            destination = entranceScript.GetWaitingPositionAtTheEndOfTheQueue(distanceBehindLastVisitor);
            agent.SetDestination(destination);
        }
        else
        {
            Debug.LogError("Entrance script not found!");
        }
    }

    private void EnterWaitingQueue()
    {
        UpdateEntranceScript();
        if (entranceScript != null)
        {
            entranceScript.VisitorReachTheQueue(this);
        }
        else
        {
            Debug.LogError("Entrance script not found!");
        }
    }

    private bool IsItCloseEnough()
    {
        return Mathf.Abs(transform.position.x - destination.x) < threshold &&
                Mathf.Abs(transform.position.z - destination.z) < threshold;
    }

    private Vector3 WaitingDestination()
    {
        return (target == null) ?
                entranceScript.GetEntrancePosition() :
                target.transform.position - (transform.forward * distanceBehindLastVisitor);
    }

    private void Start()
    {
        poiManager = FindObjectOfType<POIManager>();

        if (poiManager == null)
        {
            Debug.LogError("POIManager not found!");
            return;
        }

        agent = GetComponent<NavMeshAgent>();
        
        transform.position = door.transform.position;
        transform.rotation = door.transform.rotation;
        state = State.Leaving;
        UpdateState();
    }

    private void Update()
    {
        if (state == State.Leaving)
        {
            UpdateState();
        }

        if (state == State.OnTheirWay)
        {
            SetDestinationToTheEndOfTheQueue();
            if (IsItCloseEnough())
            {
                target = entranceScript.GetLastVisitor();
                EnterWaitingQueue();
                UpdateState();
            }
        }
        if (state == State.Waiting)
        {
            if (target != null)
            {
                if (target.GetState() != State.Waiting)
                {
                    target = null;
                }
            }
            Vector3 waitingDestination = WaitingDestination();
            if (Vector3.Distance(transform.position, waitingDestination) > distanceBehindLastVisitor)
            {
                agent.isStopped = false;
                agent.SetDestination(waitingDestination);
            }
            else
            {
                StopMoving();
            }
        }

        if (state == State.Wandering)
        {
            intervalTimer += Time.deltaTime;
            wanderingTimer += Time.deltaTime;
            if ((!agent.pathPending && agent.remainingDistance < 0.1f) || intervalTimer >= wanderingUpdatePath)
            {
                UpdateState();
                intervalTimer = 0f;
            }
        }
    }
}