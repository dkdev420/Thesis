using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thesis
{
    public class DemoManager : MonoBehaviour
    {
        [SerializeField] GameObject uI;

        private void Start()
        {
            Grabber.Instance.OnGrabbed += Instance_OnGrabbed;
            Grabber.Instance.OnReleased += Instance_OnReleased;
        }

        private void Instance_OnReleased(GameObject obj)
        {
            if (uI) uI.SetActive(true);
            HoloInputController.Instance.gazeDistanceFromCamera = 20f;
            Cursor.Instance.listenForHit = true;
        }

        private void Instance_OnGrabbed(GameObject obj)
        {
            if (uI) uI.SetActive(false);
            HoloInputController.Instance.gazeDistanceFromCamera = 1f;
            Cursor.Instance.listenForHit = false;
        }

        public void SpawnGameObject(GameObject g)
        {
            Debug.Log("spawning..");
            GameObject clone = Instantiate(g);
            clone.SetActive(true);
            Grabber.Instance.Grab(clone);            
        }
    }
}
