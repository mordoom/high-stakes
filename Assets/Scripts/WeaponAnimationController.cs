using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimationController : MonoBehaviour {
    private Animator anim;
    private bool attacking;
    private float bloodTime = 2;
    private float splatterDelay = 0.75f;

    public float attackTime = 1f;
    public float weaponRange = 20f;
    public LayerMask layerMask;
    public GameObject blood;

    void Start () {
        anim = GetComponent<Animator> ();
    }

    void Update () {
        if (!attacking && Input.GetButtonDown ("Fire1")) {
            BeginAnimation ();
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Physics.Raycast (ray, out hit, weaponRange, layerMask)) {
                VampireController vampire = hit.collider.GetComponent<VampireController> ();
                if (vampire != null && !vampire.dead) {
                    vampire.Hurt ();
                    StartCoroutine (Splatter (hit));
                }
            }
        }
    }

    IEnumerator Splatter (RaycastHit hit) {
        yield return new WaitForSeconds (splatterDelay);
        CreateBloodSplatter (hit);
    }

    private void CreateBloodSplatter (RaycastHit hit) {
        GameObject bloodSplat = Instantiate (blood, hit.point, hit.collider.gameObject.transform.rotation);
        Destroy (bloodSplat, bloodTime);
    }

    private void BeginAnimation () {
        attacking = true;
        anim.SetBool ("attacking", true);
        StartCoroutine (Attack ());
    }
    IEnumerator Attack () {
        yield return new WaitForSeconds (attackTime);
        FinishAnimation ();
    }

    private void FinishAnimation () {
        attacking = false;
        anim.SetBool ("attacking", false);
    }

}