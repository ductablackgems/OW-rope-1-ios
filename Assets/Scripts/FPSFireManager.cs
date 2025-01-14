using System;
using UnityEngine;

public class FPSFireManager : MonoBehaviour
{
	[Serializable]
	public class ImpactInfo
	{
		public MaterialType.MaterialTypeEnum MaterialType;

		public GameObject ImpactEffect;
	}

	public ImpactInfo[] ImpactElemets = new ImpactInfo[0];

	public float BulletDistance = 100f;

	public GameObject ImpactEffect;

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && Physics.Raycast(new Ray(base.transform.position, base.transform.forward), out RaycastHit hitInfo, BulletDistance))
		{
			GameObject impactEffect = GetImpactEffect(hitInfo.transform.gameObject);
			if (!(impactEffect == null))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(impactEffect, hitInfo.point, default(Quaternion));
				ImpactEffect.SetActive(value: false);
				ImpactEffect.SetActive(value: true);
				gameObject.transform.LookAt(hitInfo.point + hitInfo.normal);
				UnityEngine.Object.Destroy(gameObject, 4f);
			}
		}
	}

	private GameObject GetImpactEffect(GameObject impactedGameObject)
	{
		MaterialType component = impactedGameObject.GetComponent<MaterialType>();
		if (component == null)
		{
			return null;
		}
		ImpactInfo[] impactElemets = ImpactElemets;
		foreach (ImpactInfo impactInfo in impactElemets)
		{
			if (impactInfo.MaterialType == component.TypeOfMaterial)
			{
				return impactInfo.ImpactEffect;
			}
		}
		return null;
	}
}
