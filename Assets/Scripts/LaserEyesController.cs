using App;
using App.AI.Scanner;
using App.Player;
using App.Player.Definition;
using App.Util;
using UnityEngine;

public class LaserEyesController : MonoBehaviour
{
	private bool ociZaply;

	public bool laserFiresFromHand;

	public GameObject laser;

	private Transform virtualTarget;

	private Health hp;

	private RagdollHelper ragdollHelper;

	private EnergyScript energy;

	private bool min = true;

	private PlayerAnimatorHandler animatorHandler;

	private ClimbController climbController;

	private RopeController ropeController;

	private PlayerController playerController;

	public GameObject partiklyObj;

	public bool Running()
	{
		return ociZaply;
	}

	private void Start()
	{
		virtualTarget = ServiceLocator.GetGameObject("VirtualTarget").transform;
		hp = GetComponent<Health>();
		ragdollHelper = this.GetComponentSafe<RagdollHelper>();
		energy = this.GetComponentSafe<EnergyScript>();
		animatorHandler = this.GetComponentSafe<PlayerAnimatorHandler>();
		climbController = this.GetComponentSafe<ClimbController>();
		ropeController = GetComponent<RopeController>();
		playerController = this.GetComponentSafe<PlayerController>();
	}

	private void LateUpdate()
	{
		if (!playerController.controlled)
		{
			if (laserFiresFromHand)
			{
				animatorHandler.LaserHandLayerWeight = 0f;
				animatorHandler.IdleBlend = 0f;
			}
			return;
		}
		if (InputUtils.Eyes.IsPressed)
		{
			zapniOci();
		}
		else if (!InputUtils.Attack.IsPressed)
		{
			vypniOci();
		}
		if (ociZaply)
		{
			if (laserFiresFromHand)
			{
				animatorHandler.LaserHandLayerWeight = Mathf.Clamp01(animatorHandler.LaserHandLayerWeight + Time.deltaTime * 7f);
				animatorHandler.IdleBlend = animatorHandler.LaserHandLayerWeight;
			}
			AIScanner.LastNoiseTime = Time.time;
			return;
		}
		if (laserFiresFromHand)
		{
			animatorHandler.LaserHandLayerWeight = Mathf.Clamp01(animatorHandler.LaserHandLayerWeight - Time.deltaTime * 7f);
			animatorHandler.IdleBlend = animatorHandler.LaserHandLayerWeight;
		}
		if (energy.GetCurrentEnergy() >= 0.2f)
		{
			min = true;
		}
	}

	private void zapniOci()
	{
		if (playerController.UseVehicle() || climbController.Running() || (ropeController != null && ropeController.Running()))
		{
			return;
		}
		if ((double)energy.GetCurrentEnergy() < 0.05)
		{
			vypniOci();
			min = false;
		}
		else if (min)
		{
			energy.ConsumeLaserEnergy(Time.deltaTime);
			Vector3 worldPosition = new Vector3(virtualTarget.position.x, base.transform.position.y, virtualTarget.position.z);
			base.transform.LookAt(worldPosition);
			laser.transform.LookAt(virtualTarget.position);
			ociZaply = true;
			laser.SetActive(ociZaply);
			if (!laser.GetComponentInChildren<AudioSource>().isPlaying)
			{
				laser.GetComponentInChildren<AudioSource>().Play();
			}
			if (hp != null && (hp.Dead() || ragdollHelper.Ragdolled))
			{
				vypniOci();
			}
		}
	}

	public void vypniOci()
	{
		ociZaply = false;
		laser.SetActive(ociZaply);
	}

	public void partikly()
	{
		partiklyObj.SetActive(value: true);
	}

	public void vypniPartikly()
	{
		partiklyObj.SetActive(value: false);
	}
}
