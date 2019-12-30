using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MinionController : EnemyController {
    public bool awake = false;
    public int damageAmount = 8;
    public float attackDuration = 0.5f;
    public float attackCoolDown = 1f;

    private GameObject player;
    private NavMeshAgent agent;
    private Animator anim;
    private float interactRange = 3;
    private bool isAttacking = false;
    private float stunDuration = 1f;

    void Start () {
        agent = GetComponent<NavMeshAgent> ();
        anim = GetComponentInChildren<Animator> ();
    }

    public override void Hurt (int damage, string weaponName) {
        if (weaponName == "melee") {
            stunned = true;
            StartCoroutine (Stunned ());
        }
        TakeDamage (damage);
    }

    void Update () {
        if (player == null) {
            player = FindObjectOfType<WeaponController> ().gameObject;
        }

        if (awake && !dead) {
            if (agent.isStopped) {
                anim.SetBool ("walking", false);
            } else {
                anim.SetBool ("walking", true);
            }
            agent.destination = player.transform.position;
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

            // TODO stop moving if within certain distance of player
            // TODO stop chasing player if too far away?  
        }
    }

    IEnumerator Stunned () {
        yield return new WaitForSeconds (stunDuration);
        stunned = false;
    }

    IEnumerator Attack (HealthController playerHealth) {
        yield return new WaitForSeconds (attackDuration);
        if (!stunned && !dead) {
            playerHealth.TakeDamage (damageAmount);
            yield return new WaitForSeconds (attackCoolDown);
        }
        isAttacking = false;
        // TODO projectiles
        // if (!dead) {
        //     Vector3 newProjectilePos = new Vector3 (transform.position.x, transform.position.y + 1, transform.position.z);
        //     GameObject newProjectile = Instantiate (projectile, newProjectilePos, transform.rotation);
        //     newProjectile.GetComponent<Rigidbody> ().velocity = (player.transform.position - transform.position).normalized * projectileSpeed;
        // }
    }

    internal void WakeUp () {
        awake = true;
    }

    public override void DeathEffects () {
        agent.isStopped = true;
        anim.SetBool ("walking", false);
    }
}