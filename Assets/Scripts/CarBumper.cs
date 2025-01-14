using UnityEngine;

public class CarBumper : MonoBehaviour
{
	private void Awake()
	{
		GetComponent<ImpactDeformable>().OnDeform += CarBumper_OnDeform;
	}

	public void OnDisable()
	{
		GetComponent<ImpactDeformable>().OnDeform -= CarBumper_OnDeform;
	}

	private void CarBumper_OnDeform(ImpactDeformable deformable)
	{
		if (deformable.StructuralDamage > 0.1f)
		{
			base.transform.parent = base.transform.parent.parent;
			base.gameObject.AddComponent<Rigidbody>().mass = 0.1f;
			UnityEngine.Object.Destroy(this);
			UnityEngine.Object.Destroy(base.gameObject, 10f);
		}
	}
}
