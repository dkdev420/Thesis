using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thesis
{
    public class Cursor : SingletonMonobehaviour<Cursor>
    {
        void Start()
        {
            HoloInputController.Instance.OnHit += Instance_OnHit;
            HoloInputController.Instance.OnTargetLost += Instance_OnTargetLost;
            HoloInputController.Instance.OnNoTarget += Instance_OnNoTarget;
        }

        void Update()
        {

        }

        private void Instance_OnTargetLost(TargetLostArgs obj)
        {
            transform.position = Camera.main.transform.position + Camera.main.transform.forward * 20.0f;
        }

        private void Instance_OnNoTarget(NoTargetArgs obj)
        {
            transform.position = Camera.main.transform.position + Camera.main.transform.forward * 20.0f;
        }

        private void Instance_OnHit(RaycastHit hit)
        {
            transform.position = hit.point;
            transform.forward = hit.normal;
        }

        void OnDestroy()
        {
            HoloInputController.Instance.OnHit -= Instance_OnHit;
            HoloInputController.Instance.OnTargetLost -= Instance_OnTargetLost;
            HoloInputController.Instance.OnNoTarget -= Instance_OnNoTarget;
        }
    }
}
