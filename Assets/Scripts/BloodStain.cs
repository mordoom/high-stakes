using System.Collections.Generic;
using UnityEngine;

public class BloodStain : MonoBehaviour {
    private ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;
    public GameObject bloodStain;

    void Start() {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other) {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        EnemyController enemy = other.GetComponent<EnemyController>();
        for (int i = 0; i < numCollisionEvents; i++) {
            if (enemy == null) {
                Vector3 hitPoint = collisionEvents[i].intersection;
                GameObject stain = Instantiate(bloodStain);
                stain.transform.position = hitPoint;
                stain.transform.rotation = Quaternion.FromToRotation(Vector3.up, collisionEvents[i].normal);
                stain.transform.parent = collisionEvents[i].colliderComponent.transform;
            }
        }
    }
}