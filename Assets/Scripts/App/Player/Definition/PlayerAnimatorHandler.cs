using App.Spawn;
using App.Util;
using UnityEngine;

namespace App.Player.Definition
{
	public class PlayerAnimatorHandler : MonoBehaviour, IResetable
	{
		private WingsAnimationHandler wingsAnimatorHandler;

		private int flyTypeHash = Animator.StringToHash("FlyType");

		private int forwardHash = Animator.StringToHash("Forward");

		private int idleBlendHash = Animator.StringToHash("IdleBlend");

		private int glideHash = Animator.StringToHash("Glide");

		private int glideYHash = Animator.StringToHash("GlideY");

		private int fightHash = Animator.StringToHash("Fight");

		private int kickHash = Animator.StringToHash("Kick");

		private int punchHash = Animator.StringToHash("Punch");

		private int gunHash = Animator.StringToHash("Gun");

		private int loopGunFireHash = Animator.StringToHash("LoopGunFire");

		private int useCarHash = Animator.StringToHash("UseCar");

		private int useBikeHash = Animator.StringToHash("UseBike");

		private int useBicycleHash = Animator.StringToHash("UseBicycle");

		private int useGyroboardHash = Animator.StringToHash("UseGyroboard");

		private int useSkateboardHash = Animator.StringToHash("UseSkateboard");

		private int skaterYOffsetHash = Animator.StringToHash("SkaterYOffset");

		private int skaterInAirHash = Animator.StringToHash("SkaterInAir");

		private int skaterDirectionHash = Animator.StringToHash("SkaterDirection");

		private int mirrorGetInVehicleHash = Animator.StringToHash("MirrorGetInVehicle");

		private int useHelicopterHash = Animator.StringToHash("UseHelicopter");

		private int useTankHash = Animator.StringToHash("UseTank");

		private int throwOutDriverHash = Animator.StringToHash("ThrowOutDriver");

		private int climbHash = Animator.StringToHash("Climb");

		private int climbMoveHash = Animator.StringToHash("ClimbMove");

		private int climbExitBlendHash = Animator.StringToHash("ClimbExitBlend");

		private int fastClimbExitHash = Animator.StringToHash("FastClimbExit");

		private int pullToRopeHash = Animator.StringToHash("PullToRope");

		private int pullRopeHash = Animator.StringToHash("PullRope");

		private int rollHash = Animator.StringToHash("Roll");

		private int flyHash = Animator.StringToHash("Fly");

		private int flyXHash = Animator.StringToHash("FlyX");

		private int flyZHash = Animator.StringToHash("FlyZ");

		private int flyYHash = Animator.StringToHash("FlyY");

		private int rescueHash = Animator.StringToHash("Rescue");

		private int investigateHash = Animator.StringToHash("Investigate");

		private int sitOnBicycleBlendHash = Animator.StringToHash("SitOnBicycleBlend");

		private int jumpHeight = Animator.StringToHash("JumpHeight");

		private int onGroundHash = Animator.StringToHash("OnGround");

		private int continueFightComboHash = Animator.StringToHash("ContinueFightCombo");

		private int magicShieldHash = Animator.StringToHash("MagicShield");

		private int frontFlipHash = Animator.StringToHash("FrontFlip");

		private int standUpHash = Animator.StringToHash("StandUp");

		private int shotHash = Animator.StringToHash("Shot");

		private int reloadHash = Animator.StringToHash("Reload");

		private int throwGrenadeHash = Animator.StringToHash("ThrowGrenade");

		private int startSkateboardHash = Animator.StringToHash("StartSkateboard");

		private int skaterPushHash = Animator.StringToHash("SkaterPush");

		private AnimatorState nullState;

		private AnimatorState frontFlip;

		private AnimatorState airborneState;

		private AnimatorState groundedState;

		private AnimatorState fightIdleState;

		private AnimatorState punchState;

		private AnimatorState standUpFromBackState;

		private AnimatorState standUpFromBellyState;

		private AnimatorState openCarDoorState;

		private AnimatorState getInCarState;

		private AnimatorState closeCarDoorFromInsideState;

		private AnimatorState sittingInCarState;

		private AnimatorState getOutOffCarState;

		private AnimatorState closeCarDoorState;

