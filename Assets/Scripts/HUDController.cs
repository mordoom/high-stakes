using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {
    public Text vampiresRemainingCountText;
    public Text ammoCountText;
    public Text healthText;
    public Text vampireHealthText;
    public GameObject vampireHealthDisplay;
    private GameManager gameManager;
    private WeaponController weaponController;
    private HealthController healthController;

    void Start () {
        gameManager = FindObjectOfType<GameManager> ();
        weaponController = FindObjectOfType<WeaponController> ();
        healthController = FindObjectOfType<HealthController> ();
        vampireHealthDisplay.SetActive (false);
    }

    void Update () {
        vampiresRemainingCountText.text = gameManager.vampiresRemainingCount.ToString ();
        ammoCountText.text = weaponController.HudAmmoCount ();
        healthText.text = healthController.health.ToString ();
    }

    public void DisplayMessage (string message) {
        Debug.Log (message);
    }

    public void ShowVampireHealth (int health) {
        vampireHealthDisplay.SetActive (true);
        vampireHealthText.text = health.ToString ();
    }

    public void HideVampireHealth () {
        vampireHealthDisplay.SetActive (false);
    }
}