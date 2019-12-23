using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {
    private bool attacking;
    private float bloodTime = 2;

    public float attackTime = 1f;
    public LayerMask layerMask;
    public GameObject[] weaponModels;
    public GameObject blood;

    public List<Weapon> weaponInventory = new List<Weapon> () { new Melee (), new Pistol () };
    public Weapon state;
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
    }

    void Update () {
        HandleWeaponSwitch ();
        HandleAttack ();
    }

    private void HandleAttack () {
        bool hasAmmo = state.ammo != 0;
        if (!attacking && Input.GetButtonDown ("Fire1") && hasAmmo) {
            state.ammo--;
            BeginAnimation ();
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Physics.Raycast (ray, out hit, stats.range, layerMask)) {
                EnemyController enemy = hit.collider.GetComponent<EnemyController> ();
                if (enemy != null && enemy.CanBeHurt (stats.name)) {
                    enemy.Hurt (stats.damage);
                    StartCoroutine (Splatter (hit));
                }
            }
        }
    }

    private void HandleWeaponSwitch () {
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
        GameObject bloodSplat = Instantiate (blood, hit.point, hit.collider.gameObject.transform.rotation);
        Destroy (bloodSplat, bloodTime);
    }

    private void BeginAnimation () {
        attacking = true;
        if (anim != null) {
            anim.SetBool ("attacking", true);
        }
        StartCoroutine (Attack ());
    }
    IEnumerator Attack () {
        yield return new WaitForSeconds (attackTime);
        FinishAnimation ();
    }

    private void FinishAnimation () {
        attacking = false;
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
                    hudController.DisplayMessage ("picked up " + item.name);
                    item.SetActive (false);
                } else if (weapon.ammo < weapon.maxAmmo) {
                    hudController.DisplayMessage ("picked up " + item.name);
                    weapon.AddAmmo (20);
                    item.SetActive (false);
                } else {
                    hudController.DisplayMessage (weapon.name + " ammo is full");
                }
            }
        }
    }

    public void PickUpAmmo (GameObject item, int amount) {
        foreach (Weapon weapon in weaponInventory) {
            if (item.name.Contains (weapon.name)) {
                if (weapon.ammo < weapon.maxAmmo) {
                    hudController.DisplayMessage ("picked up " + item.name + " x" + amount);
                    weapon.AddAmmo (amount);
                    item.SetActive (false);
                } else {
                    hudController.DisplayMessage (weapon.name + " ammo is full");
                }
            }
        }
    }
}