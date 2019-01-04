using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallThrower : MonoBehaviour
{
    public GameObject ball;
    private float force = 1000f;

    void Start ()
	{
        Time.timeScale = .75f;
	}
	
	void Update ()
	{
        Vector3 relativePos = Camera.main.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = rotation;
    }

    IEnumerator ThrowCoro()
    {
        while(true)
        {
            GameObject clone = Instantiate(ball, transform.position, Quaternion.identity);
            clone.SetActive(true);
            Rigidbody r = clone.GetComponent<Rigidbody>();
            r.AddForce(transform.forward * force, ForceMode.Acceleration);
            yield return new WaitForSeconds(1f);
        }
    }

    public void StartThrowing()
    {
        StartCoroutine(ThrowCoro());
    }
}
