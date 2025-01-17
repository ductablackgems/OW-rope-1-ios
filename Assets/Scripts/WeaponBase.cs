using UnityEngine;

public class WeaponBase : MonoBehaviour
{
	[HideInInspector]
	public GameObject Owner;

	[HideInInspector]
	public GameObject Target;

	public string[] TargetTag = new string[1]
	{
		"Enemy"
	};

	public string[] IgnoreTag;

	public bool RigidbodyProjectile;

	public Vector3 TorqueSpeedAxis;

	public GameObject TorqueObject;

	public bool DoDamageCheck(GameObject gob)
	{
		for (int i = 0; i < IgnoreTag.Length; i++)
		{
			if (IgnoreTag[i] == gob.tag)
			{
				return false;
			}
		}
		return true;
	}
}
