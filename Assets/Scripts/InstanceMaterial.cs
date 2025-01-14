using UnityEngine;

public class InstanceMaterial : MonoBehaviour
{
	public Material Material;

	private void Start()
	{
		GetComponent<Renderer>().material = Material;
	}

	private void Update()
	{
	}
}
