using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodManager : MonoBehaviour {
    public GameObject blood;
    public float bloodTime = 2;

    public GameObject bloodStain;
    public int bloodStainMax = 1000;

    private int currentBloodSplatterIndex = 0;
    private List<GameObject> bloodCache = new List<GameObject> ();

    public GameObject gibs;
    public float gibsTime = 3;

    void Start () {
        // for (int i = 0; i < bloodStainMax; i++) {
        //     bloodCache.Add (Instantiate (bloodStain));
        // }
    }

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
        if (bloodCache.Count < bloodStainMax) {
            GameObject currentStain = Instantiate (bloodStain);
            currentStain.transform.position = collision.intersection;
            currentStain.transform.rotation = Quaternion.FromToRotation (Vector3.up, collision.normal);
            currentStain.transform.parent = collision.colliderComponent.transform;
            bloodCache.Add (currentStain);
        } else {
            GameObject currentStain = bloodCache[currentBloodSplatterIndex];
            currentStain.transform.position = collision.intersection;
            currentStain.transform.rotation = Quaternion.FromToRotation (Vector3.up, collision.normal);
            currentStain.transform.parent = collision.colliderComponent.transform;
            currentBloodSplatterIndex++;
            if (currentBloodSplatterIndex >= bloodStainMax) {
                currentBloodSplatterIndex = 0;
            }
        }
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