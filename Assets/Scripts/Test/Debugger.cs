using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Thesis
{
    public class Debugger : MonoBehaviour
    {
        public Text scanState;
        void Update()
        {
            scanState.text = SpatialUnderstanding.Instance.ScanState.ToString();
        }
    }
}