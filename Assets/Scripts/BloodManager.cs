using System.Collections;
using UnityEngine;

public class BloodManager : MonoBehaviour {
    public GameObject blood;
    public float bloodTime = 2;

    public GameObject bloodStain;
    public GameObject gibs;
    public float gibsTime = 3;
    private float bloodTimeOut = 10;

    public void Splatter (RaycastHit hit, float splatterDelay) {
        StartCoroutine (CreateSplatter (hit, splatterDelay));
    }

    IEnumerator CreateSplatter (RaycastHit hit, float splatterDelay) {
        yield return new WaitForSeconds (splatterDelay);
        if (hit.collider != null) {
            Vector3 bloodPos = hit.point;
            Quaternion bloodRotation = hit.collider.transform.rotation;
            GameObject bloodSplat = Instantiate (blood, bloodPos, bloodRotation);
            Destroy (bloodSplat, bloodTime);
        }
    }

    internal void CreateBloodStain (ParticleCollisionEvent collision) {
        GameObject currentStain = Instantiate (bloodStain);
        currentStain.transform.position = collision.intersection;
        currentStain.transform.rotation = Quaternion.FromToRotation (Vector3.up, collision.normal);
        currentStain.transform.parent = collision.colliderComponent.transform;
        Destroy(currentStain, bloodTimeOut);
    }

    IEnumerator CreateExplosionSplatter (GameObject thingExploding, float splatterDelay) {
        yield return new WaitForSeconds (splatterDelay);
        if (thingExploding != null) {
            Vector3 gibPosition = new Vector3 (thingExploding.transform.position.x, thingExploding.transform.position.y + 1, thingExploding.transform.position.z);
            Quaternion gibRotation = Quaternion.identity;

            GameObject gibsSplat = Instantiate (gibs, gibPosition, thingExploding.transform.rotation);
            Destroy (gibsSplat, gibsTime);

            GameObject bloodSplat = Instantiate (blood, gibPosition, gibRotation);
            Destroy (bloodSplat, bloodTime);
        }
    }

    internal void Explode (GameObject thingExploding) {
        StartCoroutine (CreateExplosionSplatter (thingExploding, 0));
    }
}