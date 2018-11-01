using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thesis
{
    public class DummyObject : MonoBehaviour
    {
        public void FocusIn() { Debug.Log("Focused: " + gameObject.name); }
        public void FocusOut() { Debug.Log("Out Focused: " + gameObject.name); }
        public void Selected() { Debug.Log("Selected: " + gameObject.name); }
    }
}
