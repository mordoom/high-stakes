using UnityEngine;
using UnityEngine.AI;

public class ChasePlayer : MonoBehaviour {
    private NavMeshAgent agent;
    private GameObject player;

    public bool knowsPlayerPosition;
    public bool sniper = false;
    public float interactRange = 3;
    public float maxChaseDist = 20;

    void Start () {
        agent = GetComponent<NavMeshAgent> ();
    }

    void Update () {
        if (player == null) {
            player = FindObjectOfType<WeaponController> ().gameObject;
        }
    }

    public HealthController Chase () {
        if (sniper) {
            return null;
        }
        agent.SetDestination (player.transform.position);
        agent.isStopped = !knowsPlayerPosition;

        float playerDistance = GetPlayerDistance ();
        if (!knowsPlayerPosition) {
            knowsPlayerPosition = SearchForPlayer ();
        }

        if (playerDistance <= 2) {
            agent.isStopped = true;
        }

        RaycastHit hit;
        Vector3 rayPos = new Vector3 (transform.position.x, transform.position.y + 1, transform.position.z);
        if (Physics.Raycast (rayPos, transform.TransformDirection (Vector3.forward), out hit, interactRange)) {
            HealthController playerHealth = hit.collider.GetComponent<HealthController> ();
            return playerHealth;
        }

        return null;

        // TODO stop chasing player if too far away?  
    }

    private float GetPlayerDistance () {
        return Vector3.Distance (transform.position, player.transform.position);
    }

    public bool SearchForPlayer () {
        if (GetPlayerDistance () <= maxChaseDist) {
            if (player.GetComponent<WeaponController> ().makingNoise) {
                return true;
            }
            Vector3 targetDir = player.transform.position - transform.position;
            float angleToPlayer = Vector3.Angle (targetDir, transform.forward);
            if (angleToPlayer <= 85) {
                RaycastHit hit;
                Vector3 lineOfSight = new Vector3 (transform.position.x, transform.position.y + 2, transform.position.z);
                if (Physics.Linecast (lineOfSight, player.transform.position, out hit)) {
                    return hit.collider.GetComponent<HealthController> () != null;
                }
            }
        }
        return false;
    }
}