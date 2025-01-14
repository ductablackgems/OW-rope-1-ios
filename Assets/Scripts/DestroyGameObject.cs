using App;
using App.AI;
using App.Player;
using App.SaveSystem;
using App.Settings;
using App.Spawn;
using App.Spawn.Pooling;
using App.Util;
using App.Vehicles;
using App.Vehicles.Car.GarbageTruck;
using UnityEngine;

public class DestroyGameObject : MonoBehaviour
{
	public float clearDistance = 100f;

	public bool human;

	public bool vehicle;

	private Health health;

	private TaxiStopperAIModule taxiStopperAIModule;

	private Transform playerTransform;

	private Resetor resetor;

	private static EnemyPooler enemyPooler;

	private DurationTimer deathTimer = new DurationTimer();

	private CarDriver playerCarDriver;

	private BikeDriver playerBikeDriver;

	private float playerDistance;

	private Renderer[] rs;

	private Animator a;

	private float optimalNum;

	public float maxDistance;

	private float coof = 1f;

	private SettingsSaveEntity settingsSave;

	private bool done;

	private void Awake()
	{
		settingsSave = ServiceLocator.Get<SaveEntities>().SettingsSave;
		optimalNum = settingsSave.graphicQuality;
		if (human && !vehicle)
		{
			playerCarDriver = GetComponent<CarDriver>();
			playerBikeDriver = GetComponent<BikeDriver>();
			if (base.transform.parent != null)
			{
				coof = 0.6f;
			}
			else
			{
				coof = 1f;
			}
			if (optimalNum == 2f)
			{
				maxDistance = 121f * coof;
			}
			else if (optimalNum == 1f)
			{
				maxDistance = 92f * coof;
			}
			else if (optimalNum == 0f)
			{
				maxDistance = 70f * coof;
			}
			else if (optimalNum == 3f)
			{
				maxDistance = 131f * coof;
			}
		}
		else
		{
			QualityScheme scheme = ServiceLocator.Get<QualitySchemeManager>().GetScheme();
			maxDistance = scheme.vehicleSpawn.destroyDistance;
		}
		if (human)
		{
			health = this.GetComponentSafe<Health>();
			taxiStopperAIModule = this.GetComponentInChildrenSafe<TaxiStopperAIModule>();
			resetor = this.GetComponentSafe<Resetor>();
			clearDistance = maxDistance;
		}
		else
		{
			clearDistance = maxDistance;
		}
		if (enemyPooler == null)
		{
			enemyPooler = ServiceLocator.Get<EnemyPooler>();
		}
		playerTransform = ServiceLocator.GetGameObject("Player").transform;
		a = GetComponent<Animator>();
	}

	private void OnDisable()
	{
		coof = 1f;
	}

	private void OnEnable()
	{
		if (human && !vehicle)
		{
			if (base.transform.parent != null)
			{
				coof = 1f;
			}
			else
			{
				coof = 0.7f;
			}
			if (optimalNum == 2f)
			{
				maxDistance = 141f * coof;
			}
			else if (optimalNum == 1f)
			{
				maxDistance = 92f * coof;
			}
			else if (optimalNum == 0f)
			{
				maxDistance = 70f * coof;
			}
			else if (optimalNum == 3f)
			{
				maxDistance = 151f * coof;
			}
			clearDistance = maxDistance;
		}
	}

	private void Update()
	{
		if (human && base.transform.parent != null)
		{
			if (coof != 1f)
			{
				coof = 1f;
			}
			return;
		}
		if (human && health.Dead() && !deathTimer.Running())
		{
			deathTimer.Run(45f);
		}
		if (human && !health.Dead() && deathTimer.Running())
		{
			deathTimer.Stop();
		}
		playerDistance = Vector3.Distance(base.transform.position, playerTransform.position);
		if (!deathTimer.Done() && (!(playerDistance > clearDistance) || (!(taxiStopperAIModule == null) && taxiStopperAIModule.enabled)))
		{
			return;
		}
		if (human)
		{
			deathTimer.Stop();
			enemyPooler.Push(resetor);
			return;
		}
		VehicleComponents componentSafe = base.gameObject.GetComponentSafe<VehicleComponents>();
		if (componentSafe.driver != null)
		{
			Resetor componentSafe2 = componentSafe.driver.GetComponentSafe<Resetor>();
			enemyPooler.Push(componentSafe2);
		}
		if (componentSafe.passenger != null)
		{
			Resetor componentSafe3 = componentSafe.passenger.GetComponentSafe<Resetor>();
			enemyPooler.Push(componentSafe3);
		}
		if (componentSafe.playerGarbageTruckControl != null)
		{
			ContainerParameters componentInChildren = base.gameObject.GetComponentInChildren<ContainerParameters>();
			if ((bool)componentInChildren)
			{
				componentInChildren.EndLif(toDefaultPosition: true);
				componentInChildren.ClearAIPoint();
			}
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
