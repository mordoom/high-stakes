using UnityEngine;

public class InteractionManager : MonoBehaviour {
    public float interactRange = 3;
    public LayerMask layerMask;
    private HUDController hud;

    void Start() {
        hud = FindObjectOfType<HUDController>();
    }

    void Update () {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        if (Physics.Raycast (ray, out hit, interactRange, layerMask)) {
            Triggerable triggerable = hit.collider.GetComponent<Triggerable> ();
            if (triggerable != null) {
                if (Input.GetKeyDown (KeyCode.E)) {
                    triggerable.Interact ();
                } else {
                    hud.DisplayMessage (triggerable.GetInteractionMessage ());
                }
            }
        }
    }

    void OnTriggerEnter (Collider collider) {
        Pickup pickup = collider.gameObject.GetComponent<Pickup> ();
        if (pickup != null) {
            pickup.OnPickUp ();
        }
    }
}