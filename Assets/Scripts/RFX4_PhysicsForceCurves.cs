using UnityEngine;

public class RFX4_PhysicsForceCurves : MonoBehaviour
{
	public float ForceRadius = 5f;

	public float ForceMultiplier = 1f;

	public AnimationCurve ForceCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	public ForceMode ForceMode;

	public float GraphTimeMultiplier = 1f;

	public float GraphIntensityMultiplier = 1f;

	public bool IsLoop;

	public float DestoryDistance = -1f;

	public bool UseDistanceScale;

	public AnimationCurve DistanceScaleCurve = AnimationCurve.EaseInOut(1f, 1f, 1f, 1f);

	public bool UseUPVector;

	public AnimationCurve DragCurve = AnimationCurve.EaseInOut(0f, 0f, 0f, 1f);

	public float DragGraphTimeMultiplier = -1f;

	public float DragGraphIntensityMultiplier = -1f;

	public string AffectedName;

	[HideInInspector]
	public float forceAdditionalMultiplier = 1f;

	private bool canUpdate;

	private float startTime;

	private Transform t;

	private void Awake()
	{
		t = base.transform;
	}

	private void OnEnable()
	{
		startTime = Time.time;
		canUpdate = true;
		forceAdditionalMultiplier = 1f;
	}

	private void FixedUpdate()
	{
		float num = Time.time - startTime;
		if (canUpdate)
		{
			float d = ForceCurve.Evaluate(num / GraphTimeMultiplier) * GraphIntensityMultiplier;
			Collider[] array = Physics.OverlapSphere(t.position, ForceRadius);
			foreach (Collider collider in array)
			{
				Rigidbody component = collider.GetComponent<Rigidbody>();
				if (!(component == null) && (AffectedName.Length <= 0 || collider.name.Contains(AffectedName)))
				{
					Vector3 vector;
					float num2;
					if (UseUPVector)
					{
						vector = Vector3.up;
						Vector3 position = collider.transform.position;
						num2 = 1f - Mathf.Clamp01(position.y - t.position.y);
						num2 *= 1f - (collider.transform.position - t.position).magnitude / ForceRadius;
					}
					else
					{
						vector = collider.transform.position - t.position;
						num2 = 1f - vector.magnitude / ForceRadius;
					}
					if (UseDistanceScale)
					{
						collider.transform.localScale = DistanceScaleCurve.Evaluate(num2) * collider.transform.localScale;
					}
					if (DestoryDistance > 0f && vector.magnitude < DestoryDistance)
					{
						UnityEngine.Object.Destroy(collider.gameObject);
					}
					component.AddForce(vector.normalized * num2 * ForceMultiplier * d * forceAdditionalMultiplier, ForceMode);
					if (DragGraphTimeMultiplier > 0f)
					{
						component.drag = DragCurve.Evaluate(num / DragGraphTimeMultiplier) * DragGraphIntensityMultiplier;
						component.angularDrag = component.drag / 10f;
					}
				}
			}
		}
		if (num >= GraphTimeMultiplier)
		{
			if (IsLoop)
			{
				startTime = Time.time;
			}
			else
			{
				canUpdate = false;
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, ForceRadius);
	}
}
