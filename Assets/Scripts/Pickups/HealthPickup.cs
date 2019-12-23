public class HealthPickup : Pickup {
    public int amount = 25;

    public override void OnPickUp () {
        FindObjectOfType<HealthController> ().PickUpHealth (gameObject, amount);
    }
}