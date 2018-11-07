using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine.AI;
using UnityEngine.XR.WSA;

namespace Thesis
{
    public class Room : SingletonMonobehaviour<Room>
    {
        const int MAX_RESULTS = 512;

        public float minArea = 30.0f;
        public float minHorizontalArea = 20.0f;
        public float minWallArea = 5.0f;

        void Start()
        {
            SpatialUnderstanding.Instance.OnScanDone += Instance_OnScanDone;
            //SpatialMappingManager.Instance.
            SpatialMappingManager.Instance.SurfaceObserver.SurfaceUpdated += SurfaceObserver_SurfaceUpdated;
            //SpatialMappingManager.Instance.SurfaceObserver.Observer.Update(SurfaceObserver_OnSurfaceChanged);
            SpatialUnderstanding.Instance.ScanStateChanged += Instance_ScanStateChanged;
        }

        private void Instance_ScanStateChanged()
        {
        }

        bool navMeshReady;
        private void SurfaceObserver_SurfaceUpdated(object sender, DataEventArgs<SpatialMappingSource.SurfaceUpdate> e)
        {
            //FIXME: Only when surface changes...
            Debug.Log("Surface updated..");
            BuildNavMesh();
        }

        private void Instance_OnScanDone()
        {
            Debug.Log("Done..");
        }

        public IEnumerator PlaceGameObject(GameObject g, SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placementDefinition, List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule> placementRules = null, List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint> placementConstraints = null)
        {
            string name = g.name;
            var thread =
#if UNITY_EDITOR || !UNITY_WSA
            new System.Threading.Thread
#else
            System.Threading.Tasks.Task.Run
#endif
            (() =>
            {
                _PlaceGameObject(name, placementDefinition, placementRules, placementConstraints);
            }
            );
#if UNITY_EDITOR || !UNITY_WSA
            thread.Start()
#endif
            ;

            while
                (
#if UNITY_EDITOR || !UNITY_WSA
                !thread.Join(System.TimeSpan.Zero)
#else
                !thread.IsCompleted
#endif
                )
            {
                yield return null;
            }

            //SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult placementResult = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticObjectPlacementResult();
            //g.transform.position = placementResult.Position;

        }

        private bool _PlaceGameObject(string name, SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placementDefinition, List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule> placementRules = null, List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint> placementConstraints = null)
        {
            SpatialUnderstandingDllTopology.TopologyResult[] resultsTopology = new SpatialUnderstandingDllTopology.TopologyResult[1];
            System.IntPtr resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsTopology);

            if
                (
                    SpatialUnderstandingDllObjectPlacement.Solver_PlaceObject
                    (
                        name,
                        SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(placementDefinition),
                        (placementRules != null) ? placementRules.Count : 0,
                        ((placementRules != null) && (placementRules.Count > 0)) ? SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(placementRules.ToArray()) : System.IntPtr.Zero,
                        (placementConstraints != null) ? placementConstraints.Count : 0,
                        ((placementConstraints != null) && (placementConstraints.Count > 0)) ? SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(placementConstraints.ToArray()) : System.IntPtr.Zero,
                        SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticObjectPlacementResultPtr()
                    ) > 0
                )
            {
                return true;
            }
            return false;
        }

        public void PlaceObjectOnFloor(GameObject g)
        {
            const int QueryResultMaxCount = 512;

            SpatialUnderstandingDllTopology.TopologyResult[] _resultsTopology = new SpatialUnderstandingDllTopology.TopologyResult[QueryResultMaxCount];

            var minLengthFloorSpace = 0.25f;
            var minWidthFloorSpace = 0.25f;

            var resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(_resultsTopology);
            var locationCount = SpatialUnderstandingDllTopology.QueryTopology_FindPositionsOnFloor(minLengthFloorSpace, minWidthFloorSpace, _resultsTopology.Length, resultsTopologyPtr);

            if (locationCount > 0)
            {
                g.transform.position = _resultsTopology[0].position;
                Debug.Log("Placed the hologram");
            }
            else
            {
                Debug.Log("I can't found the enough space to place the hologram.");
            }
        }

        public void PlaceObjectOnSittable(GameObject g)
        {
            const int QueryResultMaxCount = 512;

            SpatialUnderstandingDllTopology.TopologyResult[] _resultsTopology = new SpatialUnderstandingDllTopology.TopologyResult[QueryResultMaxCount];

            var minHeight = 0.3f;
            var maxHeight = 1f;

            var resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(_resultsTopology);
            var locationCount = SpatialUnderstandingDllTopology.QueryTopology_FindPositionsSittable(minHeight, maxHeight, 0.5f, _resultsTopology.Length, resultsTopologyPtr);

            if (locationCount > 0)
            {
                g.transform.position = _resultsTopology[0].position;
                Debug.Log("Placed the hologram");
            }
            else
            {
                Debug.Log("I can't found the enough space to place the hologram.");
            }
        }


        public NavMeshSurface navMeshSurface;
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

        private void OnDestroy()
        {
            SpatialUnderstanding.Instance.OnScanDone -= Instance_OnScanDone;
            SpatialMappingManager.Instance.SurfaceObserver.SurfaceUpdated -= SurfaceObserver_SurfaceUpdated;
            SpatialUnderstanding.Instance.ScanStateChanged -= Instance_ScanStateChanged;
        }
    }
}
