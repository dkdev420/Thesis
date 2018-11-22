using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thesis
{
    public class RoomConfigurator : MonoBehaviour
    {
        IEnumerator Start()
        {
            yield return null;

            Room.Instance.StartMappingScan();
            yield return null;

            Room.Instance.StartUnderstandingScan();
        }
    }
}