using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

namespace Thesis
{
    public class Room : SingletonMonobehaviour<Room>
    {
        const int MAX_RESULTS = 512;

        public float minArea = 30.0f;
        public float minHorizontalArea = 20.0f;
        public float minWallArea = 5.0f;

        SpatialUnderstandingDllTopology.TopologyResult[] resultsTopology = new SpatialUnderstandingDllTopology.TopologyResult[MAX_RESULTS];

        System.IntPtr resultTopologyPtr = default(System.IntPtr);
        void Start()
        {
            resultTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsTopology);
            SpatialUnderstanding.Instance.OnScanDone += Instance_OnScanDone;
        }

        private void Instance_OnScanDone()
        {
            SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();
            Debug.Log("scan done..");
            foreach(var t in resultsTopology)
            {
                //Debug.Log(t.position);
                //Debug.DrawLine(t.position, t.position + t.normal * 2f, Random.ColorHSV());
            }
        }

        void Update()
        {

        }

        [ContextMenu("Start")]
        public void StartScan()
        {
            SpatialUnderstanding.Instance.RequestBeginScanning();
        }

        [ContextMenu("End")]
        public void EndScan()
        {
            SpatialUnderstanding.Instance.RequestFinishScan();
        }
    }
}
