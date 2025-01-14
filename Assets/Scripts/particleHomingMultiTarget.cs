using UnityEngine;

[ExecuteInEditMode]
public class particleHomingMultiTarget : MonoBehaviour
{
	public enum TSOP
	{
		random,
		closest
	}

	[Tooltip("Target objects. If this parameter is undefined it will assume the attached object itself which creates self chasing particle effect.")]
	public Transform[] target;

	[Tooltip("How fast the particle is guided to the index target.")]
	public float speed = 10f;

	[Tooltip("Cap the maximum speed to prevent particle from being flung too far from the missed target.")]
	public float maxSpeed = 50f;

	[Tooltip("How long in the projectile begins being guided towards the target. Higher delay and high particle start speed requires greater distance between attacker and target to avoid uncontrolled orbitting around the target.")]
	public float homingDelay = 1f;

	[Tooltip("How each particle selects the target.")]
	public TSOP targetSelection;

	private ParticleSystem m_System;

	private ParticleSystem.Particle[] m_Particles;

	private int index;

	private void Start()
	{
		if (target[0] == null)
		{
			for (int i = 0; i < target.Length; i++)
			{
				target[i] = base.transform;
			}
		}
		m_System = GetComponent<ParticleSystem>();
		m_Particles = new ParticleSystem.Particle[m_System.main.maxParticles];
	}

	private void LateUpdate()
	{
		int particles = m_System.GetParticles(m_Particles);
		for (int i = 0; i < particles; i++)
		{
			float[] array = new float[target.Length];
			switch (targetSelection)
			{
			case TSOP.random:
				index = Mathf.Abs((int)m_Particles[i].randomSeed) % target.Length;
				break;
			case TSOP.closest:
			{
				for (int j = 0; j < target.Length; j++)
				{
					array[j] = Vector3.Distance(m_Particles[i].position, target[j].position);
				}
				float num = float.MaxValue;
				int num2 = -1;
				index = -1;
				float[] array2 = array;
				foreach (float num3 in array2)
				{
					num2++;
					if (num3 <= num)
					{
						num = num3;
						index = num2;
					}
				}
				break;
			}
			}
			float num4 = (target[index].position - base.transform.position).sqrMagnitude + 0.001f;
			Vector3 a = target[index].position - m_Particles[i].position;
			float sqrMagnitude = a.sqrMagnitude;
			float num5 = Vector3.Dot(m_Particles[i].velocity.normalized, a.normalized);
			float d = Mathf.Abs((num4 - sqrMagnitude) / num4) * num4 * (num5 + 1.001f);
			float num6 = 0f;
			num6 += Time.deltaTime / (homingDelay * 0.01f + 0.0001f);
			m_Particles[i].velocity = Vector3.ClampMagnitude(Vector3.Slerp(m_Particles[i].velocity, m_Particles[i].velocity + a * speed * 0.01f * d, num6), maxSpeed);
		}
		m_System.SetParticles(m_Particles, particles);
	}
}
