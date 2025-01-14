using System.Collections.Generic;
using UnityEngine;

namespace MagicalFXMagicalPro
{
	public class FX_Tentacle : MonoBehaviour
	{
		public TrailRenderer Trail;

		public int SubNumber = 5;

		public int Number = 10;

		public float Speed = 10f;

		public float Damping = 10f;

		public Vector3 Direction;

		public Vector3 SpreadMin;

		public Vector3 SpreadMax;

		public Vector3 SpreadSpawn;

		public Vector3 SubSpread;

		public float Duration = 10f;

		public float SubDuration = 10f;

		public float SubRate = 10f;

		public float GravityMult;

		public bool ToGround;

		private TentacleInstance[] trails;

		public List<TentacleInstance> subTrails;

		private float timeTmp;

		private void Start()
		{
			subTrails = new List<TentacleInstance>();
			timeTmp = Time.time;
			trails = new TentacleInstance[Number];
			for (int i = 0; i < trails.Length; i++)
			{
				Vector3 b = new Vector3(UnityEngine.Random.Range(0f - SpreadSpawn.x, SpreadSpawn.x), UnityEngine.Random.Range(0f - SpreadSpawn.y, SpreadSpawn.y), UnityEngine.Random.Range(0f - SpreadSpawn.z, SpreadSpawn.z)) * 0.01f;
				trails[i] = new TentacleInstance();
				trails[i].Trail = UnityEngine.Object.Instantiate(Trail, base.transform.position + b, Quaternion.identity);
				trails[i].Trail.transform.forward = Vector3.up;
				trails[i].TimeTmp = Time.time;
				trails[i].TimeSubTmp = Time.time;
				trails[i].SubNumber = SubNumber;
				trails[i].SpawnRate = SubRate;
				trails[i].Trail.transform.parent = base.transform;
			}
		}

		private void AddSubtrail(Vector3 position)
		{
			TentacleInstance tentacleInstance = new TentacleInstance();
			tentacleInstance.Trail = UnityEngine.Object.Instantiate(Trail, position, Quaternion.identity);
			tentacleInstance.Trail.endWidth *= 0.5f;
			tentacleInstance.Trail.startWidth *= 0.5f;
			tentacleInstance.Trail.transform.parent = base.transform;
			tentacleInstance.Trail.transform.forward = Vector3.up;
			tentacleInstance.TimeTmp = Time.time;
			tentacleInstance.Duration = UnityEngine.Random.Range(0f, SubDuration);
			subTrails.Add(tentacleInstance);
		}

		public void End(float speed)
		{
			for (int i = 0; i < trails.Length; i++)
			{
				Color startColor = trails[i].Trail.startColor;
				Color endColor = trails[i].Trail.endColor;
				startColor.a = Mathf.Lerp(startColor.a, 0f, speed * Time.deltaTime);
				endColor.a = startColor.a;
				trails[i].Trail.startColor = startColor;
				trails[i].Trail.endColor = endColor;
			}
			foreach (TentacleInstance subTrail in subTrails)
			{
				Color startColor2 = subTrail.Trail.startColor;
				Color endColor2 = subTrail.Trail.endColor;
				startColor2.a = Mathf.Lerp(startColor2.a, 0f, speed * Time.deltaTime);
				endColor2.a = startColor2.a;
				subTrail.Trail.startColor = startColor2;
				subTrail.Trail.endColor = endColor2;
			}
		}

		private void Update()
		{
			if (Time.time < timeTmp + Duration)
			{
				for (int i = 0; i < trails.Length; i++)
				{
					if (trails[i] != null)
					{
						Vector3 b = new Vector3(UnityEngine.Random.Range(SpreadMin.x, SpreadMax.x), UnityEngine.Random.Range(SpreadMin.y, SpreadMax.y), UnityEngine.Random.Range(SpreadMin.z, SpreadMax.z));
						Quaternion b2 = Quaternion.LookRotation((base.transform.position + Direction + b - trails[i].Trail.transform.position).normalized);
						trails[i].Trail.transform.rotation = Quaternion.Lerp(trails[i].Trail.transform.rotation, b2, Damping * Time.deltaTime);
						trails[i].Trail.transform.position += trails[i].Trail.transform.forward * Speed * Time.deltaTime;
						trails[i].Trail.transform.position += Vector3.up * GravityMult * Time.deltaTime;
						if ((float)subTrails.Count < trails[i].SubNumber && Time.time > trails[i].TimeSubTmp + trails[i].SpawnRate)
						{
							trails[i].TimeSubTmp = Time.time;
							AddSubtrail(trails[i].Trail.transform.position);
						}
					}
				}
				foreach (TentacleInstance subTrail in subTrails)
				{
					if (Time.time < subTrail.TimeTmp + subTrail.Duration)
					{
						Vector3 b3 = new Vector3(UnityEngine.Random.Range(0f - SubSpread.x, SubSpread.x), UnityEngine.Random.Range(0f - SubSpread.y, SubSpread.y), UnityEngine.Random.Range(0f - SubSpread.z, SubSpread.z));
						Quaternion b4 = Quaternion.LookRotation((base.transform.position + Direction + b3 - subTrail.Trail.transform.position).normalized);
						subTrail.Trail.transform.rotation = Quaternion.Lerp(subTrail.Trail.transform.rotation, b4, Damping * Time.deltaTime);
						subTrail.Trail.transform.position += subTrail.Trail.transform.forward * Speed * 0.5f * Time.deltaTime;
						subTrail.Trail.transform.position += Vector3.up * GravityMult * Time.deltaTime;
					}
				}
			}
			else
			{
				if (!ToGround)
				{
					return;
				}
				for (int j = 0; j < trails.Length; j++)
				{
					if (trails[j] != null)
					{
						trails[j].Trail.transform.position += trails[j].Trail.transform.forward * Speed * 0.3f * Time.deltaTime;
						trails[j].Trail.transform.position += -Vector3.up * Speed * 0.9f * Time.deltaTime;
					}
				}
				foreach (TentacleInstance subTrail2 in subTrails)
				{
					subTrail2.Trail.transform.position += subTrail2.Trail.transform.forward * Speed * 0.3f * Time.deltaTime;
					subTrail2.Trail.transform.position += -Vector3.up * Speed * 0.9f * Time.deltaTime;
				}
			}
		}
	}
}
