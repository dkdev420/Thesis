using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Thesis
{
    public class Interactable : MonoBehaviour
    {
        public UnityEvent onFocus;
        public UnityEvent outFocus;
        public UnityEvent selected;
    }
}