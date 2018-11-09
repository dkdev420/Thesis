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
        [SerializeField] string storedMeshFilename;
        [SerializeField] CachedSpatialMapping cachedSpatialMapping;

        public bool IsSolverInitialized { get; private set; }

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

        public NavMeshSurface navMeshSurface;
        [ContextMenu("Build NavMesh")]
        public void BuildNavMesh()
        {
            if (navMeshSurface) navMeshSurface.BuildNavMesh();
        }

        [ContextMenu("Start")]
        public void StartScan() { SpatialUnderstanding.Instance.RequestBeginScanning(); }

        [ContextMenu("End")]
        public void EndScan() { SpatialUnderstanding.Instance.RequestFinishScan(); }

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
