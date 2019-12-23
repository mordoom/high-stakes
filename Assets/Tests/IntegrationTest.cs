using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests {
    public class IntegrationTest {
        [UnityTest]
        public IEnumerator ReduceVampireCount () {
            GameObject instance = MonoBehaviour.Instantiate (Resources.Load<GameObject> ("Prefabs/vampire"));
            GameObject gm = MonoBehaviour.Instantiate (Resources.Load<GameObject> ("Prefabs/GameManager"));

            GameManager manager = gm.GetComponent<GameManager> ();
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual (1, manager.vampiresRemainingCount);

            VampireController controller = instance.GetComponent<VampireController> ();
            Assert.AreEqual (3, controller.health);
            controller.Hurt (1);
            Assert.Less (controller.health, 3);
            while (controller.health > 0) {
                controller.Hurt (1);
            }
            Assert.AreEqual (0, controller.health);
            Assert.AreEqual (true, controller.dead);
            Assert.AreEqual (0, manager.vampiresRemainingCount);
        }
    }
}