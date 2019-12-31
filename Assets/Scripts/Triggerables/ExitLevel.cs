using UnityEngine;

public class ExitLevel : Triggerable {
    private GameManager gameManager;
    private bool open;
    public GameObject exitLight;

    void Start () {
        gameManager = FindObjectOfType<GameManager> ();
        exitLight.SetActive(false);
    }

    void Update () {
        if (gameManager.vampiresRemainingCount == 0) {
            open = true;
        }
        if (open) {
            exitLight.SetActive (true);
        }
    }

    public override void Interact () {
        if (open) {
            gameManager.NextLevel();
        } else {
            FindObjectOfType<HUDController> ().Log ("Door is locked");
        }
    }
}