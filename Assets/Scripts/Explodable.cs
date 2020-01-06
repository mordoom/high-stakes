using UnityEngine;

public class Explodable : MonoBehaviour, Shootable {
    public float timeToExplode;
    public GameObject explosionEffect;
    public float blastRadius = 5;
    public bool explodeAfterDuration = true;
    public bool explodeOnCollision = false;
    public bool explodeWhenShot = false;

    private float blastForce = 400;
    private float blastDuration = 0.9f;
    private int damage = 300;
    private bool hasExploded = false;

    void Update () {
        if (!explodeAfterDuration) {
            return;
        } else if (timeToExplode <= 0) {
            Explode ();
        } else {
            timeToExplode -= Time.deltaTime;
        }
    }

    void OnCollisionEnter (Collision collision) {
        if (explodeOnCollision) {
            Explode ();
        }
    }

    public void Explode () {
        if (hasExploded) {
            return;
        }

        hasExploded = true;
        GetComponentInChildren<MeshRenderer> ().enabled = false;

        GameObject explosion = Instantiate (explosionEffect, transform.position, transform.rotation);
        Destroy (explosion, blastDuration);
        Destroy (gameObject, blastDuration);

        Collider[] nearbyObjects = Physics.OverlapSphere (transform.position, blastRadius);
        foreach (Collider nearbyObject in nearbyObjects) {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody> ();
            if (rb != null) {
                rb.AddExplosionForce (blastForce, transform.position, blastRadius);
            }

            float distance = Vector3.Distance (nearbyObject.gameObject.transform.position, transform.position);
            int calcDamage = Mathf.CeilToInt ((1 / distance) * damage);
            EnemyController enemyController = nearbyObject.GetComponent<EnemyController> ();
            if (enemyController != null) {
                enemyController.TakeExplosiveDamage (calcDamage);
            }
            HealthController player = nearbyObject.GetComponent<HealthController> ();
            if (player != null) {
                player.TakeDamage (calcDamage);
            }
            Explodable explodable = nearbyObject.GetComponent<Explodable> ();
            if (explodable != null) {
                explodable.Explode ();
            }
        }
    }

    public void wasShotBy (WeaponStatsController stats) {
        if (explodeWhenShot) {
            Explode ();
        }
    }
}