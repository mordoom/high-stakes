using UnityEngine;

public class Projectile : MonoBehaviour {
    public float speed = 5f;
    public int damageAmount = 10;

    void OnTriggerEnter(Collider other) {
        HealthController playerHealth = other.gameObject.GetComponent<HealthController>();
        if (playerHealth != null) {
            playerHealth.TakeDamage(damageAmount);
        }

        EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
        if (enemy != null) {
            return;
        }

        // Destroy(gameObject);
    }
}