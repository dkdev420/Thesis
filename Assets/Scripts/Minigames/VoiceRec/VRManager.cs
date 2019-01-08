using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRManager : MonoBehaviour
{
    public SimonBlock r, g, b;

	void Start ()
	{
        Thesis.HoloInputController.Instance.AddKeywords(new Dictionary<string, System.Action>
        {
            { "red", () => { r.PlaySound();  } },
            { "blue", () => { g.PlaySound(); } },
            { "green", () => { b.PlaySound(); } },
        });
	}

}
