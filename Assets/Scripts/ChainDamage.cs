using UnityEngine;

public class ChainDamage : DamageBase
{
	public GameObject ChainObject;

	public int NumberChain = 5;

	public int Distance = 100;

	public float Direction = 0.5f;

	private void Start()
	{
		int num = 0;
		for (int i = 0; i < TargetTag.Length; i++)
		{
			if (GameObject.FindGameObjectsWithTag(TargetTag[i]).Length == 0)
			{
				continue;
			}
			GameObject[] array = GameObject.FindGameObjectsWithTag(TargetTag[i]);
			float num2 = Distance;
			for (int j = 0; j < array.Length; j++)
			{
				if (!(array[j] != null))
				{
					continue;
				}
				float num3 = Vector3.Dot((array[j].transform.position - base.transform.position).normalized, base.transform.forward);
				float num4 = Vector3.Distance(array[j].transform.position, base.transform.position);
				if (num4 < num2)
				{
					if (num3 >= Direction && (bool)ChainObject && num <= NumberChain)
					{
						GameObject gameObject = Object.Instantiate(ChainObject, base.transform.position, base.transform.rotation);
						Quaternion rotation = Quaternion.LookRotation(array[j].transform.position - gameObject.transform.position);
						gameObject.transform.rotation = rotation;
						num++;
					}
					num2 = num4;
				}
			}
		}
	}
}
