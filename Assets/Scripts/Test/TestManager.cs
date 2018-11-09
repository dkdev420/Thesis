using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

namespace Thesis
{
    public class TestManager : MonoBehaviour
    {
        public GameObject character;
        public GameObject platform;

        [ContextMenu("Place character On Floor")]
        public void PlaceCharacterOnFloor()
        {
            Room.Instance.PlaceGameObject
            (
                character,
                SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnFloor(Vector3.one * .05f)
            );
            character.SetActive(true);
        }

        [ContextMenu("Place character Edge")]
        public void PlaceCharacterOnEdge()
        {
            Room.Instance.PlaceGameObject
            (
                character,
                SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnEdge(Vector3.one * .05f, Vector3.one * .05f)
            );
            character.SetActive(true);
        }

        [ContextMenu("Place Platform")]
        public void PlacePlatform()
        {
            Debug.Log(platform.transform.localScale);
            Room.Instance.PlaceGameObject
            (
                platform,
                SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_UnderPlatformEdge(Vector3.one * 0.01f)
            );
            platform.SetActive(true);
        }
    }
}