using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponController : MonoBehaviour {
    public float attackTime = 1f;
    public LayerMask layerMask;
    public GameObject[] weaponModels;
    public GameObject blood;
    public bool makingNoise;
    public Weapon state;
    public List<Weapon> weaponInventory = new List<Weapon> () { new Melee (), new Pistol (), new Shotgun () };

    public GameObject bulletHole;
    List<GameObject> bulletHoles = new List<GameObject> ();
    public int bulletHoleMax = 25;
    private int currentBulletHoleIndex = 0;

    private bool attacking;
    private float bloodTime = 2;
    private Animator anim;
    private WeaponStatsController stats;
    private HUDController hudController;

    private void Awake () {
        GameEvents.SaveInitiated += Save;
        GameEvents.LoadInitiated += Load;
    }

    void Start () {
        SwitchToWeapon (0);
        hudController = FindObjectOfType<HUDController> ();
        for (int i = 0; i < bulletHoleMax; i++) {
            bulletHoles.Add (Instantiate (bulletHole));
        }
    }

    void Update () {
        HandleWeaponSwitch ();
        HandleAttack ();
    }

    private void HandleAttack () {
        bool hasAmmo = state.ammo != 0;
        if (!attacking && Input.GetButtonDown ("Fire1") && hasAmmo) {
            if (stats.name != "melee") {
                makingNoise = true;
                state.ammo--;
            }
            BeginAnimation ();

            for (int i = 0; i < stats.pellets; i++) {
                float maxDeviation = stats.spreadDeviation * i;
                float xDeviation = Random.Range (-maxDeviation, maxDeviation);
                float yDeviation = Random.Range (-maxDeviation, maxDeviation);

                Vector3 bulletPos = new Vector3 (Input.mousePosition.x + xDeviation, Input.mousePosition.y + yDeviation, Input.mousePosition.z);
                Ray ray = Camera.main.ScreenPointToRay (bulletPos);
                RaycastHit hit;
                if (Physics.Raycast (ray, out hit, stats.range, layerMask)) {
                    EnemyController enemy = hit.collider.GetComponent<EnemyController> ();
                    if (enemy != null) {
                        if (!hit.collider.isTrigger && enemy != null && enemy.CanBeHurt (stats.name)) {
                            if (stats.name == "melee") {
                                state.ammo--;
                            }
                            enemy.Hurt (stats.damage, stats.name);
                            Rigidbody rb = hit.collider.GetComponent<Rigidbody> ();
                            if (rb != null) {
                                Vector3 force = transform.forward * stats.bulletForce;
                                rb.AddForce (force);
                            }

                            StartCoroutine (Splatter (hit));
                        }
                    } else {
                        DrawBulletHole (hit);
                    }
                }
            }
        }
    }

    private void HandleWeaponSwitch () {
        CheckWeaponKeyPress ();
        CheckMouseWheelScroll ();
    }

    private void CheckMouseWheelScroll () {
        int currentWeaponIndex = weaponInventory.IndexOf (state);

        if (Input.GetAxis ("Mouse ScrollWheel") > 0f) {
            int nextWeaponIndex = currentWeaponIndex + 1 >= weaponInventory.Count ? 0 : currentWeaponIndex + 1;
            while (!weaponInventory[nextWeaponIndex].collected) {
                nextWeaponIndex = nextWeaponIndex + 1 >= weaponInventory.Count ? 0 : nextWeaponIndex + 1;
            }

            SwitchToWeapon (nextWeaponIndex);
        } else if (Input.GetAxis ("Mouse ScrollWheel") < 0f) {
            int prevWeaponIndex = currentWeaponIndex - 1 < 0 ? weaponInventory.Count - 1 : currentWeaponIndex - 1;
            while (!weaponInventory[prevWeaponIndex].collected) {
                prevWeaponIndex = prevWeaponIndex - 1 < 0 ? weaponInventory.Count - 1 : prevWeaponIndex - 1;
            }

            SwitchToWeapon (prevWeaponIndex);
        }
    }

    private void DrawBulletHole (RaycastHit hit) {
        GameObject currentHole = bulletHoles[currentBulletHoleIndex];
        currentHole.transform.position = hit.point;
        currentHole.transform.rotation = Quaternion.FromToRotation (Vector3.up, hit.normal);
        currentHole.transform.parent = hit.transform;
        currentBulletHoleIndex++;
        if (currentBulletHoleIndex >= bulletHoleMax) {
            currentBulletHoleIndex = 0;
        }
    }

    private void CheckWeaponKeyPress () {
        for (int i = 0; i < weaponInventory.Count; i++) {
            string weaponKey = (i + 1).ToString ();
            if (Input.GetKeyDown (weaponKey) && weaponInventory[i].collected) {
                SwitchToWeapon (i);
            }
        }
    }

    private void SwitchToWeapon (int index) {
        foreach (GameObject weapon in weaponModels) {
            weapon.SetActive (false);
        }
        weaponModels[index].SetActive (true);
        state = weaponInventory[index];
        GameObject model = weaponModels[index];
        anim = model.GetComponent<Animator> ();
        stats = model.GetComponent<WeaponStatsController> ();
    }

    IEnumerator Splatter (RaycastHit hit) {
        yield return new WaitForSeconds (stats.splatterDelay);
        CreateBloodSplatter (hit);
    }

    private void CreateBloodSplatter (RaycastHit hit) {
        if (hit.collider != null) {
            GameObject bloodSplat = Instantiate (blood, hit.point, hit.collider.gameObject.transform.rotation);
            Destroy (bloodSplat, bloodTime);
        }
    }

    private void BeginAnimation () {
        attacking = true;
        if (anim != null) {
            anim.SetBool ("attacking", true);
        }
        StartCoroutine (Attack ());
    }

    IEnumerator Attack () {
        yield return new WaitForSeconds (stats.fireRate);
        FinishAnimation ();
    }

    private void FinishAnimation () {
        attacking = false;
        makingNoise = false;
        if (anim != null) {
            anim.SetBool ("attacking", false);
        }
    }

    void Save () {
        SaveLoad.Save (weaponInventory, "weaponInventory");
    }

    void Load () {
        if (SaveLoad.SaveExistsAt ("weaponInventory")) {
            List<Weapon> loadedWeapons = SaveLoad.Load<List<Weapon>> ("weaponInventory");
            weaponInventory = loadedWeapons;
        }
    }

    internal string HudAmmoCount () {
        return state.ammo >= 0 ? state.ammo.ToString () : "unlimited";
    }

    public void PickUpWeapon (GameObject item) {
        foreach (Weapon weapon in weaponInventory) {
            if (item.name == weapon.name) {
                if (!weapon.collected) {
                    weapon.collected = true;
                    SwitchToWeapon (weaponInventory.IndexOf (weapon));
                    hudController.Log ("picked up " + item.name);
                    item.SetActive (false);
                } else if (weapon.ammo < weapon.maxAmmo) {
                    hudController.Log ("picked up " + item.name);
                    weapon.AddAmmo (20);
                    item.SetActive (false);
                } else {
                    hudController.Log (weapon.name + " ammo is full");
                }
            }
        }
    }

    public void PickUpAmmo (GameObject item, int amount) {
        foreach (Weapon weapon in weaponInventory) {
            if (item.name.Contains (weapon.name)) {
                if (weapon.ammo < weapon.maxAmmo) {
                    hudController.Log ("picked up " + item.name + " x" + amount);
                    weapon.AddAmmo (amount);
                    item.SetActive (false);
                } else {
                    hudController.Log (weapon.name + " ammo is full");
                }
            }
        }
    }
}