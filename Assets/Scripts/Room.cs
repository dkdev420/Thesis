using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine.AI;

namespace Thesis
{
    public class Room : SingletonMonobehaviour<Room>
    {
        const int MAX_RESULTS = 512;

        public float minArea = 30.0f;
        public float minHorizontalArea = 20.0f;
        public float minWallArea = 5.0f;

        public NavMeshSurface navMeshSurface;
        void Start()
        {
            SpatialUnderstanding.Instance.OnScanDone += Instance_OnScanDone;
            SpatialMappingManager.Instance.SurfaceObserver.SurfaceUpdated += SurfaceObserver_SurfaceUpdated;
        }

        private void SurfaceObserver_SurfaceUpdated(object sender, DataEventArgs<SpatialMappingSource.SurfaceUpdate> e)
        {
            Debug.Log("Surface updated..");
            BuildNavMesh();
        }

        private void Instance_OnScanDone() { }

        [ContextMenu("Build NavMesh")]
        public void BuildNavMesh()
        {
            if (navMeshSurface) navMeshSurface.BuildNavMesh();
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
