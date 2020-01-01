public class ItemPickup : Pickup {
    public override void OnPickUp () {
        FindObjectOfType<ItemManager> ().PickUpItem (gameObject);
    }
}