		private AnimatorState throwOutDriverState;

		private AnimatorState getOnBikeState;

		private AnimatorState sittingOnBikeState;

		private AnimatorState getOffBikeState;

		private AnimatorState throwOutBikeDriverState;

		private AnimatorState sittingInHelicopterState;

		private AnimatorState sittingInTankState;

		private AnimatorState getOnBicycleState;

		private AnimatorState sittingOnBicycleState;

		private AnimatorState getOffBicycleState;

		private AnimatorState throwOutBicycleDriverState;

		private AnimatorState magicShield1State;

		private AnimatorState magicShield2State;

		private AnimatorMachine fightMachine;

		private AnimatorMachine carMachine;

		private AnimatorMachine bikeMachine;

		private AnimatorMachine bicycleMachine;

		private AnimatorMachine magicMachine;

		private Animator animator;

		private AnimatorState climbMoveState;

		private AnimatorState climbIdleState;

		private AnimatorState pullToRopeState;

		private AnimatorState pullRopeState;

		private AnimatorMachine climbMachine;

		public Animator Animator => animator;

		public float FlyType
		{
			get
			{
				return animator.GetFloat(flyTypeHash);
			}
			set
			{
				animator.SetFloat(flyTypeHash, value);
			}
		}

		public AutoBlend WalkTreeBlend
		{
			get;
			private set;
		}

		public float Forward
		{
			get
			{
				return animator.GetFloat(forwardHash);
			}
			set
			{
				animator.SetFloat(forwardHash, value);
			}
		}

		public AutoBlend StrafeX
		{
			get;
			private set;
		}

		public AutoBlend StrafeY
		{
			get;
			private set;
		}

		public float IdleBlend
		{
			get
			{
				return animator.GetFloat(idleBlendHash);
			}
			set
			{
				animator.SetFloat(idleBlendHash, value);
			}
		}

		public AutoBlend IdleSubBlend
		{
			get;
			private set;
		}

		public bool Glide
		{
			get
			{
				return animator.GetBool(glideHash);
			}
			set
			{
				animator.SetBool(glideHash, value);
				if ((bool)wingsAnimatorHandler)
				{
					wingsAnimatorHandler.Glide = value;
				}
			}
		}

		public AutoBlend GlideY
		{
			get;
			private set;
		}

		public bool Fight
		{
			get
			{
				return animator.GetBool(fightHash);
			}
			set
			{
				animator.SetBool(fightHash, value);
			}
		}

		public bool Punch
		{
			get
			{
				return animator.GetBool(punchHash);
			}
			set
			{
				animator.SetBool(punchHash, value);
			}
		}

		public bool Kick
		{
			get
			{
				return animator.GetBool(kickHash);
			}
			set
			{
				animator.SetBool(kickHash, value);
			}
		}

		public GunType Gun
		{
			get
			{
				return (GunType)animator.GetFloat(punchHash);
			}
			set
			{
				animator.SetFloat(gunHash, (float)value);
			}
		}

		public bool LoopGunFire
		{
			get
			{
				return animator.GetBool(loopGunFireHash);
			}
			set
			{
				animator.SetBool(loopGunFireHash, value);
			}
		}

		public bool UseCar
		{
			get
			{
				return animator.GetBool(useCarHash);
			}
			set
			{
				animator.SetBool(useCarHash, value);
				if ((bool)wingsAnimatorHandler)
				{
					wingsAnimatorHandler.UseCar = value;
				}
			}
		}

		public bool UseBike
		{
			get
			{
				return animator.GetBool(useBikeHash);
			}
			set
			{
				animator.SetBool(useBikeHash, value);
			}
		}

		public bool UseBicycle
		{
			get
			{
				return animator.GetBool(useBicycleHash);
			}
			set
			{
				animator.SetBool(useBicycleHash, value);
			}
		}

		public bool UseGyroboard
		{
			get
			{
				return animator.GetBool(useGyroboardHash);
			}
			set
			{
				animator.SetBool(useGyroboardHash, value);
			}
		}

		public bool UseSkateboard
		{
			get
			{
				return animator.GetBool(useSkateboardHash);
			}
			set
			{
				animator.SetBool(useSkateboardHash, value);
			}
		}

