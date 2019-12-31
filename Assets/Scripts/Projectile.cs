using UnityEngine;

public class Projectile : MonoBehaviour {
    public float speed = 5f;
    public int damageAmount = 10;

    void OnTriggerEnter (Collider other) {
        HealthController playerHealth = other.gameObject.GetComponent<HealthController> ();
        if (playerHealth != null) {
            playerHealth.TakeDamage (damageAmount);
        }

        Projectile projectile = other.gameObject.GetComponent<Projectile> ();
        EnemyController enemy = other.gameObject.GetComponent<EnemyController> ();
        if (enemy != null || projectile != null) {
            return;
        }

        // TODO fix this
        // Destroy (gameObject);
    }
}