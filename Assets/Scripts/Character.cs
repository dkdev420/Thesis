using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Thesis
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Character : MonoBehaviour
    {
        [SerializeField] Transform dummyFollow;

        NavMeshAgent agent;
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        bool onNav = false;
        void Update()
        {
            //FIXME: Place object on navmesh on startup
            if(!onNav)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(transform.position, out hit, 100f, NavMesh.AllAreas))
                {
                    transform.position = hit.position;
                    onNav = true;
                }
                return;
            }
            //--

            if(dummyFollow) agent.SetDestination(dummyFollow.position);
            else agent.SetDestination(Cursor.Instance.transform.position);
            RaycastHit hitInfo;
            if(Physics.Raycast(transform.position, -transform.up, out hitInfo))
            {
                agent.baseOffset = hitInfo.point.y - transform.position.y;
            }
        }
    }
}