		public float SkaterYOffset => animator.GetFloat(skaterYOffsetHash);

		public bool SkaterInAir
		{
			get
			{
				return animator.GetBool(skaterInAirHash);
			}
			set
			{
				animator.SetBool(skaterInAirHash, value);
			}
		}

		public AutoBlend SkaterDirection
		{
			get;
			private set;
		}

		public bool MirrorGetInVehicle
		{
			get
			{
				return animator.GetBool(mirrorGetInVehicleHash);
			}
			set
			{
				animator.SetBool(mirrorGetInVehicleHash, value);
			}
		}

		public bool UseHelicopter
		{
			get
			{
				return animator.GetBool(useHelicopterHash);
			}
			set
			{
				animator.SetBool(useHelicopterHash, value);
			}
		}

		public bool UseTank
		{
			get
			{
				return animator.GetBool(useTankHash);
			}
			set
			{
				animator.SetBool(useTankHash, value);
			}
		}

		public bool ThrowOutDriver
		{
			get
			{
				return animator.GetBool(throwOutDriverHash);
			}
			set
			{
				animator.SetBool(throwOutDriverHash, value);
			}
		}

		public bool Climb
		{
			get
			{
				return animator.GetBool(climbHash);
			}
			set
			{
				animator.SetBool(climbHash, value);
			}
		}

		public bool ClimbMove
		{
			get
			{
				return animator.GetBool(climbMoveHash);
			}
			set
			{
				animator.SetBool(climbMoveHash, value);
			}
		}

		public float ClimbExitBlend
		{
			get
			{
				return animator.GetFloat(climbExitBlendHash);
			}
			set
			{
				animator.SetFloat(climbExitBlendHash, value);
			}
		}

		public bool FastClimbExit
		{
			get
			{
				return animator.GetBool(fastClimbExitHash);
			}
			set
			{
				animator.SetBool(fastClimbExitHash, value);
			}
		}

		public bool PullToRope
		{
			get
			{
				return animator.GetBool(pullToRopeHash);
			}
			set
			{
				animator.SetBool(pullToRopeHash, value);
			}
		}

		public bool PullRope
		{
			get
			{
				return animator.GetBool(pullRopeHash);
			}
			set
			{
				animator.SetBool(pullRopeHash, value);
			}
		}

		public bool Roll
		{
			get
			{
				return animator.GetBool(rollHash);
			}
			set
			{
				animator.SetBool(rollHash, value);
			}
		}

		public bool Fly
		{
			get
			{
				return animator.GetBool(flyHash);
			}
			set
			{
				animator.SetBool(flyHash, value);
				if ((bool)wingsAnimatorHandler)
				{
					wingsAnimatorHandler.Fly = value;
				}
			}
		}

		public float FlyX
		{
			get
			{
				return animator.GetFloat(flyXHash);
			}
			set
			{
				animator.SetFloat(flyXHash, value);
				if ((bool)wingsAnimatorHandler)
				{
					wingsAnimatorHandler.FlyX = value;
				}
			}
		}

		public float FlyZ
		{
			get
			{
				return animator.GetFloat(flyZHash);
			}
			set
			{
				animator.SetFloat(flyZHash, value);
				if ((bool)wingsAnimatorHandler)
				{
					wingsAnimatorHandler.FlyZ = value;
				}
			}
		}

		public float FlyY
		{
			get
			{
				return animator.GetFloat(flyYHash);
			}
			set
			{
				animator.SetFloat(flyYHash, value);
				if ((bool)wingsAnimatorHandler)
				{
					wingsAnimatorHandler.FlyY = value;
				}
			}
		}

		public bool Rescue
		{
			get
			{
				return animator.GetBool(rescueHash);
			}
			set
			{
				animator.SetBool(rescueHash, value);
			}
		}

		public bool Investigate
		{
			get
			{
				return animator.GetBool(investigateHash);
			}
			set
			{
				animator.SetBool(investigateHash, value);
			}
		}

		public AutoBlend InvestigateBlend
		{
			get;
			private set;
		}

		public float SItOnBicycleBlend
		{
			get
			{
				return animator.GetFloat(sitOnBicycleBlendHash);
			}
			set
			{
				animator.SetFloat(sitOnBicycleBlendHash, value);
			}
		}

