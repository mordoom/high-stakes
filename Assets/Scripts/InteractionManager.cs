using UnityEngine;

public class InteractionManager : MonoBehaviour {
    public float interactRange = 3;
    public LayerMask layerMask;

    void Update () {
        if (Input.GetKeyDown (KeyCode.E)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Physics.Raycast (ray, out hit, interactRange, layerMask)) {
                Triggerable triggerable = hit.collider.GetComponent<Triggerable> ();
                if (triggerable != null) {
                    triggerable.Interact ();
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