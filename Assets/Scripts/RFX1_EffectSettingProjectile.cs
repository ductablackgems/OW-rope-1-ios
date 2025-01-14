using UnityEngine;

public class RFX1_EffectSettingProjectile : MonoBehaviour
{
	public float FlyDistanceForProjectiles = 30f;

	public float SpeedMultiplier = 1f;

	public LayerMask CollidesWith = -1;

	private float startSpeed;

	private float oldSpeedMultiplier;

	private void Awake()
	{
		oldSpeedMultiplier = SpeedMultiplier;
		RFX1_TransformMotion componentInChildren = GetComponentInChildren<RFX1_TransformMotion>(includeInactive: true);
		if (componentInChildren != null)
		{
			startSpeed = componentInChildren.Speed;
		}
	}

	private void OnEnable()
	{
		RFX1_TransformMotion componentInChildren = GetComponentInChildren<RFX1_TransformMotion>(includeInactive: true);
		if (componentInChildren != null)
		{
			componentInChildren.Distance = FlyDistanceForProjectiles;
			componentInChildren.CollidesWith = CollidesWith;
			componentInChildren.Speed = startSpeed * SpeedMultiplier;
		}
	}

	private void Update()
	{
		if (Mathf.Abs(oldSpeedMultiplier - SpeedMultiplier) > 0.001f)
		{
			OnEnable();
		}
	}
}
