using UnityEngine;
using UnityEngine.AI;

public class PatrolArea : MonoBehaviour {
    public Transform[] route;
    public float patrolPauseLength = 3;

    private int destPoint = 0;
    private float currentPatrolPauseTime = 0;
    private NavMeshAgent agent;
    private bool waiting = false;

    void Start () {
        agent = GetComponent<NavMeshAgent> ();
        agent.autoBraking = false;
        GotoNextPoint ();
    }

    void GotoNextPoint () {
        if (route.Length == 0) {
            return;
        }
        agent.isStopped = false;
        agent.destination = route[destPoint].position;
        destPoint = (destPoint + 1) % route.Length;
    }

    public void Patrol () {
        if (waiting) {
            agent.isStopped = true;
            currentPatrolPauseTime += Time.deltaTime;

            if (currentPatrolPauseTime >= patrolPauseLength) {
                waiting = false;
                currentPatrolPauseTime = 0;
                GotoNextPoint ();
            }
        }
        if (!agent.pathPending && agent.remainingDistance < 0.5f) {
            waiting = true;
        }
    }
}