using UnityEngine;

public class MoverMissile : WeaponBase
{
	public float Damping = 3f;

	public float Speed = 80f;

	public float SpeedMax = 80f;

	public float SpeedMult = 1f;

	public Vector3 Noise = new Vector3(20f, 20f, 20f);

	public float TargetLockDirection = 0.5f;

	public int DistanceLock = 70;

	public int DurationLock = 40;

	public bool Seeker;

	public float LifeTime = 5f;

	private bool locked;

	private int timetorock;

	private float timeCount;

	private void Start()
	{
		timeCount = Time.time;
		UnityEngine.Object.Destroy(base.gameObject, LifeTime);
	}

	private void FixedUpdate()
	{
		GetComponent<Rigidbody>().velocity = new Vector3(base.transform.forward.x * Speed * Time.fixedDeltaTime, base.transform.forward.y * Speed * Time.fixedDeltaTime, base.transform.forward.z * Speed * Time.fixedDeltaTime);
		if (Speed < SpeedMax)
		{
			Speed += SpeedMult * Time.fixedDeltaTime;
		}
	}

	private void Update()
	{
		if (Time.time >= timeCount + LifeTime - 0.5f)
		{
			Damage component = GetComponent<Damage>();
			if (component != null)
			{
				component.Active(component.Effect);
			}
		}
		if ((bool)Target)
		{
			Quaternion b = Quaternion.LookRotation(Target.transform.position - base.transform.transform.position);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * Damping);
			if (Vector3.Dot((Target.transform.position - base.transform.position).normalized, base.transform.forward) < TargetLockDirection)
			{
				Target = null;
			}
		}
		if (!Seeker)
		{
			return;
		}
		if (timetorock > DurationLock)
		{
			if (!locked && !Target)
			{
				float num = 2.14748365E+09f;
				for (int i = 0; i < TargetTag.Length; i++)
				{
					if (GameObject.FindGameObjectsWithTag(TargetTag[i]).Length == 0)
					{
						continue;
					}
					GameObject[] array = GameObject.FindGameObjectsWithTag(TargetTag[i]);
					for (int j = 0; j < array.Length; j++)
					{
						if (!array[j])
						{
							continue;
						}
						float num2 = Vector3.Dot((array[j].transform.position - base.transform.position).normalized, base.transform.forward);
						float num3 = Vector3.Distance(array[j].transform.position, base.transform.position);
						if (num2 >= TargetLockDirection && (float)DistanceLock > num3)
						{
							if (num > num3)
							{
								num = num3;
								Target = array[j];
							}
							locked = true;
						}
					}
				}
			}
		}
		else
		{
			timetorock++;
		}
		if (!Target)
		{
			locked = false;
		}
	}
}
