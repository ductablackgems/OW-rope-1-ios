using App;
using App.AI;
using UnityEngine;

public class AimShot : MonoBehaviour
{
	public float time;

	public float AimLate = 0.3f;

	private Transform projectileOuter;

	private GameObject player;

	private bool run;

	private ITargetManager targetManager;

	private void Start()
	{
		if (!base.transform.root.CompareTag("Player"))
		{
			player = ServiceLocator.GetPlayerModel().GameObject;
			WeaponLauncher componentSafe = this.GetComponentSafe<WeaponLauncher>();
			projectileOuter = componentSafe.MissileOuter[0];
			componentSafe.Spread += 4f;
			targetManager = componentSafe.Owner.GetComponentInChildren<ITargetManager>();
			time = AimLate;
			run = true;
		}
		else
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (run)
		{
			time -= Time.deltaTime;
			if (!(time > 0f))
			{
				time = AimLate;
				GameObject gameObject = (targetManager != null) ? targetManager.GetTarget() : player;
				projectileOuter.transform.LookAt(gameObject.transform.position + Vector3.up);
			}
		}
	}
}
