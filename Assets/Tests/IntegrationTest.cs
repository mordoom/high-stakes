using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests {
    public class IntegrationTest {
        [UnityTest]
        public IEnumerator ReduceVampireCount () {
            SceneManager.LoadScene(0);

            yield return new WaitForSeconds(1f);
            GameManager manager = MonoBehaviour.FindObjectOfType<GameManager>();
            Assert.AreEqual (1, manager.vampiresRemainingCount);

            VampireController controller = MonoBehaviour.FindObjectOfType<VampireController>();
            Assert.AreEqual (false, controller.dead);
            Assert.Less (0, controller.health);
            while (controller.health > 0) {
                controller.Hurt (100, "melee");
            }
            Assert.AreEqual (0, controller.health);
            Assert.AreEqual (true, controller.dead);
            Assert.AreEqual (0, manager.vampiresRemainingCount);
        }
    }
}