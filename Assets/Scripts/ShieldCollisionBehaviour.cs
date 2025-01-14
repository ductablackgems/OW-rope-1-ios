using UnityEngine;

public class ShieldCollisionBehaviour : MonoBehaviour
{
	public GameObject EffectOnHit;

	public GameObject ExplosionOnHit;

	public bool IsWaterInstance;

	public float ScaleWave = 0.89f;

	public bool CreateMechInstanceOnHit;

	public Vector3 AngleFix;

	public int currentQueue = 3001;

	public void ShieldCollisionEnter(CollisionInfo e)
	{
		if (!(e.Hit.transform != null))
		{
			return;
		}
		if (IsWaterInstance)
		{
			Transform transform = UnityEngine.Object.Instantiate(ExplosionOnHit).transform;
			transform.parent = base.transform;
			float num = base.transform.localScale.x * ScaleWave;
			transform.localScale = new Vector3(num, num, num);
			transform.localPosition = new Vector3(0f, 0.001f, 0f);
			transform.LookAt(e.Hit.point);
			return;
		}
		if (EffectOnHit != null)
		{
			if (!CreateMechInstanceOnHit)
			{
				Renderer componentInChildren = e.Hit.transform.GetComponentInChildren<Renderer>();
				GameObject gameObject = UnityEngine.Object.Instantiate(EffectOnHit);
				gameObject.transform.parent = componentInChildren.transform;
				gameObject.transform.localPosition = Vector3.zero;
				AddMaterialOnHit component = gameObject.GetComponent<AddMaterialOnHit>();
				component.SetMaterialQueue(currentQueue);
				component.UpdateMaterial(e.Hit);
			}
			else
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(EffectOnHit);
				Transform transform2 = gameObject2.transform;
				transform2.parent = GetComponent<Renderer>().transform;
				transform2.localPosition = Vector3.zero;
				transform2.localScale = base.transform.localScale * ScaleWave;
				transform2.LookAt(e.Hit.point);
				transform2.Rotate(AngleFix);
				gameObject2.GetComponent<Renderer>().material.renderQueue = currentQueue - 1000;
			}
		}
		if (currentQueue > 4000)
		{
			currentQueue = 3001;
		}
		else
		{
			currentQueue++;
		}
		if (ExplosionOnHit != null)
		{
			Object.Instantiate(ExplosionOnHit, e.Hit.point, default(Quaternion)).transform.parent = base.transform;
		}
	}

	private void Update()
	{
	}
}
