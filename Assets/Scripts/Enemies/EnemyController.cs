using System;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour {
    public bool dead = false;
    public int health = 100;

    public virtual bool CanBeHurt (string weaponName) {
        return !dead;
    }

    public virtual void Hurt (int damage) {
        TakeDamage (damage);
    }

    public void TakeDamage (int damage) {
        health -= damage;

        if (health <= 0) {
            Die ();
        }
    }

    public void Die () {
        dead = true;
        DeathEffects ();
    }

    public virtual void DeathEffects () { }
}