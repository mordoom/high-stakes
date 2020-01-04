using UnityEngine;

public class ThrowableBehaviour : WeaponBehaviour {
    public GameObject throwable;
    public float throwForce = 20;

    public override bool isShotAHit (int i, out RaycastHit hit) {
        Transform playerTransform = player.transform;
        Vector3 newProjectilePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        GameObject newThrowable = Instantiate (throwable, newProjectilePos, playerTransform.rotation);

        Vector3 dir = Camera.main.transform.forward;
        newThrowable.GetComponent<Rigidbody> ().AddForce (dir * throwForce, ForceMode.Impulse);
        hit = new RaycastHit ();
        return false;
    }

    public override void DamageEnemy (EnemyController enemy, RaycastHit hit) { }
}