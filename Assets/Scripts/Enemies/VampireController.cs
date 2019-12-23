public class VampireController : EnemyController {
    public override bool CanBeHurt (string weaponName) {
        return !dead && weaponName == "melee";
    }

    public override void Hurt (int damage) {
        TakeDamage (1);
    }

    public override void DeathEffects () {
        GameManager gameManager = FindObjectOfType<GameManager> ();
        gameManager.ReduceVampireCount ();
    }
}