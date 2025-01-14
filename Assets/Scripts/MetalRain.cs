using System.Collections;
using UnityEngine;

public class MetalRain : MonoBehaviour
{
	public GameObject RainObject;

	private ImpactDeformable impactDeformable;

	private void Start()
	{
		StartCoroutine(Rain());
		impactDeformable = GetComponent<ImpactDeformable>();
	}

	private IEnumerator Rain()
	{
		while (true)
		{
			Object.Instantiate(RainObject, new Vector3((UnityEngine.Random.value - 0.5f) * 20f, UnityEngine.Random.value * 40f + 10f, (UnityEngine.Random.value - 0.5f) * 20f), Quaternion.identity);
			yield return new WaitForSeconds(UnityEngine.Random.value * 0.2f + 0.1f);
		}
	}

	private void OnMouseDown()
	{
		impactDeformable.Repair(0.25f);
	}

	public void SetHardness(float value)
	{
		impactDeformable.Hardness = value;
	}

	public void SetDeformMeshCollider(bool fd)
	{
		impactDeformable.DeformMeshCollider = fd;
	}
}
