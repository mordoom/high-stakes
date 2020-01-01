using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodManager : MonoBehaviour {
    public GameObject blood;
    public float bloodTime = 2;
    public int bloodSplatterMax = 250;

    private int currentBloodSplatterIndex = 0;
    private List<GameObject> bloodSplatters = new List<GameObject> ();

    public void Splatter (RaycastHit hit, float splatterDelay) {
        StartCoroutine (CreateSplatter (hit, splatterDelay));
    }

    IEnumerator CreateSplatter (RaycastHit hit, float splatterDelay) {
        yield return new WaitForSeconds (splatterDelay);
        if (hit.collider != null) {
            if (bloodSplatters.Count < bloodSplatterMax) {
                GameObject bloodSplat = Instantiate (blood, hit.point, hit.collider.gameObject.transform.rotation);
                Destroy (bloodSplat, bloodTime);
            } else {
                GameObject currentBloodSplatter = bloodSplatters[currentBloodSplatterIndex];
                currentBloodSplatter.transform.position = hit.point;
                currentBloodSplatter.transform.rotation = Quaternion.FromToRotation (Vector3.up, hit.normal);
                currentBloodSplatter.transform.parent = hit.transform;
                currentBloodSplatterIndex++;
                if (currentBloodSplatterIndex >= bloodSplatterMax) {
                    currentBloodSplatterIndex = 0;
                }
            }
        }
    }
}