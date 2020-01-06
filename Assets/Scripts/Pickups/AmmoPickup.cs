public class AmmoPickup : Pickup {
    public int amount;

    public override void OnPickUp () {
        FindObjectOfType<WeaponController> ().PickUpAmmo (gameObject, amount);
    }
}
