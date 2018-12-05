using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thesis
{
    public class Grabber : SingletonMonobehaviour<Grabber>
    {
        [SerializeField] private GameObject grabbed;
        [SerializeField] MeshRenderer cursorRenderer;

        public event System.Action<GameObject> OnGrabbed;
        public event System.Action<GameObject> OnReleased;

        void Start()
        {
            HoloInputController.Instance.InteractionSourcePressed += Instance_InteractionSourcePressed;
            cursorRenderer = Cursor.Instance.GetComponentInChildren<MeshRenderer>(false);   
        }

        private void Instance_InteractionSourcePressed(UnityEngine.XR.WSA.Input.InteractionSourcePressedEventArgs obj)
        {
            if (grabbed && grabbedTime > .5f) Release();
            else if(!(HoloInputController.Instance.Current.gameObject.GetComponent<Button3D>())) Grab();
        }

        void Update()
        {
            UpdateGrabbedPosition();
        }

        private float grabbedTime = 0f;
        void UpdateGrabbedPosition()
        {
            if (grabbed)
            {
                grabbedTime += Time.deltaTime;
                grabbed.transform.position = Cursor.Instance.transform.position;
            }
            else grabbedTime = 0f;
        }

        public void Grab(GameObject g = null)
        {
            grabbed = g ? g : HoloInputController.Instance.Current.gameObject;
            if (!grabbed) return;
            cursorRenderer.enabled = false;
            var c = grabbed.GetComponent<Collider>();
            var r = grabbed.GetComponent<Rigidbody>();
            if (c) c.enabled = false;
            if (r)
            {
                r.useGravity = false;
                r.detectCollisions = false;
            }
            if(OnGrabbed != null) OnGrabbed(grabbed);
        }

        public void Release()
        {
            if (!grabbed) return;
            Debug.Log("released..");
            cursorRenderer.enabled = true;
            GameObject released = grabbed;
            var c = grabbed.GetComponent<Collider>();
            var r = grabbed.GetComponent<Rigidbody>();
            if (c) c.enabled = true;
            if (r)
            {
                r.useGravity = true;
                r.detectCollisions = true;
            }
            grabbed = null;
            if (OnReleased != null) OnReleased(released);
        }

    }
}
