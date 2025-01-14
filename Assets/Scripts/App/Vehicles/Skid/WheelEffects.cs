using System.Collections;
using UnityEngine;

namespace App.Vehicles.Skid
{
	public class WheelEffects : MonoBehaviour
	{
		public Transform SkidTrailPrefab;

		public static Transform skidTrailsDetachedParent;

		public ParticleSystem skidParticles;

		private Transform m_SkidTrail;

		private WheelCollider m_WheelCollider;

		public bool skidding
		{
			get;
			private set;
		}

		private void Start()
		{
			m_WheelCollider = GetComponent<WheelCollider>();
			if (skidTrailsDetachedParent == null)
			{
				skidTrailsDetachedParent = new GameObject("Skid Trails - Detached").transform;
			}
		}

		public void EmitTyreSmoke()
		{
			skidParticles.transform.position = base.transform.position - base.transform.up * m_WheelCollider.radius;
			skidParticles.Play();
			if (!skidding)
			{
				StartCoroutine(StartSkidTrail());
			}
		}

		public IEnumerator StartSkidTrail()
		{
			skidding = true;
			m_SkidTrail = UnityEngine.Object.Instantiate(SkidTrailPrefab, base.transform.position + base.transform.TransformDirection(-Vector3.up * m_WheelCollider.radius), Quaternion.identity, base.transform);
			while (m_SkidTrail == null)
			{
				yield return null;
			}
		}

		public void EndSkidTrail()
		{
			if (skidding)
			{
				skidding = false;
				m_SkidTrail.parent = skidTrailsDetachedParent;
				UnityEngine.Object.Destroy(m_SkidTrail.gameObject, 10f);
				skidParticles.Stop();
			}
		}
	}
}
