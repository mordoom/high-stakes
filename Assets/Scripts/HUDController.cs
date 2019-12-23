using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {
    public Text vampiresRemainingCountText;
    public Text ammoCountText;
    public Text healthText;
    private GameManager gameManager;
    private WeaponController weaponController;
    private HealthController healthController;

    void Start () {
        gameManager = FindObjectOfType<GameManager> ();
        weaponController = FindObjectOfType<WeaponController> ();
        healthController = FindObjectOfType<HealthController> ();
    }

    void Update () {
        vampiresRemainingCountText.text = gameManager.vampiresRemainingCount.ToString ();
        ammoCountText.text = weaponController.HudAmmoCount();
        healthText.text = healthController.health.ToString();
    }

    internal void DisplayMessage (string message) {
        Debug.Log (message);
    }
}