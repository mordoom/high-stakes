using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {
    public float attackTime = 1f;
    public LayerMask layerMask;
    public GameObject[] weaponModels;
    public bool makingNoise;
    public WeaponState state;
    public List<WeaponState> weaponInventory;

    public GameObject bulletHole;
    public int bulletHoleMax = 25;

    private List<GameObject> bulletHoles = new List<GameObject> ();
    private int currentBulletHoleIndex = 0;

    private bool attacking;
    private Animator anim;
    private WeaponStatsController stats;
    private WeaponBehaviour behaviour;
    private HUDController hudController;

    private void Awake () {
        InitWeaponInventory ();
        SwitchToWeapon (0);
        GameEvents.SaveInitiated += Save;
        GameEvents.LoadInitiated += Load;
    }

    void Start () {
        hudController = FindObjectOfType<HUDController> ();
        for (int i = 0; i < bulletHoleMax; i++) {
            bulletHoles.Add (Instantiate (bulletHole));
        }
    }

    private void InitWeaponInventory () {
        weaponInventory = new List<WeaponState> ();
        foreach (GameObject weaponModel in weaponModels) {
            WeaponStatsController weaponStats = weaponModel.GetComponent<WeaponStatsController> ();
            weaponInventory.Add (new WeaponState (weaponStats.name, weaponStats.initalAmmo, weaponStats.maxAmmo, weaponStats.startCollected));
        }
    }

    void Update () {
        HandleWeaponSwitch ();
        HandleAttack ();
    }

    private void HandleAttack () {
        bool hasAmmo = state.ammo != 0;
        if (!attacking && behaviour.isShooting () && hasAmmo) {
            if (stats.name != "melee") {
                makingNoise = true;
                state.ammo--;
            }
            BeginAnimation ();

            for (int i = 0; i < stats.pellets; i++) {
                RaycastHit hit;
                if (behaviour.isShotAHit (i, out hit)) {
                    DetectBulletHit (hit);
                }
            }
        }
    }

    private void DetectBulletHit (RaycastHit hit) {
        EnemyController enemy = hit.collider.GetComponent<EnemyController> ();
        Shootable shootable = hit.collider.GetComponent<Shootable> ();
        if (enemy != null) {
            if (stats.name == "melee") {
                state.ammo--;
            }
            behaviour.DamageEnemy (enemy, hit);
        } else if (shootable != null) {
            shootable.wasShotBy (stats);
        } else {
            DrawBulletHole (hit);
        }
    }

    private void HandleWeaponSwitch () {
        CheckWeaponKeyPress ();
        CheckMouseWheelScroll ();
    }

    private void CheckWeaponKeyPress () {
        for (int i = 0; i < weaponInventory.Count; i++) {
            string weaponKey = (i + 1).ToString ();
            if (Input.GetKeyDown (weaponKey) && weaponInventory[i].collected) {
                SwitchToWeapon (i);
            }
        }
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

    private void SwitchToWeapon (int index) {
        foreach (GameObject weapon in weaponModels) {
            weapon.SetActive (false);
        }
        weaponModels[index].SetActive (true);
        state = weaponInventory[index];
        GameObject model = weaponModels[index];
        anim = model.GetComponent<Animator> ();
        stats = model.GetComponent<WeaponStatsController> ();
        behaviour = model.GetComponent<WeaponBehaviour> ();
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
            List<WeaponState> loadedWeapons = SaveLoad.Load<List<WeaponState>> ("weaponInventory");
            weaponInventory = loadedWeapons;
        }
    }

    internal string HudAmmoCount () {
        return state.ammo >= 0 ? state.ammo.ToString () : "unlimited";
    }

    public void PickUpWeapon (GameObject item) {
        foreach (WeaponState weapon in weaponInventory) {
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
        foreach (WeaponState weapon in weaponInventory) {
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