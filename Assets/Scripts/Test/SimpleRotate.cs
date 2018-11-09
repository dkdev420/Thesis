using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thesis
{
    public class SimpleRotate : MonoBehaviour
    {
        public Vector3 eulers;
        void Update() { transform.Rotate(eulers); }
    }
}