		public float JumpHeight
		{
			get
			{
				return animator.GetFloat(jumpHeight);
			}
			set
			{
				animator.SetFloat(jumpHeight, value);
			}
		}

		public bool OnGround
		{
			get
			{
				return animator.GetBool(onGroundHash);
			}
			set
			{
				animator.SetBool(onGroundHash, value);
				if ((bool)wingsAnimatorHandler)
				{
					wingsAnimatorHandler.OnGround = value;
				}
			}
		}

		public bool ContinueFightCombo
		{
			get
			{
				return animator.GetBool(continueFightComboHash);
			}
			set
			{
				animator.SetBool(continueFightComboHash, value);
			}
		}

		public int MagicShield
		{
			get
			{
				return animator.GetInteger(magicShieldHash);
			}
			set
			{
				animator.SetInteger(magicShieldHash, value);
			}
		}

		public AnimatorState NullState => nullState;

		public AnimatorState FrontFlipState => frontFlip;

		public AnimatorState AirborneState => airborneState;

		public AnimatorState GroundedState => groundedState;

		public AnimatorState FightIdleState => fightIdleState;

		public AnimatorState PunchState => punchState;

		public AnimatorState StandUpFromBackState => standUpFromBackState;

		public AnimatorState StandUpFromBellyState => standUpFromBellyState;

		public AnimatorState GlideState
		{
			get;
			private set;
		}

		public AnimatorState RollFromGlideState
		{
			get;
			private set;
		}

		public AnimatorState OpenCarDoorState => openCarDoorState;

		public AnimatorState GetInCarState => getInCarState;

		public AnimatorState CloseCarDoorFromInsideState => closeCarDoorFromInsideState;

		public AnimatorState SittingInCarState => sittingInCarState;

		public AnimatorState GetOutOffCarState => getOutOffCarState;

		public AnimatorState CloseCarDoorState => closeCarDoorState;

		public AnimatorState ThrowOutDriverState => throwOutDriverState;

		public AnimatorState GetOnBikeState => getOnBikeState;

		public AnimatorState SittingOnBikeState => sittingOnBikeState;

		public AnimatorState GetOffBikeState => getOffBikeState;

		public AnimatorState ThrowOutBikeDriverState => throwOutBikeDriverState;

		public AnimatorState GetOnGyroboardState
		{
			get;
			private set;
		}

		public AnimatorState StandingOnGyroboardState
		{
			get;
			private set;
		}

		public AnimatorState GetOffGyroboardState
		{
			get;
			private set;
		}

		public AnimatorState ThrowOutGyroboardDriverState
		{
			get;
			private set;
		}

		public AnimatorState GetOnSkateboardState
		{
			get;
			private set;
		}

		public AnimatorState SkateboardGroundedState
		{
			get;
			private set;
		}

		public AnimatorState SkateboardPushState
		{
			get;
			private set;
		}

		public AnimatorState SkateboardJumpState
		{
			get;
			private set;
		}

		public AnimatorState SkateboardFallState
		{
			get;
			private set;
		}

		public AnimatorState SkateboardLandState
		{
			get;
			private set;
		}

		public AnimatorState GetOffSkateboardState
		{
			get;
			private set;
		}

		public AnimatorState ThrowOutSkateboardDriverState
		{
			get;
			private set;
		}

		public AnimatorState SittingInHelicopterState => sittingInHelicopterState;

		public AnimatorState SittingInTankState => sittingInTankState;

		public AnimatorState GetOnBicycleState => getOnBicycleState;

		public AnimatorState SittingOnBicycleState => sittingOnBicycleState;

		public AnimatorState GetOffBicycleState => getOffBicycleState;

		public AnimatorState ThrowOutBicycleDriverState => throwOutBicycleDriverState;

		public AnimatorState ClimbIdleState => climbIdleState;

		public AnimatorState ClimbMoveState => climbMoveState;

		public AnimatorState PullToRopeState => pullToRopeState;

		public AnimatorState PullRopeState => pullRopeState;

		public AnimatorState FlyState
		{
			get;
			private set;
		}

		public AnimatorState GrenadeIdleState
		{
			get;
			private set;
		}

		public AnimatorState GrenadeThrowState
		{
			get;
			private set;
		}

