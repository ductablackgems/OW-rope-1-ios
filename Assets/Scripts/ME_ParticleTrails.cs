using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ME_ParticleTrails : MonoBehaviour
{
	public GameObject TrailPrefab;

	private ParticleSystem ps;

	private ParticleSystem.Particle[] particles;

	private Dictionary<uint, GameObject> hashTrails = new Dictionary<uint, GameObject>();

	private void Start()
	{
		ps = GetComponent<ParticleSystem>();
		particles = new ParticleSystem.Particle[ps.main.maxParticles];
	}

	private void OnEnable()
	{
		InvokeRepeating("ClearEmptyHashes", 1f, 1f);
	}

	private void OnDisable()
	{
		CancelInvoke("ClearEmptyHashes");
	}

	private void Update()
	{
		UpdateTrail();
	}

	private void UpdateTrail()
	{
		int num = ps.GetParticles(particles);
		for (int i = 0; i < num; i++)
		{
			if (!hashTrails.ContainsKey(particles[i].randomSeed))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(TrailPrefab, base.transform.position, default(Quaternion));
				gameObject.hideFlags = HideFlags.HideInHierarchy;
				hashTrails.Add(particles[i].randomSeed, gameObject);
				gameObject.GetComponent<LineRenderer>().widthMultiplier *= particles[i].startSize;
				continue;
			}
			GameObject gameObject2 = hashTrails[particles[i].randomSeed];
			if (gameObject2 != null)
			{
				LineRenderer component = gameObject2.GetComponent<LineRenderer>();
				component.startColor *= (Color)particles[i].GetCurrentColor(ps);
				component.endColor *= (Color)particles[i].GetCurrentColor(ps);
				if (ps.main.simulationSpace == ParticleSystemSimulationSpace.World)
				{
					gameObject2.transform.position = particles[i].position;
				}
				if (ps.main.simulationSpace == ParticleSystemSimulationSpace.Local)
				{
					gameObject2.transform.position = ps.transform.TransformPoint(particles[i].position);
				}
			}
		}
	}

	private void ClearEmptyHashes()
	{
		hashTrails = (from h in hashTrails
			where h.Value != null
			select h).ToDictionary((KeyValuePair<uint, GameObject> h) => h.Key, (KeyValuePair<uint, GameObject> h) => h.Value);
	}
}
