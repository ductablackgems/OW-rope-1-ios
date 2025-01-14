using UnityEngine;

[ExecuteInEditMode]
public class RFX1_ParticleGravityPoint : MonoBehaviour
{
	public Transform target;

	public float Force = 1f;

	public AnimationCurve ForceByTime = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);

	public float ForceLifeTime = 1f;

	private ParticleSystem ps;

	private ParticleSystem.Particle[] particles;

	private ParticleSystem.MainModule mainModule;

	private float startTime;

	private void Start()
	{
		ps = GetComponent<ParticleSystem>();
		mainModule = ps.main;
	}

	private void OnEnable()
	{
		startTime = Time.time;
	}

	private void LateUpdate()
	{
		int maxParticles = mainModule.maxParticles;
		if (particles == null || particles.Length < maxParticles)
		{
			particles = new ParticleSystem.Particle[maxParticles];
		}
		int num = ps.GetParticles(particles);
		float d = ForceByTime.Evaluate((Time.time - startTime) / ForceLifeTime) * Time.deltaTime * Force;
		Vector3 a = Vector3.zero;
		if (mainModule.simulationSpace == ParticleSystemSimulationSpace.Local)
		{
			a = base.transform.InverseTransformPoint(target.position);
		}
		if (mainModule.simulationSpace == ParticleSystemSimulationSpace.World)
		{
			a = target.position;
		}
		for (int i = 0; i < num; i++)
		{
			Vector3 vector = Vector3.Normalize(a - particles[i].position) * d;
			particles[i].velocity += vector;
		}
		ps.SetParticles(particles, num);
	}
}