		public AnimatorMachine FightMachine => fightMachine;

		public AnimatorMachine CarMachine => carMachine;

		public AnimatorMachine BikeMachine => bikeMachine;

		public AnimatorMachine ClimbMachine => climbMachine;

		public AnimatorMachine MagicMachine => magicMachine;

		public float TorsoGunLayerWeight
		{
			get
			{
				return animator.GetLayerWeight(1);
			}
			set
			{
				animator.SetLayerWeight(1, value);
			}
		}

		public float LaserHandLayerWeight
		{
			get
			{
				return animator.GetLayerWeight(2);
			}
			set
			{
				animator.SetLayerWeight(2, value);
			}
		}

		public float GrenadeLayerWeight
		{
			get
			{
				return animator.GetLayerWeight(3);
			}
			set
			{
				animator.SetLayerWeight(3, value);
			}
		}

		public void TriggerFrontFlip()
		{
			animator.SetTrigger(frontFlipHash);
		}

		public void TriggerStandUp()
		{
			animator.SetTrigger(standUpHash);
		}

		public void TriggerShot()
		{
			animator.SetTrigger(shotHash);
		}

		public void TriggerReload()
		{
			animator.SetTrigger(reloadHash);
		}

		public void TriggerThrowGrenade()
		{
			animator.SetTrigger(throwGrenadeHash);
		}

		public void TriggerStartSkateboard()
		{
			animator.SetTrigger(startSkateboardHash);
		}

		public void TriggerSkaterPush()
		{
			animator.SetTrigger(skaterPushHash);
		}

		public void ResetStates()
		{
			animator.enabled = true;
			groundedState.RunPrompt();
		}

