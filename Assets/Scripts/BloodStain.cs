using System.Collections.Generic;
using UnityEngine;

public class BloodStain : MonoBehaviour {
    private ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;
    private BloodManager bloodManager;

    void Start() {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
        bloodManager = FindObjectOfType<BloodManager>();
    }

    void OnParticleCollision(GameObject other) {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        EnemyController enemy = other.GetComponent<EnemyController>();
        for (int i = 0; i < numCollisionEvents; i++) {
            if (enemy == null) {
                ParticleCollisionEvent collision = collisionEvents[i];
                bloodManager.CreateBloodStain(collision);
            }
        }
    }
}