using App.AI.Scanner;
using App.Audio;
using App.GameConfig;
using App.Missions;
using App.Player;
using App.Player.Definition;
using App.Player.FightSystem;
using App.Player.SkeletonEffect;
using App.Spawn;
using App.Util;
using App.Weapons;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace App.AI
{
	public class AITransitions : AbstractAIScript, IResetable, ITargetManager
	{
		public AITemperament temperament;

		public AudioClipsScriptableObject yellClips;

		private AudioSource audioSource;

		private FreeWalkAIModule freeWalkAIModule;

		private FleeAIModule fleeAIModule;

		private BurningAIModule burningAIModule;

		private FightAIModule fightAIModule;

		private ShotAIModule shotAIModule;

		private TaxiStopperAIModule taxiStopperAIModule;

		private StreetVehicleAIModule streetVehicleAIModule;

		private RescuerAIModule rescuerAIModule;

		private PoliceAIModule policeAIModule;

		private NavmeshWalker walker;

		private AIScanner scanner;

		private AggressionTracker aggressionTracker;

		private CarDriver carDriver;

		private BikeDriver bikeDriver;

		private BicycleDriver bicycleDriver;

		private GyroboardDriver gyroboardDriver;

		private SkateboardDriver skateboardDriver;

		private ShotController shotController;

		private AdvancedFightController advancedFightController;

		private PlayerAnimatorHandler animatorHandler;

		private Health health;

		private HumanFireManager fireManager;

		private RagdollHelper ragdollHelper;

		private NavMeshAgent agent;

		private AttackZone attackZone;

		private PlayerMonitor playerMonitor;

		private GameObject player;

		private CollectableGunLoadFactory collectableGunLoadFactory;

		private HealBoxFactory healBoxFactory;

		private CollectableMoneyFactory collectableMoneyFactory;

		private CollectableArmorFactory collectableArmorFactory;

		private CrimeManager crimeManager;

		private GameConfigScriptableObject gameConfig;

		private MissionManager missionManager;

		public bool civil;

		private Health lastAttacker;

		private PlayerModel playerModel;

		private DurationTimer preventFleeTimer = new DurationTimer();

		private DurationTimer preventShotTimer = new DurationTimer();

		public void ResetStates()
		{
			DisableAllModules();
			streetVehicleAIModule.enabled = false;
			if (civil)
			{
				temperament = ((UnityEngine.Random.Range(0, 2) == 0) ? AITemperament.Fearful : AITemperament.Brave);
			}
			if (!civil || temperament == AITemperament.Policeman)
			{
				if (shotController.GunTypes.Length == 0)
				{
					shotAIModule.gunType = GunType.Unknown;
				}
				else
				{
					shotAIModule.gunType = shotController.GunTypes[UnityEngine.Random.Range(0, shotController.GunTypes.Length)];
				}
			}
		}

		GameObject ITargetManager.GetTarget()
		{
			return GetTarget().gameObject;
		}

		GameObject ITargetManager.GetVisibleTargetInRange()
		{
			throw new NotImplementedException();
		}

		protected void Awake()
		{
			audioSource = this.GetComponentSafe<AudioSource>();
			freeWalkAIModule = this.GetComponentSafe<FreeWalkAIModule>();
			fleeAIModule = this.GetComponentSafe<FleeAIModule>();
			burningAIModule = GetComponent<BurningAIModule>();
			fightAIModule = this.GetComponentSafe<FightAIModule>();
			shotAIModule = this.GetComponentSafe<ShotAIModule>();
			taxiStopperAIModule = this.GetComponentSafe<TaxiStopperAIModule>();
			streetVehicleAIModule = this.GetComponentSafe<StreetVehicleAIModule>();
			rescuerAIModule = GetComponent<RescuerAIModule>();
			policeAIModule = GetComponent<PoliceAIModule>();
			walker = this.GetComponentSafe<NavmeshWalker>();
			scanner = this.GetComponentSafe<AIScanner>();
			aggressionTracker = this.GetComponentSafe<AggressionTracker>();
			carDriver = base.ComponentsRoot.GetComponentSafe<CarDriver>();
			bikeDriver = base.ComponentsRoot.GetComponentSafe<BikeDriver>();
			bicycleDriver = base.ComponentsRoot.GetComponentSafe<BicycleDriver>();
			gyroboardDriver = base.ComponentsRoot.GetComponentSafe<GyroboardDriver>();
			skateboardDriver = base.ComponentsRoot.GetComponentSafe<SkateboardDriver>();
			shotController = base.ComponentsRoot.GetComponentSafe<ShotController>();
			advancedFightController = base.ComponentsRoot.GetComponentSafe<AdvancedFightController>();
			animatorHandler = base.ComponentsRoot.GetComponentSafe<PlayerAnimatorHandler>();
			health = base.ComponentsRoot.GetComponentSafe<Health>();
			fireManager = base.ComponentsRoot.GetComponent<HumanFireManager>();
			ragdollHelper = base.ComponentsRoot.GetComponentSafe<RagdollHelper>();
			agent = base.ComponentsRoot.GetComponentSafe<NavMeshAgent>();
			attackZone = base.ComponentsRoot.GetComponentInChildrenSafe<AttackZone>();
			playerMonitor = ServiceLocator.Get<PlayerMonitor>();
			player = ServiceLocator.GetGameObject("Player");
			collectableGunLoadFactory = ServiceLocator.Get<CollectableGunLoadFactory>();
			healBoxFactory = ServiceLocator.Get<HealBoxFactory>();
			collectableMoneyFactory = ServiceLocator.Get<CollectableMoneyFactory>();
			collectableArmorFactory = ServiceLocator.Get<CollectableArmorFactory>();
			crimeManager = ServiceLocator.Get<CrimeManager>();
			gameConfig = ServiceLocator.Get<ConfigContainer>().gameConfig;
			missionManager = ServiceLocator.Get<MissionManager>();
			playerModel = ServiceLocator.GetPlayerModel();
			ragdollHelper.OnRaggdolled += OnRagdolled;
			ragdollHelper.OnStandUpDone += OnStandUpDone;
			carDriver.BeforeRun += BeforeVehicleControllerStart;
			carDriver.AfterStop += AfterVehicleControllerStop;
			carDriver.OnKickOutDriver += OnKickOutDriver;
			bikeDriver.BeforeRun += BeforeVehicleControllerStart;
			bikeDriver.AfterStop += AfterVehicleControllerStop;
			bikeDriver.OnKickOutDriver += OnKickOutDriver;
			health.OnDamage += OnDamage;
			health.OnDie += OnDie;
			streetVehicleAIModule.OnLooseVehicle += OnLooseBicycle;
		}

		protected void OnDestroy()
		{
			ragdollHelper.OnRaggdolled -= OnRagdolled;
			ragdollHelper.OnStandUpDone -= OnStandUpDone;
			carDriver.BeforeRun -= BeforeVehicleControllerStart;
			carDriver.AfterStop -= AfterVehicleControllerStop;
			carDriver.OnKickOutDriver -= OnKickOutDriver;
			bikeDriver.BeforeRun -= BeforeVehicleControllerStart;
			bikeDriver.AfterStop -= AfterVehicleControllerStop;
			bikeDriver.OnKickOutDriver -= OnKickOutDriver;
			health.OnDamage -= OnDamage;
			health.OnDie -= OnDie;
			streetVehicleAIModule.OnLooseVehicle -= OnLooseBicycle;
		}

		protected void Update()
		{
			attackZone.gameObject.SetActive(fightAIModule.enabled);
			if (taxiStopperAIModule.enabled && taxiStopperAIModule.carRigidbody == null)
			{
				missionManager.FinishMission(taxiStopperAIModule.missionKey, success: false);
				taxiStopperAIModule.enabled = false;
				freeWalkAIModule.enabled = true;
			}
			if (taxiStopperAIModule.enabled && !missionManager.CompareMission(taxiStopperAIModule.missionKey))
			{
				taxiStopperAIModule.enabled = false;
				freeWalkAIModule.enabled = true;
			}
			animatorHandler.IdleSubBlend.BlendTo(advancedFightController.Running() ? 1 : 0);
			animatorHandler.IdleSubBlend.Update(Time.deltaTime);
			if (health.Dead())
			{
				scanner.enabled = false;
				return;
			}
			bool flag = aggressionTracker.AttackedRecently(30f);
			bool flag2 = shotAIModule.gunType != GunType.Unknown;
			bool flag3 = aggressionTracker.ShotRecently(30f);
			scanner.enabled = ((temperament == AITemperament.Policeman && crimeManager.StarCount > 0) || (temperament != AITemperament.Fearful && flag && (flag2 || !flag3)));
			if (ragdollHelper.Ragdolled || ragdollHelper.StandingUp)
			{
				return;
			}
			if (carDriver.Running() || bikeDriver.Running() || bicycleDriver.Running() || gyroboardDriver.Running() || skateboardDriver.Running())
			{
				if (fireManager != null && fireManager.IsBurning())
				{
					carDriver.Stop();
					bikeDriver.Stop();
					bicycleDriver.Stop();
					gyroboardDriver.Stop();
					skateboardDriver.Stop();
				}
				else if ((!carDriver.StayInCar && carDriver.ReceivedEnoughDamage()) || bikeDriver.ReceivedEnoughDamage())
				{
					carDriver.Stop();
					bikeDriver.Stop();
					health.ApplyDamage(1f, 2);
				}
				return;
			}
			if (burningAIModule != null)
			{
				if (fireManager.IsBurning() && !burningAIModule.enabled)
				{
					DisableAllModules();
					burningAIModule.enabled = true;
					return;
				}
				if (!fireManager.IsBurning() && burningAIModule.enabled)
				{
					burningAIModule.enabled = false;
					freeWalkAIModule.enabled = true;
				}
				if (burningAIModule.enabled)
				{
					return;
				}
			}
			Health target = GetTarget();
			float num = Vector3.Distance(target.transform.position, base.ComponentsRoot.position);
			bool num2 = num < 90f && ((temperament == AITemperament.Fearful && flag) || ((temperament == AITemperament.Brave && !flag2) & flag3));
			bool flag4 = target.gameObject == player;
			bool flag5 = !flag4 || scanner.HasTrackToPlayer;
			bool flag6 = !num2 && flag5 && num < 90f && (((temperament == AITemperament.Brave || temperament == AITemperament.Policeman) & flag) || (temperament == AITemperament.Policeman && crimeManager.StarCount > 0));
			bool flag7 = flag6 && num <= 17f;
			if (scanner.TrackShootable != flag7)
			{
				scanner.SetTrackShootable(flag7);
			}
			if (num2)
			{
				if (!fleeAIModule.enabled)
				{
					DisableAllModules();
					if (!ragdollHelper.Ragdolled && !preventFleeTimer.InProgress())
					{
						fleeAIModule.enabled = true;
					}
				}
			}
			else if (flag6)
			{
				bool flag8 = playerMonitor.SittingInVehicle();
				if (preventShotTimer.Done())
				{
					preventShotTimer.Stop();
				}
				bool flag9 = preventShotTimer.Running();
				float num3 = flag4 ? 10f : 0f;
				bool flag10 = flag4 && num < 5f;
				if (flag2 && !flag9 && !shotAIModule.enabled && !flag8 && num > num3)
				{
					DisableAllModules();
					shotAIModule.enabled = true;
				}
				else if (!fightAIModule.enabled && (flag8 || flag10))
				{
					DisableAllModules();
					fightAIModule.enabled = true;
				}
				else if (!shotAIModule.enabled && !fightAIModule.enabled)
				{
					DisableAllModules();
					fightAIModule.enabled = true;
				}
			}
			else if ((shotAIModule.enabled || fightAIModule.enabled) && !streetVehicleAIModule.enabled)
			{
				DisableAllModules();
				freeWalkAIModule.enabled = true;
			}
			else if (rescuerAIModule != null)
			{
				if (rescuerAIModule.enabled && rescuerAIModule.myVehicle == null && !rescuerAIModule.DeathTarget.IsValid())
				{
					rescuerAIModule.enabled = false;
					freeWalkAIModule.enabled = true;
				}
				if (!rescuerAIModule.enabled && (rescuerAIModule.myVehicle != null || rescuerAIModule.DeathTarget.IsValid()))
				{
					DisableAllModules();
					rescuerAIModule.enabled = true;
				}
			}
			else if (policeAIModule != null)
			{
				if (policeAIModule.enabled && policeAIModule.myVehicle == null && !policeAIModule.DeathTarget.IsValid())
				{
					policeAIModule.enabled = false;
					freeWalkAIModule.enabled = true;
				}
				if (!policeAIModule.enabled && (policeAIModule.myVehicle != null || policeAIModule.DeathTarget.IsValid()))
				{
					DisableAllModules();
					policeAIModule.enabled = true;
				}
			}
		}

		private void OnRagdolled(bool ragdolled)
		{
			if (ragdolled)
			{
				agent.enabled = false;
				DisableAllModules(includeStreetVehicleModule: false);
			}
			else
			{
				walker.Stop();
				preventShotTimer.Run(2.5f);
			}
		}

		private void OnStandUpDone()
		{
			agent.enabled = true;
			preventFleeTimer.Run(1.5f);
			if (!streetVehicleAIModule.enabled)
			{
				freeWalkAIModule.enabled = true;
			}
		}

		private void BeforeVehicleControllerStart()
		{
			agent.enabled = false;
			DisableAllModules(includeStreetVehicleModule: true, includeTaxiStopper: false);
			scanner.enabled = false;
		}

		private void AfterVehicleControllerStop()
		{
			if (taxiStopperAIModule.enabled)
			{
				missionManager.FinishMission(taxiStopperAIModule.missionKey, success: false);
			}
			agent.enabled = true;
			DisableAllModules();
			freeWalkAIModule.enabled = true;
		}

		private void OnDamage(float damage, int damageType, GameObject agressor)
		{
			switch (damageType)
			{
			case 8:
				audioSource.PlayOneShot(yellClips.GetRandomClip());
				break;
			case 2:
			case 4:
				if ((player.Equals(agressor) || damageType == 4) && !shotAIModule.enabled && !ragdollHelper.Ragdolled)
				{
					lastAttacker = playerModel.Health;
					preventShotTimer.Stop();
					float distance = playerMonitor.GetDistance(base.ComponentsRoot.transform.position);
					bool flag = shotAIModule.gunType != GunType.Unknown;
					if (distance > 5f && flag && IsShooter())
					{
						DisableAllModules();
						shotAIModule.enabled = true;
					}
				}
				break;
			case 11:
				if (agressor.CompareTagSafe("Dog"))
				{
					TryHandleDogAttack(agressor);
				}
				break;
			}
		}

		private bool IsShooter()
		{
			if (temperament != 0)
			{
				return temperament == AITemperament.Policeman;
			}
			return true;
		}

		private void OnDie()
		{
			if (shotAIModule.gunType != 0)
			{
				if (UnityEngine.Random.Range(0f, 100f) <= 40f)
				{
					GameObject gameObject = collectableGunLoadFactory.Create(shotAIModule.gunType);
					gameObject.transform.position = base.ComponentsRoot.transform.position + Vector3.left * 0.4f;
					UnityEngine.Object.Destroy(gameObject.gameObject, 180f);
				}
				if (UnityEngine.Random.Range(0f, 100f) <= (float)gameConfig.killMoneyPercentage)
				{
					CollectableMoney collectableMoney = collectableMoneyFactory.Create(UnityEngine.Random.Range(gameConfig.minKillMoney, gameConfig.maxKillMoney + 1));
					collectableMoney.transform.position = base.ComponentsRoot.transform.position + Vector3.right * 0.4f;
					UnityEngine.Object.Destroy(collectableMoney.gameObject, 180f);
				}
				float num = UnityEngine.Random.Range(0f, 100f);
				if (num <= 25f)
				{
					GameObject gameObject2 = healBoxFactory.Create();
					gameObject2.transform.position = base.ComponentsRoot.transform.position;
					UnityEngine.Object.Destroy(gameObject2.gameObject, 180f);
				}
				else if (num <= 40f)
				{
					GameObject gameObject3 = collectableArmorFactory.Create();
					gameObject3.transform.position = base.ComponentsRoot.transform.position;
					UnityEngine.Object.Destroy(gameObject3.gameObject, 180f);
				}
			}
		}

		private void OnKickOutDriver()
		{
			if (temperament == AITemperament.Policeman)
			{
				crimeManager.SetBusted(busted: true);
			}
		}

		private void OnLooseBicycle()
		{
			freeWalkAIModule.enabled = true;
		}

		private void DisableAllModules(bool includeStreetVehicleModule = true, bool includeTaxiStopper = true)
		{
			fleeAIModule.enabled = false;
			fightAIModule.enabled = false;
			shotAIModule.enabled = false;
			freeWalkAIModule.enabled = false;
			if (burningAIModule != null)
			{
				burningAIModule.enabled = false;
			}
			if (rescuerAIModule != null)
			{
				rescuerAIModule.enabled = false;
			}
			if (policeAIModule != null)
			{
				policeAIModule.enabled = false;
			}
			if (includeTaxiStopper)
			{
				taxiStopperAIModule.enabled = false;
			}
			if ((!bicycleDriver.Running() && !gyroboardDriver.Running() && !skateboardDriver.Running()) & includeStreetVehicleModule)
			{
				streetVehicleAIModule.enabled = false;
			}
		}

		private Health GetTarget()
		{
			if (lastAttacker == null || lastAttacker.Dead())
			{
				lastAttacker = playerModel.Health;
			}
			return lastAttacker;
		}

		private void TryHandleDogAttack(GameObject agressor)
		{
			lastAttacker = agressor.GetComponent<Health>();
			if (!shotAIModule.enabled && !ragdollHelper.Ragdolled && IsShooter() && shotAIModule.gunType != 0)
			{
				preventShotTimer.Stop();
				DisableAllModules();
				shotAIModule.enabled = true;
			}
		}
	}
}
