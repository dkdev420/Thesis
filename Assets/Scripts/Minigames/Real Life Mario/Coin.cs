using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] float distance = 5f;
    [SerializeField] float speed = 15f;
    IEnumerator Start ()
	{
        float startY = transform.position.y;
        while (transform.position.y < startY + distance)
        {
            transform.position += (-transform.forward * Time.deltaTime * speed);
            yield return 0;
        }
        Destroy(this.gameObject);
        yield break;
	}
}
