using UnityEngine;

public abstract class Triggerable : MonoBehaviour {
    public abstract void Interact ();
    public virtual string GetInteractionMessage () {
        return "Press E to interact";
    }
}