using System;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour {
    public bool dead = false;
    public bool stunned = false;
    public int health = 100;

    public virtual bool CanBeHurt (string weaponName) {
        return !dead;
    }

    public virtual void Hurt (int damage, string weaponName) {
        TakeDamage (damage);
    }

    public void TakeDamage (int damage) {
        health -= damage;

        if (health <= 0) {
            health = 0;
            Die ();
        }
    }

    internal void TakeExplosiveDamage (int damage) {
        health -= damage;

        if (health <= 0) {
            health = 0;
            Explode ();
        }
    }

    public void Stun () {
        stunned = true;
    }

    public void Die () {
        dead = true;
        Destroy (GetComponent<Collider> (), 1);
        DeathEffects ();
    }

    public void Explode () {
        Die();
        FindObjectOfType<BloodManager>().Explode(gameObject);
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public virtual void DeathEffects () { }
}