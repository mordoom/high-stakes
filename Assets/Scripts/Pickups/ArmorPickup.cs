public class ArmorPickup : Pickup {
    public int amount = 25;

    public override void OnPickUp () {
        FindObjectOfType<HealthController> ().PickUpArmor (gameObject, amount);
    }
}