using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {
    public List<string> items = new List<string> ();

    internal void PickUpItem (GameObject item) {
        FindObjectOfType<HUDController> ().Log ("picked up " + item.name);
        items.Add (item.name);
        item.SetActive (false);
    }
}