using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class GameManager : MonoBehaviour {
    public GameObject hud;
    public GameObject player;
    public int vampiresRemainingCount = 0;

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
        if (vampiresRemainingCount <= 0) {
            vampiresRemainingCount = 0;
            foreach (OpenCoffin coffin in FindObjectsOfType<OpenCoffin> ()) {
                coffin.OnOpen ();
            }
            HUDController hudController = FindObjectOfType<HUDController>();
            hudController.Log ("All vampires have been slain.");
            hudController.Log ("You can now exit the level.");
        }
    }

    public void Die () {
        Time.timeScale = 0;
        FirstPersonController controller = player.GetComponent<FirstPersonController>();
        FindObjectOfType<HUDController> ().Log("You are dead");
    }
}