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
        const int QueryResultMaxCount = 512;
        const int DisplayResultMaxCount = 32;

        [SerializeField] private float minAreaForComplete;
        [SerializeField] private float minHorizAreaForComplete;
        [SerializeField] private float minWallAreaForComplete;
        [SerializeField] string storedMeshFilename;
        [SerializeField] CachedSpatialMapping cachedSpatialMapping;

        private SpatialUnderstandingDllTopology.TopologyResult[] resultsTopology = new SpatialUnderstandingDllTopology.TopologyResult[QueryResultMaxCount];

        public bool IsSolverInitialized { get; private set; }
        public bool IsReady
        {
            get
            {
                return SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done
                       && IsSolverInitialized;
            }
        }

        public bool MinSurfaceReached
        {
            get
            {
                if ((SpatialUnderstanding.Instance.ScanState != SpatialUnderstanding.ScanStates.Scanning) || (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)) return false;

                System.IntPtr statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
                if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) == 0) return false;
                SpatialUnderstandingDll.Imports.PlayspaceStats stats = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();

                if ((stats.TotalSurfaceArea > minAreaForComplete) || (stats.HorizSurfaceArea > minHorizAreaForComplete) || (stats.WallSurfaceArea > minWallAreaForComplete)) return true;
                return false;
            }
        }


        void Start()
        {
            SpatialUnderstanding.Instance.OnScanDone += Instance_OnScanDone;
            SpatialMappingManager.Instance.SurfaceObserver.SurfaceUpdated += SurfaceObserver_SurfaceUpdated;
        }

        private void Update()
        {
            if (SpatialUnderstanding.Instance.ScanState != SpatialUnderstanding.ScanStates.Done) return;
            if (!IsSolverInitialized && SpatialUnderstanding.Instance.AllowSpatialUnderstanding) InitializeSolver();
        }

        public bool InitializeSolver()
        {
            if (IsSolverInitialized || !SpatialUnderstanding.Instance.AllowSpatialUnderstanding) return IsSolverInitialized;
            if (SpatialUnderstandingDllObjectPlacement.Solver_Init() == 1) IsSolverInitialized = true;
            return IsSolverInitialized;
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

        public void PlaceGameObject(GameObject g, SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placementDefinition, List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule> placementRules = null, List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint> placementConstraints = null)
        {
            StartCoroutine(PlaceGameObjectCoro(g, placementDefinition, placementRules, placementConstraints));
        }

        IEnumerator PlaceGameObjectCoro(GameObject g, SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placementDefinition, List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule> placementRules = null, List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint> placementConstraints = null)
        {
            string name = g.name;
            bool hasResults = false;
            var thread =
#if UNITY_EDITOR || !UNITY_WSA
            new System.Threading.Thread
#else
            System.Threading.Tasks.Task.Run
#endif
            (() =>
            {
                hasResults = _PlaceGameObject(name, placementDefinition, placementRules, placementConstraints);
            });
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

            if (hasResults)
            {
                SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult placementResult = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticObjectPlacementResult();
                g.transform.position = placementResult.Position;
            }
            else Debug.Log("No placements found");
        }

        private bool _PlaceGameObject(string name, SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placementDefinition, List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule> placementRules = null, List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint> placementConstraints = null)
        {
            return
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
                );
        }

        [ContextMenu("Find Largest Floor")]
        public void PlaceOnLargestFloor()
        {
            System.IntPtr resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsTopology);
            int locationCount = SpatialUnderstandingDllTopology.QueryTopology_FindLargestPositionsOnFloor(resultsTopology.Length, resultsTopologyPtr);

            for(int i=0; i < locationCount; i++)
            {
                Debug.Log(resultsTopology[i].position);
            }
        }

        [ContextMenu("Find Largest Floor")]
        public void PlaceOnSittable()
        {
            System.IntPtr resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsTopology);
            int locationCount = SpatialUnderstandingDllTopology.QueryTopology_FindPositionsSittable(.4f, 1.5f, 1f, resultsTopology.Length, resultsTopologyPtr);

            for (int i = 0; i < locationCount; i++)
            {
                Debug.Log(resultsTopology[i].position);
            }
        }

        public NavMeshSurface navMeshSurface;
        [ContextMenu("Build NavMesh")]
        public void BuildNavMesh()
        {
            if (navMeshSurface) navMeshSurface.BuildNavMesh();
        }

        [ContextMenu("Start Mapping")]
        public void StartMappingScan() { SpatialMappingManager.Instance.StartObserver(); }

        [ContextMenu("Start Understanding")]
        public void StartUnderstandingScan() { SpatialUnderstanding.Instance.RequestBeginScanning(); }

        [ContextMenu("Stop Understanding")]
        public void EndUnderstandingScan() { SpatialUnderstanding.Instance.RequestFinishScan(); }

        [ContextMenu("SaveSpatialMeshes")]
        public void SaveSpatialMeshes()
        {
            if(string.IsNullOrEmpty(storedMeshFilename))
            {
                Debug.LogError("Mesh filename cannot be empty");
                return;
            }
            SpatialMappingManager.Instance.SurfaceObserver.SaveSpatialMeshes(storedMeshFilename);
        }

        [ContextMenu("LoadSpatialMeshes")]
        public void LoadSpatialMeshes()
        {
            if (string.IsNullOrEmpty(storedMeshFilename))
            {
                Debug.Log("No mesh file specified.");
                return;
            }
            try
            {
                SpatialMappingManager.Instance.SetSpatialMappingSource(cachedSpatialMapping);
                cachedSpatialMapping.Load(MeshSaver.Load(storedMeshFilename));
            }
            catch
            {
                Debug.Log("Failed to load " + storedMeshFilename);
            }
        }

        private void OnDestroy()
        {
            SpatialUnderstanding.Instance.OnScanDone -= Instance_OnScanDone;
            SpatialMappingManager.Instance.SurfaceObserver.SurfaceUpdated -= SurfaceObserver_SurfaceUpdated;
         }
    }
}
