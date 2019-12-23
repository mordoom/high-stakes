[System.Serializable]
public abstract class Weapon {
    public string name;
    public bool collected = false;
    public int ammo = 10;
    public int maxAmmo = 299;

    public Weapon (string name) {
        this.name = name;
    }

    public void AddAmmo (int amount) {
        ammo += amount;
        if (ammo > maxAmmo) {
            ammo = maxAmmo;
        }
    }
}

[System.Serializable]
public class Melee : Weapon {
    public Melee () : base ("melee") {
        ammo = -1;
        maxAmmo = 1;
        collected = true;
    }
}

[System.Serializable]
public class Pistol : Weapon {
    public Pistol () : base ("pistol") {
        collected = true;
    }
}
