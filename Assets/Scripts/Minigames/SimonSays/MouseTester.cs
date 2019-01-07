using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTester : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                var s = hitInfo.collider.gameObject.GetComponent<SimonBlock>();
                if (s) s.Select();
            }
        }
    }
}
