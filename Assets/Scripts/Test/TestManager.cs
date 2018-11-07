using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

namespace Thesis
{
    public class TestManager : MonoBehaviour
    {

        public GameObject character;

        void Start()
        {

        }

        void Update()
        {

        }

        [ContextMenu("Place character")]
        public void PlaceCharacter()
        {
            /*
            StartCoroutine(Room.Instance.PlaceGameObject
            (
                character,
                SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnFloor(Vector3.one * .05f)
            ));
            */
            Room.Instance.PlaceObjectOnSittable(character);
            character.SetActive(true);
        }
    }
}