using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {
    public Text vampiresRemainingCountText;
    public Text ammoCountText;
    public Text healthText;
    public Text armorText;
    public Text vampireHealthText;
    public Text log;
    public float logTextDisplayLength = 3;
    public GameObject vampireHealthDisplay;
    public GameObject playerHurt;

    private GameManager gameManager;
    private WeaponController weaponController;
    private HealthController healthController;
    private float playerHurtLength = 0.25f;

    void Start () {
        gameManager = FindObjectOfType<GameManager> ();
        weaponController = FindObjectOfType<WeaponController> ();
        healthController = FindObjectOfType<HealthController> ();
        vampireHealthDisplay.SetActive (false);
        log.text = "";
        playerHurt.SetActive (false);
    }

    void Update () {
        vampiresRemainingCountText.text = gameManager.vampiresRemainingCount.ToString ();
        ammoCountText.text = weaponController.HudAmmoCount ();
        healthText.text = healthController.health.ToString ();
        armorText.text = healthController.armor.ToString ();
    }

    public void Log (string message) {
        string newText = message.ToUpper ();
        if (string.IsNullOrEmpty (log.text)) {
            log.text = newText;
        } else {
            log.text = newText + "\n" + log.text;
        }
        StartCoroutine (ClearText ());
    }

    IEnumerator ClearText () {
        yield return new WaitForSeconds (logTextDisplayLength);

        string[] logMessages = log.text.Split ('\n');
        string newLog = "";
        if (logMessages.Length > 1) {
            Array.Resize (ref logMessages, logMessages.Length - 1);
            newLog = string.Join ("\n", logMessages);
        }
        log.text = newLog;
    }

    internal void Die () {
        Log ("You are dead");
        playerHurt.SetActive (true);
    }

    public void ShowPlayerHurt () {
        playerHurt.SetActive (true);
        StartCoroutine (ClearPlayerHurt ());
    }

    IEnumerator ClearPlayerHurt () {
        yield return new WaitForSeconds (playerHurtLength);
        playerHurt.SetActive (false);
    }

    public void ShowVampireHealth (int health) {
        vampireHealthDisplay.SetActive (true);
        vampireHealthText.text = health.ToString ();
    }

    public void HideVampireHealth () {
        vampireHealthDisplay.SetActive (false);
    }
}