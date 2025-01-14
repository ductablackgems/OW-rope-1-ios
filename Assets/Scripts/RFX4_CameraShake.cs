using System.Collections;
using UnityEngine;

public class RFX4_CameraShake : MonoBehaviour
{
	public AnimationCurve ShakeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

	public float Duration = 2f;

	public float Speed = 22f;

	public float Magnitude = 1f;

	public float DistanceForce = 100f;

	public float RotationDamper = 2f;

	public bool IsEnabled = true;

	private bool isPlaying;

	[HideInInspector]
	public bool canUpdate;

	private void PlayShake()
	{
		StopAllCoroutines();
		StartCoroutine(Shake());
	}

	private void Update()
	{
		if (isPlaying && IsEnabled)
		{
			isPlaying = false;
			PlayShake();
		}
	}

	private void OnEnable()
	{
		isPlaying = true;
		RFX4_CameraShake[] array = UnityEngine.Object.FindObjectsOfType(typeof(RFX4_CameraShake)) as RFX4_CameraShake[];
		if (array != null)
		{
			RFX4_CameraShake[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].canUpdate = false;
			}
		}
		canUpdate = true;
	}

	private IEnumerator Shake()
	{
		float elapsed = 0f;
		Transform camT = Camera.main.transform;
		Vector3 originalCamRotation = camT.rotation.eulerAngles;
		Vector3 direction = (base.transform.position - camT.position).normalized;
		float time = 0f;
		float randomStart = UnityEngine.Random.Range(-1000f, 1000f);
		float distanceDamper = 1f - Mathf.Clamp01((camT.position - base.transform.position).magnitude / DistanceForce);
		Vector3 oldRotation = Vector3.zero;
		while (elapsed < Duration && canUpdate)
		{
			elapsed += Time.deltaTime;
			float num = elapsed / Duration;
			float num2 = ShakeCurve.Evaluate(num) * distanceDamper;
			time += Time.deltaTime * num2;
			camT.position -= direction * Time.deltaTime * Mathf.Sin(time * Speed) * num2 * Magnitude / 2f;
			float num3 = randomStart + Speed * num / 10f;
			float num4 = Mathf.PerlinNoise(num3, 0f) * 2f - 1f;
			float num5 = Mathf.PerlinNoise(1000f + num3, num3 + 1000f) * 2f - 1f;
			float num6 = Mathf.PerlinNoise(0f, num3) * 2f - 1f;
			if (Quaternion.Euler(originalCamRotation + oldRotation) != camT.rotation)
			{
				originalCamRotation = camT.rotation.eulerAngles;
			}
			oldRotation = Mathf.Sin(time * Speed) * num2 * Magnitude * new Vector3(0.5f + num5, 0.3f + num4, 0.3f + num6) * RotationDamper;
			camT.rotation = Quaternion.Euler(originalCamRotation + oldRotation);
			yield return null;
		}
	}
}
