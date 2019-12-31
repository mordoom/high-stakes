using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {
    public Text vampiresRemainingCountText;
    public Text ammoCountText;
    public Text healthText;
    public Text vampireHealthText;
    public Text log;
    public float logTextDisplayLength = 3;
    public GameObject vampireHealthDisplay;

    private GameManager gameManager;
    private WeaponController weaponController;
    private HealthController healthController;

    void Start () {
        gameManager = FindObjectOfType<GameManager> ();
        weaponController = FindObjectOfType<WeaponController> ();
        healthController = FindObjectOfType<HealthController> ();
        vampireHealthDisplay.SetActive (false);
        log.text = "";
    }

    void Update () {
        vampiresRemainingCountText.text = gameManager.vampiresRemainingCount.ToString ();
        ammoCountText.text = weaponController.HudAmmoCount ();
        healthText.text = healthController.health.ToString ();
    }

    public void Log (string message) {
        string newText = message.ToUpper();
        if (string.IsNullOrEmpty(log.text)) {
            log.text = newText;
        }
        else {
            log.text = newText + "\n" + log.text;
        }
        StartCoroutine(ClearText());
    }

    IEnumerator ClearText() {
        yield return new WaitForSeconds(logTextDisplayLength);

        string[] logMessages = log.text.Split('\n');
        string newLog = "";
        if (logMessages.Length > 1) {
            Array.Resize(ref logMessages, logMessages.Length - 1);
            newLog = string.Join("\n", logMessages);
        }
        log.text = newLog;
    }

    public void ShowVampireHealth (int health) {
        vampireHealthDisplay.SetActive (true);
        vampireHealthText.text = health.ToString ();
    }

    public void HideVampireHealth () {
        vampireHealthDisplay.SetActive (false);
    }
}