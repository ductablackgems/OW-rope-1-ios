using App;
using App.Abilities;
using App.Player.Definition;
using App.Util;
using UnityEngine;

public class MagicShield : MonoBehaviour, IDamageBlocker
{
	public Ability attackAbilityPrefab;

	public Ability defenseAbilityPrefab;

	public Ability shockWaveAbilityPrefab;

	public Transform hammerHead;

	private Ability attackAbility;

	private Ability defenseAbility;

	private Ability shockWaveAbility;

	private EnergyScript energy;

	private PlayerAnimatorHandler animatorHandler;

	private DurationTimer checkTimer = new DurationTimer();

	private GameObject player;

	private int enemyLayerMask;

	public bool ShieldActivated
	{
		get
		{
			if (defenseAbility != null)
			{
				return defenseAbility.IsRunning;
			}
			return false;
		}
	}

	public bool DefenseRunning => ShieldActivated;

	public bool IsMovementBlocked => GetIsMovementBlocked();

	public bool IsInjuryBlockRequest => GetIsInjuryBlockRequest();

	bool IDamageBlocker.IsDamageBlocked(float damage)
	{
		return ShieldActivated;
	}

	public bool ActivateLightningStorm()
	{
		if (attackAbility.IsRunning)
		{
			return false;
		}
		if (energy.GetCurrentEnergy() < attackAbility.MinEnergyRequired)
		{
			return false;
		}
		if (PhysicsUtils.GetNumberOfObjectsInRadius(player.transform.position, attackAbility.Radius, enemyLayerMask) == 0)
		{
			return false;
		}
		attackAbility.Activate();
		return true;
	}

	public void ActivateShockWave()
	{
		if (!shockWaveAbility.IsRunning && !(energy.GetCurrentEnergy() < shockWaveAbility.MinEnergyRequired))
		{
			shockWaveAbility.Activate(hammerHead.position);
		}
	}

	private void OnStormAnimationReady()
	{
		(attackAbility as LightningStormAbility).ActivateEffect();
	}

	private void Awake()
	{
		energy = ServiceLocator.Get<EnergyScript>();
		player = ServiceLocator.GetGameObject("Player");
		animatorHandler = player.GetComponent<PlayerAnimatorHandler>();
		enemyLayerMask = LayerMask.GetMask("Enemy");
		attackAbility = InitializeAbility(attackAbilityPrefab);
		defenseAbility = InitializeAbility(defenseAbilityPrefab);
		shockWaveAbility = InitializeAbility(shockWaveAbilityPrefab);
	}

	private void Update()
	{
		animatorHandler.MagicShield = GetAnimParam();
		if (energy.GetCurrentEnergy() < 0.05f)
		{
			defenseAbility.Deactivate();
			attackAbility.Deactivate();
		}
		ProcessAbilityActivation();
		UpdateEnergyDrain();
	}

	private void UpdateEnergyDrain()
	{
		Ability runningAbility = GetRunningAbility();
		if (!(runningAbility == null))
		{
			float amount = Time.deltaTime * runningAbility.EnergyDrain;
			energy.ConsumeEnergy(amount);
		}
	}

	private void ActivateShield()
	{
		if (!defenseAbility.IsRunning && !(energy.GetCurrentEnergy() < defenseAbility.MinEnergyRequired))
		{
			defenseAbility.Activate();
		}
	}

	private Ability InitializeAbility(Ability abilityPrefab)
	{
		Ability ability = UnityEngine.Object.Instantiate(abilityPrefab, player.transform);
		ability.Initialize(player);
		return ability;
	}

	private void ProcessAbilityActivation()
	{
		if (InputUtils.MagicShield.IsDown)
		{
			if (!defenseAbility.IsRunning)
			{
				ActivateShield();
			}
			else
			{
				defenseAbility.Deactivate();
			}
		}
	}

	private bool GetIsMovementBlocked()
	{
		if (attackAbility.IsRunning)
		{
			return !attackAbility.CanMove;
		}
		return false;
	}

	private Ability GetRunningAbility()
	{
		if (!attackAbility.IsRunning)
		{
			if (!defenseAbility.IsRunning)
			{
				return null;
			}
			return defenseAbility;
		}
		return attackAbility;
	}

	private int GetAnimParam()
	{
		Ability runningAbility = GetRunningAbility();
		if (!(runningAbility != null))
		{
			return 0;
		}
		return runningAbility.AnimParam;
	}

	private bool GetIsInjuryBlockRequest()
	{
		Ability runningAbility = GetRunningAbility();
		if (runningAbility != null)
		{
			return !runningAbility.CanMove;
		}
		return false;
	}
}
