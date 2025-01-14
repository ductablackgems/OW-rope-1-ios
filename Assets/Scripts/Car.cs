using System.Collections;
using System.Linq;
using UnityEngine;

public class Car : MonoBehaviour
{
	public Color Color;

	public Transform RTire;

	public Transform LTire;

	public Vector2 Control;

	public AudioSource EngineSound;

	public AudioSource DriftSound;

	public ParticleSystem Smoke;

	private ParticleSystem.EmissionModule smokeEmission;

	public ImpactDeformable CarHullDeformable;

	private float RPM;

	private Rigidbody body;

	private bool AI;

	private ImpactDeformable impactDeformable;

	private bool Broken;

	public float CarDamage => Mathf.Clamp01(CarHullDeformable.StructuralDamage / 0.065f);

	private void Awake()
	{
		body = GetComponent<Rigidbody>();
		StartCoroutine(StartVolume());
	}

	private void Start()
	{
		AI = (GetComponent<AIControl>() != null);
		Mesh mesh = CarHullDeformable.GetComponent<MeshFilter>().mesh;
		mesh.colors = (from v in mesh.vertices
			select Color).ToArray();
		CarHullDeformable.SetCurrentVertexColorsAsOriginal();
		smokeEmission = Smoke.emission;
	}

	private IEnumerator StartVolume()
	{
		while (EngineSound.volume < 1f)
		{
			EngineSound.volume += Time.deltaTime * 0.5f;
			yield return null;
		}
	}

	public void FixedUpdate()
	{
		float magnitude = body.velocity.magnitude;
		float magnitude2 = body.angularVelocity.magnitude;
		if (Broken)
		{
			Control = Vector3.zero;
		}
		if (base.transform.position.y < 0.1f)
		{
			body.AddForce(base.transform.forward * Control.y * 20f);
			if (magnitude2 < 2f)
			{
				float num = Control.x;
				if (Control.y < 0f)
				{
					num *= -1f;
				}
				body.AddTorque(base.transform.up * num * 20f);
			}
		}
		if (magnitude2 < 0.1f && magnitude < 0.1f)
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			if (eulerAngles.x > 180f)
			{
				eulerAngles.x -= 360f;
			}
			if (eulerAngles.z > 180f)
			{
				eulerAngles.z -= 360f;
			}
			eulerAngles.x = Mathf.Abs(eulerAngles.x);
			eulerAngles.z = Mathf.Abs(eulerAngles.z);
			if (eulerAngles.x > 1f || eulerAngles.z > 1f)
			{
				Invoke("UnturnCar", 2f);
			}
		}
		Vector3 localEulerAngles = new Vector3(0f, Control.x * 45f, 0f);
		LTire.transform.localEulerAngles = localEulerAngles;
		RTire.transform.localEulerAngles = localEulerAngles;
	}

	private void UnturnCar()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		base.transform.eulerAngles = eulerAngles;
	}

	private void Update()
	{
		ProcessEngineSound();
		if (!AI)
		{
			ProcessDriftSound();
		}
		CheckDamage();
	}

	private void ProcessEngineSound()
	{
		float num = body.velocity.magnitude / 20f;
		if (Control.y != 0f)
		{
			num += 0.25f;
			num *= 2f;
		}
		num += 0.2f;
		if (Broken)
		{
			num = 0f;
		}
		RPM = Mathf.Lerp(RPM, num, Time.deltaTime * 3f);
		EngineSound.pitch = RPM;
	}

	private void ProcessDriftSound()
	{
		float num = Mathf.Abs(body.angularVelocity.y);
		if (num >= 1f && !DriftSound.isPlaying)
		{
			DriftSound.Play();
		}
		if (num < 1f && DriftSound.isPlaying)
		{
			DriftSound.Stop();
		}
		if (DriftSound.isPlaying)
		{
			DriftSound.volume = num * 0.5f;
		}
	}

	private void CheckDamage()
	{
		Broken = (CarDamage >= 1f);
		smokeEmission.enabled = Broken;
	}
}
