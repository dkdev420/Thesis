﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
	IEnumerator Start ()
	{
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
	}
}
