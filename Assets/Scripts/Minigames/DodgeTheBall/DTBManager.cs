using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DTBManager : MonoBehaviour
{
    public GameObject UI;
    public GameObject cursor;
    public BallThrower thrower;

    public float distance;

    void Start ()
	{
		
	}
	
	void Update ()
	{
		
	}

    public void StartGame()
    {
        thrower.transform.position = Camera.main.transform.position + Camera.main.transform.forward * distance;
        thrower.transform.forward = (Camera.main.transform.position - thrower.transform.position).normalized;
        UI.SetActive(false);
        cursor.SetActive(false);
        thrower.StartThrowing();
    }
}
