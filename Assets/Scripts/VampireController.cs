using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireController : MonoBehaviour {
    public int health = 3;
    public bool dead = false;

    void Start () {

    }

    void Update () {

    }

    internal void Hurt () {
        if (dead) {
            return;
        }

        health--;

        if (health <= 0) {
            Die ();
        }
    }

    private void Die () {
        Debug.Log ("dead");
        dead = true;
    }
}