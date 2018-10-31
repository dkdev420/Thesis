using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thesis;
using UnityEngine.UI;

public class StupidTest : MonoBehaviour
{

    [SerializeField] GameObject o;

    public Text t;

    public SpatialUnderstanding.ScanStates s;

    // Use this for initialization
    void Start()
    {
        SpatialUnderstanding.Instance.RequestBeginScanning();
        HoloInputController.Instance.InteractionSourcePressed += Instance_InteractionSourcePressed;
    }

    private void Instance_InteractionSourcePressed(UnityEngine.XR.WSA.Input.InteractionSourcePressedEventArgs obj)
    {
        if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.ReadyToScan) StartScan();
        else if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Scanning) EndScan();
        else if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.ReadyToScan) StartScan();
    }

    private void OnDestroy()
    {
    }

    // Update is called once per frame
    void Update()
    {
        s = SpatialUnderstanding.Instance.ScanState;
        t.text = SpatialUnderstanding.Instance.ScanState.ToString();
        switch (SpatialUnderstanding.Instance.ScanState)
        {
            case SpatialUnderstanding.ScanStates.None:
            case SpatialUnderstanding.ScanStates.ReadyToScan:
                break;
            case SpatialUnderstanding.ScanStates.Scanning:
                break;
            case SpatialUnderstanding.ScanStates.Finishing:
                Debug.Log("State: Finishing Scan");
                break;
            case SpatialUnderstanding.ScanStates.Done:
                InstanciateObjectOnFloor();
                Debug.Log("State: Scan Finished");
                break;
            default:
                break;
        }
    }

    private void InstanciateObjectOnFloor()
    {
        const int QueryResultMaxCount = 512;

        SpatialUnderstandingDllTopology.TopologyResult[] _resultsTopology = new SpatialUnderstandingDllTopology.TopologyResult[QueryResultMaxCount];

        var minLengthFloorSpace = 0.25f;
        var minWidthFloorSpace = 0.25f;

        var resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(_resultsTopology);
        var locationCount = SpatialUnderstandingDllTopology.QueryTopology_FindPositionsOnFloor(minLengthFloorSpace, minWidthFloorSpace, _resultsTopology.Length, resultsTopologyPtr);

        if (locationCount > 0)
        {
            o.transform.position = _resultsTopology[(int)(locationCount / 2)].position;
            //o.transform.up = _resultsTopology[0].normal;

            Debug.Log("Placed the hologram");
        }
        else
        {
            Debug.Log("I can't found the enough space to place the hologram.");
        }
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
