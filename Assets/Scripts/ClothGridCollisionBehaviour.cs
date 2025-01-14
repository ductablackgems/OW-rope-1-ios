using System;
using UnityEngine;

public class ClothGridCollisionBehaviour : MonoBehaviour
{
	public GameObject[] AttachedPoints;

	public bool IsLookAt;

	private EffectSettings effectSettings;

	private Transform tRoot;

	private Transform tTarget;

	private Vector3 targetPos;

	private bool onCollision;

	public event EventHandler<CollisionInfo> OnCollision;

	private void GetEffectSettingsComponent(Transform tr)
	{
		Transform parent = tr.parent;
		if (parent != null)
		{
			effectSettings = parent.GetComponentInChildren<EffectSettings>();
			if (effectSettings == null)
			{
				GetEffectSettingsComponent(parent.transform);
			}
		}
	}

	private void Start()
	{
		GetEffectSettingsComponent(base.transform);
		if (effectSettings == null)
		{
			UnityEngine.Debug.Log("Prefab root have not script \"PrefabSettings\"");
		}
		tRoot = effectSettings.transform;
		InitDefaultVariables();
	}

	private void InitDefaultVariables()
	{
		tTarget = effectSettings.Target.transform;
		if (IsLookAt)
		{
			tRoot.LookAt(tTarget);
		}
		Vector3 vector = CenterPoint();
		targetPos = vector + Vector3.Normalize(tTarget.position - vector) * effectSettings.MoveDistance;
		Vector3 force = Vector3.Normalize(tTarget.position - vector) * effectSettings.MoveSpeed / 100f;
		for (int i = 0; i < AttachedPoints.Length; i++)
		{
			GameObject obj = AttachedPoints[i];
			Rigidbody component = obj.GetComponent<Rigidbody>();
			component.useGravity = false;
			component.AddForce(force, ForceMode.Impulse);
			obj.SetActive(value: true);
		}
	}

	private Vector3 CenterPoint()
	{
		return (base.transform.TransformPoint(AttachedPoints[0].transform.localPosition) + base.transform.TransformPoint(AttachedPoints[2].transform.localPosition)) / 2f;
	}

	private void Update()
	{
		if (!(tTarget == null) && !onCollision)
		{
			Vector3 vector = CenterPoint();
			RaycastHit hitInfo = default(RaycastHit);
			float num = Vector3.Distance(vector, targetPos);
			UnityEngine.Debug.DrawLine(vector, targetPos);
			float num2 = effectSettings.MoveSpeed * Time.deltaTime;
			if (num2 > num)
			{
				num2 = num;
			}
			if (num <= effectSettings.ColliderRadius)
			{
				DeactivateAttachedPoints(hitInfo);
			}
			Vector3 normalized = (targetPos - vector).normalized;
			if (Physics.Raycast(vector, normalized, out hitInfo, num2 + effectSettings.ColliderRadius))
			{
				targetPos = hitInfo.point - normalized * effectSettings.ColliderRadius;
				DeactivateAttachedPoints(hitInfo);
			}
		}
	}

	private void DeactivateAttachedPoints(RaycastHit hit)
	{
		for (int i = 0; i < AttachedPoints.Length; i++)
		{
			AttachedPoints[i].SetActive(value: false);
		}
		CollisionInfo e = new CollisionInfo
		{
			Hit = hit
		};
		effectSettings.OnCollisionHandler(e);
		if (hit.transform != null)
		{
			ShieldCollisionBehaviour component = hit.transform.GetComponent<ShieldCollisionBehaviour>();
			if (component != null)
			{
				component.ShieldCollisionEnter(e);
			}
		}
		onCollision = true;
	}
}
