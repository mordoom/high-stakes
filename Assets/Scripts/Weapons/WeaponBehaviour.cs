using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponBehaviour : MonoBehaviour {
    public LayerMask layerMask;

    protected WeaponStatsController stats;
    protected BloodManager bloodManager;
    protected GameObject player;

    void Start () {
        stats = GetComponent<WeaponStatsController> ();
        bloodManager = FindObjectOfType<BloodManager> ();
        player = FindObjectOfType<HealthController> ().gameObject;
    }

    public virtual bool isShooting () {
        if (stats == null) {
            return false;
        }
        return stats.auto ? Input.GetMouseButton (0) : Input.GetButtonDown ("Fire1");
    }

    public virtual bool isShotAHit (int i, out RaycastHit hit) {
        float maxDeviation = stats.spreadDeviation * i;
        float xDeviation = Random.Range (-maxDeviation, maxDeviation);
        float yDeviation = Random.Range (-maxDeviation, maxDeviation);

        Vector3 bulletPos = new Vector3 (Input.mousePosition.x + xDeviation, Input.mousePosition.y + yDeviation, Input.mousePosition.z);
        Ray ray = Camera.main.ScreenPointToRay (bulletPos);
        return Physics.Raycast (ray, out hit, stats.range, layerMask);
    }

    public virtual void DamageEnemy (EnemyController enemy, RaycastHit hit) {
        if (!hit.collider.isTrigger && enemy != null && enemy.CanBeHurt (stats.name)) {
            StartCoroutine (HurtEnemy (enemy));
            Rigidbody rb = hit.collider.GetComponent<Rigidbody> ();
            if (rb != null) {
                Vector3 force = transform.forward * stats.bulletForce;
                rb.AddForce (force);
            }

            bloodManager.Splatter (hit, stats.splatterDelay);
        }
    }

    private IEnumerator HurtEnemy (EnemyController enemy) {
        yield return new WaitForSeconds (stats.splatterDelay);
        enemy.Hurt (stats.damage, stats.name);
    }
}