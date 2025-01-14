using UnityEngine;

public class ColliderTester : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		UnityEngine.Debug.Log("Enter " + other.name);
	}

	private void OnTriggerStay(Collider other)
	{
		UnityEngine.Debug.Log("Stay " + other.name);
	}
}
