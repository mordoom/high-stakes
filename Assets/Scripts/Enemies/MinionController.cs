using UnityEngine;
using UnityEngine.AI;

public class MinionController : EnemyController {
    private bool awake = false;
    private GameObject player;
    private NavMeshAgent agent;

    void Start () {
        agent = GetComponent<NavMeshAgent> ();
    }

    void Update () {
        if (awake && !dead) {
            agent.destination = player.transform.position;
        }
    }

    internal void WakeUp () {
        awake = true;
        player = FindObjectOfType<WeaponController> ().gameObject;
    }

    public override void DeathEffects () {
        agent.isStopped = true;
    }
}