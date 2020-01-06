using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MinionController : EnemyController {
    public bool awake = false;
    public int damageAmount = 8;
    public float attackDuration = 0.5f;
    public float attackCoolDown = 1f;
    public float maxChaseDist = 20;
    public bool knowsPlayerPosition;

    private GameObject player;
    private NavMeshAgent agent;
    private Animator anim;
    private float interactRange = 3;
    private bool isAttacking = false;
    private float stunDuration = 0.5f;
    private float sightDistance = 30;

    void Start () {
        agent = GetComponent<NavMeshAgent> ();
        anim = GetComponentInChildren<Animator> ();
    }

    public override void Hurt (WeaponStatsController stats) {
        stunned = true;
        agent.isStopped = true;
        knowsPlayerPosition = true;
        float duration = stats.name == "melee" ? stunDuration * 2 : stunDuration;
        StartCoroutine (Stunned (duration));
        StartCoroutine (TakeDamageAfterDelay (stats));
    }

    private IEnumerator TakeDamageAfterDelay (WeaponStatsController stats) {
        yield return new WaitForSeconds (stats.splatterDelay);
        TakeDamage (stats.damage);
    }

    void Update () {
        if (player == null) {
            player = FindObjectOfType<WeaponController> ().gameObject;
        }

        if (awake && !dead && !stunned) {
            agent.SetDestination (player.transform.position);
            agent.isStopped = !knowsPlayerPosition;

            float playerDistance = Vector3.Distance (transform.position, player.transform.position);
            if (!knowsPlayerPosition) {
                knowsPlayerPosition = SearchForPlayer (playerDistance);
            }

            if (playerDistance <= 2) {
                agent.isStopped = true;
            }

            if (agent.isStopped) {
                anim.SetBool ("walking", false);
            } else {
                anim.SetBool ("walking", true);
            }

            RaycastHit hit;
            Vector3 rayPos = new Vector3 (transform.position.x, transform.position.y + 1, transform.position.z);
            if (Physics.Raycast (rayPos, transform.TransformDirection (Vector3.forward), out hit, interactRange)) {
                OpenDoor openDoor = hit.collider.GetComponent<OpenDoor> ();
                if (openDoor != null && !openDoor.open) {
                    openDoor.Interact ();
                }

                HealthController playerHealth = hit.collider.GetComponent<HealthController> ();
                if (playerHealth != null && !isAttacking) {
                    isAttacking = true;
                    StartCoroutine (Attack (playerHealth));
                }
            }

            // TODO stop chasing player if too far away?  
        }
    }

    private bool SearchForPlayer (float playerDistance) {
        if (playerDistance <= maxChaseDist) {
            if (player.GetComponent<WeaponController> ().makingNoise) {
                return true;
            }
            Vector3 targetDir = player.transform.position - transform.position;
            float angleToPlayer = Vector3.Angle (targetDir, transform.forward);
            if (angleToPlayer <= 85) {
                Vector3 rayPos = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
                RaycastHit hit;
                if (Physics.Linecast (transform.position, player.transform.position, out hit) && hit.collider.GetComponent<HealthController> () != null) {
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator Stunned (float duration) {
        yield return new WaitForSeconds (duration);
        stunned = false;
    }

    IEnumerator Attack (HealthController playerHealth) {
        yield return new WaitForSeconds (attackDuration);
        anim.SetBool ("attacking", true);
        if (!stunned && !dead) {
            playerHealth.TakeDamage (damageAmount);
            yield return new WaitForSeconds (attackCoolDown);
        }
        anim.SetBool ("attacking", false);
        isAttacking = false;
    }

    internal void WakeUp () {
        awake = true;
        knowsPlayerPosition = true;
    }

    public override void DeathEffects () {
        agent.isStopped = true;
        anim.SetBool ("dead", true);
        anim.SetBool ("walking", false);
        agent.updateRotation = false;
    }
}