		protected void Awake()
		{
			wingsAnimatorHandler = GetComponent<WingsAnimationHandler>();
			animator = this.GetComponentInChildrenSafe<Animator>();
			nullState = new AnimatorState("Base Layer.NullState", animator);
			groundedState = new AnimatorState("Base Layer.Grounded", animator);
			airborneState = new AnimatorState("Base Layer.Airborne", animator);
			frontFlip = new AnimatorState("Base Layer.FrontFlip", animator);
			standUpFromBackState = new AnimatorState("Base Layer.StandUpFromBack", animator);
			standUpFromBellyState = new AnimatorState("Base Layer.StandUpFromBelly", animator);
			GlideState = new AnimatorState("Base Layer.GlideMachine.Glide", animator);
			RollFromGlideState = new AnimatorState("Base Layer.GlideMachine.RollFromGlide", animator);
			fightIdleState = new AnimatorState("Base Layer.FightMachine.FightIdle", animator);
			punchState = new AnimatorState("Base Layer.FightMachine.Punch", animator);
			fightMachine = new AnimatorMachine(fightIdleState, punchState);
			magicShield1State = new AnimatorState("Base Layer.Magic.Magic1", animator);
			magicShield2State = new AnimatorState("Base Layer.Magic.Magic2", animator);
			magicMachine = new AnimatorMachine(magicShield1State, magicShield2State);
			openCarDoorState = new AnimatorState("Base Layer.CarMachine.OpenCarDoor", animator);
			getInCarState = new AnimatorState("Base Layer.CarMachine.GetInCar", animator);
			closeCarDoorFromInsideState = new AnimatorState("Base Layer.CarMachine.CloseCarDoorFromInside", animator);
			sittingInCarState = new AnimatorState("Base Layer.CarMachine.SittingInCar", animator);
			getOutOffCarState = new AnimatorState("Base Layer.CarMachine.GetOutOffCar", animator);
			closeCarDoorState = new AnimatorState("Base Layer.CarMachine.CloseCarDoor", animator);
			throwOutDriverState = new AnimatorState("Base Layer.CarMachine.ThrowOutDriver", animator);
			carMachine = new AnimatorMachine(openCarDoorState, getInCarState, closeCarDoorFromInsideState, sittingInCarState, getOutOffCarState, closeCarDoorState, throwOutDriverState);
			getOnBikeState = new AnimatorState("Base Layer.BikeMachine.GetOnBike", animator);
			sittingOnBikeState = new AnimatorState("Base Layer.BikeMachine.SittingOnBike", animator);
			getOffBikeState = new AnimatorState("Base Layer.BikeMachine.GetOffBike", animator);
			throwOutBikeDriverState = new AnimatorState("Base Layer.BikeMachine.ThrowOutDriver", animator);
			bikeMachine = new AnimatorMachine(getOnBikeState, sittingOnBikeState, getOffBikeState, throwOutBikeDriverState);
			sittingInHelicopterState = new AnimatorState("Base Layer.SittingInHelicopter", animator);
			sittingInHelicopterState = new AnimatorState("Base Layer.SittingInTank", animator);
			GetOnGyroboardState = new AnimatorState("Base Layer.GyroboardMachine.GetOnGyroboard", animator);
			StandingOnGyroboardState = new AnimatorState("Base Layer.GyroboardMachine.StandingOnGyroboard", animator);
			GetOffGyroboardState = new AnimatorState("Base Layer.GyroboardMachine.GetOffGyroboard", animator);
			ThrowOutGyroboardDriverState = new AnimatorState("Base Layer.GyroboardMachine.ThrowOutDriver", animator);
			GetOnSkateboardState = new AnimatorState("Base Layer.SkateboardMachine.GetOnSkateboard", animator);
			SkateboardGroundedState = new AnimatorState("Base Layer.SkateboardMachine.SkateboardGrounded", animator);
			SkateboardPushState = new AnimatorState("Base Layer.SkateboardMachine.SkateboardPush", animator);
			SkateboardJumpState = new AnimatorState("Base Layer.SkateboardMachine.SkateboardJump", animator);
			SkateboardFallState = new AnimatorState("Base Layer.SkateboardMachine.SkateboardFall", animator);
			SkateboardLandState = new AnimatorState("Base Layer.SkateboardMachine.SkateboardLand", animator);
			GetOffSkateboardState = new AnimatorState("Base Layer.SkateboardMachine.GetOffSkateboard", animator);
			ThrowOutSkateboardDriverState = new AnimatorState("Base Layer.SkateboardMachine.ThrowOutDriver", animator);
			getOnBicycleState = new AnimatorState("Base Layer.BicycleMachine.GetOnBicycle", animator);
			sittingOnBicycleState = new AnimatorState("Base Layer.BicycleMachine.SittingOnBicycle", animator);
			getOffBicycleState = new AnimatorState("Base Layer.BicycleMachine.GetOffBicycle", animator);
			throwOutBicycleDriverState = new AnimatorState("Base Layer.BicycleMachine.ThrowOutDriver", animator);
			bicycleMachine = new AnimatorMachine(getOnBicycleState, sittingOnBicycleState, getOffBicycleState, throwOutBicycleDriverState);
			climbIdleState = new AnimatorState("Base Layer.ClimbMachine.ClimbIdle", animator);
			climbMoveState = new AnimatorState("Base Layer.ClimbMachine.ClimbMove", animator);
			climbMachine = new AnimatorMachine(climbIdleState, climbMoveState);
			pullToRopeState = new AnimatorState("Base Layer.RopeMachine.PullToRope", animator);
			pullRopeState = new AnimatorState("Base Layer.RopeMachine.PullRope", animator);
			FlyState = new AnimatorState("Base Layer.Fly", animator);
			GrenadeIdleState = new AnimatorState("GrenadeTorsoLayer.GrenadeIdle", animator, 3);
			GrenadeThrowState = new AnimatorState("GrenadeTorsoLayer.GrenadeThrow", animator, 3);
			WalkTreeBlend = new AutoBlend(Animator.StringToHash("WalkTree"), 8f, animator);
			StrafeX = new AutoBlend(Animator.StringToHash("StrafeX"), 8f, animator);
			StrafeY = new AutoBlend(Animator.StringToHash("StrafeY"), 8f, animator);
			IdleSubBlend = new AutoBlend(Animator.StringToHash("IdleSubBlend"), 8f, animator);
			InvestigateBlend = new AutoBlend(Animator.StringToHash("InvestigateBlend"), 2f, animator);
			GlideY = new AutoBlend(Animator.StringToHash("GlideY"), 2f, animator);
			SkaterDirection = new AutoBlend(Animator.StringToHash("SkaterDirection"), 2f, animator);
		}
	}
}
