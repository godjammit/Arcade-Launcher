using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
	public float Speed = 10f;

	private void Update()
	{
		this.transform.rotation *= Quaternion.Euler(0f, 0f, Speed * Time.deltaTime);
	}
}
