using UnityEngine;

namespace Forge3D
{
	public class F3DFlameThrower : MonoBehaviour
	{
		public Light pLight;

		public ParticleSystem heat;

		private int lightState;

		private bool despawn;

		private ParticleSystem ps;

		private void Start()
		{
			ps = GetComponent<ParticleSystem>();
		}

		private void OnSpawned()
		{
			despawn = false;
			lightState = 1;
			pLight.intensity = 0f;
		}

		private void OnDespawned()
		{
		}

		private void OnDespawn()
		{
			F3DPoolManager.Pools["GeneratedPool"].Despawn(base.transform);
		}

		private void Update()
		{
			if (Input.GetMouseButtonUp(0) && !despawn)
			{
				despawn = true;
				F3DTime.time.AddTimer(1f, 1, OnDespawn);
				ps.Stop();
				if ((bool)heat)
				{
					heat.Stop();
				}
				pLight.intensity = 0.6f;
				lightState = -1;
			}
			if (lightState == 1)
			{
				pLight.intensity = Mathf.Lerp(pLight.intensity, 0.7f, Time.deltaTime * 10f);
				if (pLight.intensity >= 0.5f)
				{
					lightState = 0;
				}
			}
			else if (lightState == -1)
			{
				pLight.intensity = Mathf.Lerp(pLight.intensity, -0.1f, Time.deltaTime * 10f);
				if (pLight.intensity <= 0f)
				{
					lightState = 0;
				}
			}
		}
	}
}
