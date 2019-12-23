using UnityEngine;

public class HealthController : MonoBehaviour {
    public int health = 100;
    public int armor = 0;
    private bool dead;
    private int maxHealth = 100;
    private int maxArmor = 100;
    private GameManager gameManager;
    private HUDController hudController;

    void Awake () {
        GameEvents.SaveInitiated += Save;
        GameEvents.LoadInitiated += Load;
    }

    void Start () {
        gameManager = FindObjectOfType<GameManager>();
        hudController = FindObjectOfType<HUDController>();
    }

    void Save () {
        int[] healthArmor = { health, armor };
        SaveLoad.Save (healthArmor, "playerHealth");
    }

    void Load () {
        int[] healthArmor = SaveLoad.Load<int[]> ("playerHealth");
        health = healthArmor[0];
        armor = healthArmor[1];
    }

    public void PickUpHealth (GameObject item, int amount) {
        if (health < maxHealth) {
            hudController.DisplayMessage ("picked up " + item.name);
            health += amount;
            item.SetActive (false);
            if (health > maxHealth) {
                health = maxHealth;
            }
        } else {
            hudController.DisplayMessage ("health at max");
        }
    }

    public void PickUpArmor (GameObject item, int amount) {
        if (armor < maxArmor) {
            hudController.DisplayMessage ("picked up " + item.name);
            armor += amount;
            item.SetActive (false);
            if (armor > maxArmor) {
                armor = maxArmor;
            }
        } else {
            hudController.DisplayMessage ("armor at max");
        }
    }

    public void TakeDamage (int amount) {
        if (dead) {
            return;
        }

        int finalAmount = amount;

        if (armor > 0) {
            finalAmount = amount / 2;
            armor -= finalAmount;
            if (armor < 0) {
                finalAmount += armor;
                armor = 0;
            }
        }

        health -= finalAmount;
        if (health <= 0) {
            health = 0;
            Die ();
        }
    }

    private void Die () {
        dead = true;
        gameManager.Die();
    }
}