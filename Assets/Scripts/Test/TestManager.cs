using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

namespace Thesis
{
    public class TestManager : MonoBehaviour
    {

        public GameObject character;

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
    }
}