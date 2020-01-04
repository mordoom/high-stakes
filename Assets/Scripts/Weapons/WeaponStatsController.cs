using UnityEngine;

public class WeaponStatsController : MonoBehaviour {
    public string name;
    public float range = 9999f;
    public float fireRate = 0.25f;
    public float splatterDelay = 0;
    public int damage = 25;
    public int maxAmmo = 299;
    public bool auto = false;
    public bool projectile = false;
    public float effectiveRange = 9999f;
    public int bulletForce = 3;
    public int pellets = 1;
    public float spreadDeviation = 0;
    public AudioClip shootSound;
    public int initalAmmo = -1;
    public bool startCollected = false;
}
