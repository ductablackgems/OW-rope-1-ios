using App.Player;
using App.Util;
using App.Vehicles;
using App.Weapons;
using UnityEngine;

public class laser : MonoBehaviour
{
	private bool isEnemyOwner;

	private BreakableItemInstantiate BreableItem;

	private void Awake()
	{
		isEnemyOwner = (GetComponentInParent<PlayerController>() == null);
	}

	private void Start()
	{
	}

	private void OnTriggerStay(Collider other)
	{
		if (isEnemyOwner && other.CompareTag("Player"))
		{
			other.GetComponent<Health>().ApplyDamage(5f, 4);
		}
		if (other.CompareTag("Enemy"))
		{
			other.GetComponent<Health>().ApplyDamage(30f, 4);
		}
		if (other.gameObject.layer == 10)
		{
			ApplyDamageRedirect component = other.GetComponent<ApplyDamageRedirect>();
			if (component != null)
			{
				component.GetHealth().ApplyDamage(70f, 4);
			}
		}
		if (other.CompareTag("Lamp"))
		{
			BreableItem = other.GetComponent<BreakableItemInstantiate>();
			if (BreableItem != null)
			{
				other.GetComponentSafe<BreakableItemInstantiate>().OnLaserDamage();
			}
		}
		if (other.CompareTag("Vehicle"))
		{
			other.GetComponentSafe<Health>().ApplyDamage(70f, 4);
			if (UnityEngine.Random.Range(0, 2) == 0)
			{
				other.SendMessage("AddForceVehicle", 250f, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
