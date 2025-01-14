using UnityEngine;
using UnityEngine.UI;

public class RepairArea : MonoBehaviour
{
	public ImpactDeformable PlayerCallHull;

	public Text Text;

	private float repairTime;

	private void Awake()
	{
		InvokeRepeating("Repair", 0f, 0.1f);
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject == PlayerCallHull.gameObject)
		{
			repairTime = Time.time;
		}
	}

	private void Repair()
	{
		bool flag = Time.time - repairTime <= 0.11f;
		Text.gameObject.SetActive(flag);
		if (flag)
		{
			PlayerCallHull.Repair(0.1f);
		}
	}
}
