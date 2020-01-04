[System.Serializable]
public class WeaponState {
    public string name;
    public bool collected = false;
    public int ammo = 10;
    public int maxAmmo = 299;

    public WeaponState (string name) {
        this.name = name;
    }

    public WeaponState (string name, int ammo, int maxAmmo, bool collected) {
        this.name = name;
        this.ammo = ammo;
        this.maxAmmo = maxAmmo;
        this.collected = collected;
    }

    public void AddAmmo (int amount) {
        ammo += amount;
        if (ammo > maxAmmo) {
            ammo = maxAmmo;
        }
    }
}