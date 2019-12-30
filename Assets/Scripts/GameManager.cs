using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class GameManager : MonoBehaviour {
    public int vampiresRemainingCount = 0;
    public GameObject hud;
    public GameObject player;

    void Start () {
        vampiresRemainingCount = FindObjectsOfType<VampireController> ().Length;
        Instantiate (player, transform.position, transform.rotation);
        Instantiate (hud);

        int sceneIndex = SceneManager.GetActiveScene ().buildIndex;
        if (sceneIndex > 0) {
            GameEvents.OnLoadInitiated ();
        }
    }

    internal void NextLevel () {
        GameEvents.OnSaveInitiated ();
        SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
    }

    public void ReduceVampireCount () {
        vampiresRemainingCount--;
        if (vampiresRemainingCount == 0) {
            foreach (OpenCoffin coffin in FindObjectsOfType<OpenCoffin> ()) {
                coffin.OnOpen ();
            }
            hud.GetComponent<HUDController> ().DisplayMessage ("All vampires have been slain.\nYou can now exit the level.");
        }
    }

    public void Die () {
        Time.timeScale = 0;
        FirstPersonController controller = player.GetComponent<FirstPersonController>();
        hud.GetComponent<HUDController> ().DisplayMessage("You are dead");
    }
}