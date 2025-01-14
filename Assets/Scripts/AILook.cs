using UnityEngine;

public class AILook : MonoBehaviour
{
	public string[] TargetTag = new string[1]
	{
		"Enemy"
	};

	public float Distance = 1000f;

	private int indexWeapon;

	private GameObject target;

	private WeaponController weapon;

	private float timeAIattack;

	private void Start()
	{
		weapon = GetComponent<WeaponController>();
	}

	private void Update()
	{
		if ((bool)target)
		{
			Quaternion b = Quaternion.LookRotation(target.transform.position - base.transform.position);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, Time.deltaTime * 3f);
			if (Vector3.Dot((target.transform.position - base.transform.position).normalized, base.transform.forward) > 0.9f)
			{
				bool flag = (bool)weapon;
			}
			if (Time.time > timeAIattack + 3f)
			{
				target = null;
			}
			return;
		}
		for (int i = 0; i < TargetTag.Length; i++)
		{
			if (GameObject.FindGameObjectsWithTag(TargetTag[i]).Length == 0)
			{
				continue;
			}
			GameObject[] array = GameObject.FindGameObjectsWithTag(TargetTag[i]);
			float num = Distance;
			for (int j = 0; j < array.Length; j++)
			{
				float num2 = Vector3.Distance(array[j].transform.position, base.transform.position);
				if (num > num2)
				{
					num = num2;
					target = array[j];
					bool flag2 = (bool)weapon;
					timeAIattack = Time.time;
				}
			}
		}
	}
}
