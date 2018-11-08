using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thesis
{
    public class Grabber : MonoBehaviour
    {
        public GameObject grabbed;

        public MeshRenderer cursorRenderer;

        void Start()
        {
            HoloInputController.Instance.InteractionSourcePressed += Instance_InteractionSourcePressed;
            cursorRenderer = Cursor.Instance.GetComponentInChildren<MeshRenderer>(false);   
        }

        private void Instance_InteractionSourcePressed(UnityEngine.XR.WSA.Input.InteractionSourcePressedEventArgs obj)
        {
            if (grabbed)
            {
                cursorRenderer.enabled = true;
                grabbed.GetComponent<Collider>().enabled = true;
                grabbed = null;
            }
            else
            {
                cursorRenderer.enabled = false;
                grabbed = HoloInputController.Instance.Current.gameObject;
                grabbed.GetComponent<Collider>().enabled = false;
            }
        }

        void Update()
        {
            UpdateGrabbedPosition();
        }

        void UpdateGrabbedPosition()
        {
            grabbed.transform.position = Cursor.Instance.transform.position;
        }
    }
}
