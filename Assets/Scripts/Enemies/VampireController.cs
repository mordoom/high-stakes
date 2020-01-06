using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class VampireController : EnemyController {
    public GameObject[] minions;
    public float standUpAngle = 90;
    public int damageAmount = 25;
    public float attackDuration = 0.5f;
    public float attackCoolDown = 1f;
    public GameObject projectile;
    public GameObject mistEffect;
    public float projectileSpeed = 10f;

    private WeaponController player;
    private HUDController hud;
    private NavMeshAgent agent;
    private float smooth = 2f;
    private float interactRange = 3;

    enum State {
        ASLEEP,
        CHASING,
        THROW_KNIFE,
        BITE,
        MIST
    }

    private State currentState = State.ASLEEP;
    private float knifeThrowCoolDownLength = 3f;
    private float knifeThrowCoolDown;
    private Vector3 regularScale;
    private float stunDuration = 2f;
    private float lineOfSight = 20;
    private float listenDistance = 10;
    private float mistDuration = 5;
    private bool misted;

    void Awake () {
        agent = GetComponent<NavMeshAgent> ();
        agent.isStopped = true;
        knifeThrowCoolDown = knifeThrowCoolDownLength;
        regularScale = transform.localScale;
    }

    public override void Hurt (WeaponStatsController stats) {
        if (stats.name == "melee") {
            TakeDamage (stats.damage);
            stunned = true;
            StartCoroutine (Stunned ());
        } else {
            TakeDamage (stats.damage / 10);
            CheckIfTimeToWakeUp ();
        }
    }

    IEnumerator Stunned () {
        yield return new WaitForSeconds (stunDuration);
        stunned = false;
    }

    void Update () {
        if (player == null) {
            player = FindObjectOfType<WeaponController> ();
        }

        ListenForPlayer ();

        if (dead || stunned) {
            agent.isStopped = true;
            return;
        }

        switch (currentState) {
            case State.CHASING:
                ChasePlayer ();
                break;
            case State.BITE:
                BitePlayer ();
                break;
            case State.THROW_KNIFE:
                ThrowAtPlayer ();
                break;
            case State.MIST:
                TurnToMist ();
                break;
            case State.ASLEEP:
            default:
                break;
        }
    }

    private void ListenForPlayer () {
        float dist = Vector3.Distance (player.transform.position, transform.position);
        if (dist < listenDistance) {
            if (hud == null) {
                hud = FindObjectOfType<HUDController> ();
            }
            hud.ShowVampireHealth (health);
            CheckIfTimeToWakeUp ();
        }
    }

    private void CheckIfTimeToWakeUp () {
        if (currentState == State.ASLEEP && player.makingNoise) {
            currentState = State.CHASING;
            WakeUpMinions ();
        }
    }

    private void ThrowAtPlayer () {
        agent.isStopped = true;
    }

    private void BitePlayer () {
        agent.isStopped = true;
    }

    private void ChasePlayer () {
        agent.SetDestination (player.transform.position);
        agent.isStopped = false;

        float playerDistance = Vector3.Distance (transform.position, player.transform.position);
        if (playerDistance <= 2) {
            agent.isStopped = true;
        }

        if (health <= 100 && !misted) {
            misted = true;
            currentState = State.MIST;
            GameObject newMistEffect = Instantiate (mistEffect, transform.position, transform.rotation);
            StartCoroutine (MistCooldown (newMistEffect));
        }

        RaycastHit hit;
        Vector3 rayPos = new Vector3 (transform.position.x, transform.position.y + 1, transform.position.z);
        if (Physics.Raycast (rayPos, transform.TransformDirection (Vector3.forward), out hit, interactRange)) {
            OpenDoor openDoor = hit.collider.GetComponent<OpenDoor> ();
            if (openDoor != null && !openDoor.open) {
                openDoor.Interact ();
            }

            HealthController playerHealth = hit.collider.GetComponent<HealthController> ();
            if (playerHealth != null) {
                Bite (playerHealth);
            }
        } else {
            if (knifeThrowCoolDown <= 0) {
                if (Physics.Raycast (rayPos, transform.TransformDirection (Vector3.forward), out hit, lineOfSight)) {
                    ThrowKnife ();
                }

            } else {
                knifeThrowCoolDown -= Time.deltaTime;
            }

        }
    }

    private void TurnToMist () {
        agent.isStopped = true;
        transform.localScale = new Vector3 (0, 0, 0);
    }

    IEnumerator MistCooldown (GameObject newMistEffect) {
        yield return new WaitForSeconds (mistDuration);
        transform.localScale = regularScale;
        currentState = State.CHASING;
        Destroy (newMistEffect);
        AppearBehindPlayer ();
    }

    private void AppearBehindPlayer () {
        transform.position = player.transform.position - player.transform.forward * 3;
        transform.LookAt (player.transform.position);
        stunned = true;
        StartCoroutine (Stunned ());
    }

    private void Bite (HealthController playerHealth) {
        currentState = State.BITE;
        StartCoroutine (BiteCooldown (playerHealth));
    }

    IEnumerator BiteCooldown (HealthController playerHealth) {
        yield return new WaitForSeconds (attackDuration);
        if (!stunned && !dead) {
            playerHealth.TakeDamage (damageAmount);
            yield return new WaitForSeconds (attackCoolDown);
        }
        currentState = State.CHASING;
    }

    private void ThrowKnife () {
        currentState = State.THROW_KNIFE;
        knifeThrowCoolDown = knifeThrowCoolDownLength;
        if (!stunned && !dead) {
            Vector3 newProjectilePos = new Vector3 (transform.position.x, transform.position.y + 1.5f, transform.position.z);
            GameObject newProjectile = Instantiate (projectile, newProjectilePos, transform.rotation);
            newProjectile.GetComponent<Rigidbody> ().AddForce (transform.forward * projectileSpeed);

            StartCoroutine (ThrowKnifeCoolDown ());
        }
    }

    IEnumerator ThrowKnifeCoolDown () {
        yield return new WaitForSeconds (attackCoolDown);
        currentState = State.CHASING;
    }

    public override void DeathEffects () {
        GameManager gameManager = FindObjectOfType<GameManager> ();
        gameManager.ReduceVampireCount ();
        WakeUpMinions ();
    }

    private void WakeUpMinions () {
        foreach (GameObject minion in minions) {
            OpenCoffin openCoffin = minion.GetComponent<OpenCoffin> ();
            if (openCoffin != null) {
                minion.GetComponent<OpenCoffin> ().OnOpen ();
            }
        }
    }
}