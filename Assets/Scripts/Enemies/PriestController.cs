using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PriestController : EnemyController {
    public bool awake = false;
    public int damageAmount = 8;
    public float attackDuration = 0.5f;
    public float attackCoolDown = 1f;
    public int weaponRange = 99;

    private GameObject player;
    private HealthController playerHealth;
    private NavMeshAgent agent;
    private Animator anim;
    private ChasePlayer chaser;
    private PatrolArea patrol;
    private bool isAttacking = false;
    private float stunDuration = 0.5f;

    void Start () {
        agent = GetComponent<NavMeshAgent> ();
        anim = GetComponentInChildren<Animator> ();
        chaser = GetComponent<ChasePlayer> ();
        patrol = GetComponent<PatrolArea> ();
    }

    public override void Hurt (WeaponStatsController stats) {
        stunned = true;
        agent.isStopped = true;
        chaser.knowsPlayerPosition = true;
        StartCoroutine (Stunned (stunDuration));
        StartCoroutine (TakeDamageAfterDelay (stats));
    }

    private IEnumerator TakeDamageAfterDelay (WeaponStatsController stats) {
        yield return new WaitForSeconds (stats.splatterDelay);
        int finalDamage = stats.name == "melee" ? stats.damage / 2 : stats.damage;
        TakeDamage (finalDamage);
    }

    void Update () {
        if (player == null) {
            player = FindObjectOfType<WeaponController> ().gameObject;
            playerHealth = FindObjectOfType<HealthController> ();
        }

        if (awake && !dead && !stunned && !isAttacking) {
            if (chaser.knowsPlayerPosition) {
                chaser.Chase ();
                if (IsAimingAtPlayer ()) {
                    StartCoroutine (Attack ());
                }
            } else {
                patrol.Patrol ();
                chaser.knowsPlayerPosition = chaser.SearchForPlayer ();
            }

            if (agent.isStopped) {
                anim.SetBool ("walking", false);
            } else {
                anim.SetBool ("walking", true);
            }
        }
    }

    private bool IsAimingAtPlayer () {
        Vector3 bulletPos = new Vector3 (transform.position.x, transform.position.y + 1, transform.position.z);
        Vector3 dir = player.transform.position - bulletPos;
        RaycastHit hit;
        if (Physics.Raycast (bulletPos, dir, out hit, weaponRange)) {
            return hit.collider.GetComponent<HealthController> () != null;
        }
        return false;
    }

    IEnumerator Stunned (float duration) {
        yield return new WaitForSeconds (duration);
        stunned = false;
    }

    IEnumerator Attack () {
        isAttacking = true;
        agent.isStopped = true;
        yield return new WaitForSeconds (attackDuration);

        anim.SetBool ("attacking", true);
        if (IsAimingAtPlayer ()) {
            playerHealth.TakeDamage (damageAmount);
        }

        yield return new WaitForSeconds (attackCoolDown);
        anim.SetBool ("attacking", false);
        isAttacking = false;
    }

    internal void WakeUp () {
        awake = true;
        chaser.knowsPlayerPosition = true;
    }

    public override void DeathEffects () {
        agent.isStopped = true;
        anim.SetBool ("dead", true);
        anim.SetBool ("walking", false);
        agent.updateRotation = false;
    }
}