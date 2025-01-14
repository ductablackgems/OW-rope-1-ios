using UnityEngine;

public class DebuffOnEnemyFromCollision : MonoBehaviour
{
	public EffectSettings EffectSettings;

	public GameObject Effect;

	private void Start()
	{
		EffectSettings.CollisionEnter += EffectSettings_CollisionEnter;
	}

	private void EffectSettings_CollisionEnter(object sender, CollisionInfo e)
	{
		if (!(Effect == null))
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, EffectSettings.EffectRadius, EffectSettings.LayerMask);
			foreach (Collider collider in array)
			{
				Renderer componentInChildren = collider.transform.GetComponentInChildren<Renderer>();
				GameObject gameObject = UnityEngine.Object.Instantiate(Effect);
				gameObject.transform.parent = componentInChildren.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.GetComponent<AddMaterialOnHit>().UpdateMaterial(collider.transform);
			}
		}
	}

	private void Update()
	{
	}
}
