using App.Interaction;
using App.Player.Definition;
using App.Player.FightSystem;
using App.Player.GrenadeThrow;
using App.Util;
using UnityEngine;

namespace App.Player
{
    [RequireComponent(typeof(CharacterControl))]
    public class PlayerController : MonoBehaviour
    {
        public bool kickEnabled = true;

        public CharacterMode runningMode;

        public bool controlled = true;

        private Rigidbody _rigidbody;

        private PlayerAnimatorHandler animatorHandler;

        private CharacterControl characterController;

        private CharacterModesHandler characterModesHandler;

        private Health health;

        private HitHandler hitHandler;

        private PlayerNavigator playerNavigator;

        private BikeDriver bikeDriver;

        private CarDriver carDriver;

        private BicycleDriver bicycleDriver;

        private GyroboardDriver gyroboardDriver;

        private SkateboardDriver skateboardDriver;

        private HelicopterDriver helicopterDriver;

        private TankDriver tankDriver;

        private MechDriver mechDriver;

        private AirplaneDriver airplaneDriver;

        private AdvancedFightController advancedFightController;

        private ShotController shotController;

        private GrenadeThrowController grenadeThrowController;

        private GlideController glideController;

        private ClimbController climbController;

        private RopeController ropeController;

        private FlyController flyController;

        private LaserEyesController laserEyesController;

        private WeaponInventory weaponInvetory;

        private MagicController magicController;

        private MagicShield magicShield;

        private RagdollHelper ragdollHelper;

        private CharacterRotator characterRotator;

        private Transform cameraTransform;

        private Vector3 cameraForward;

        private Vector3 move;

        private CapsuleCollider capsuleCollider;

        private PlayerMonitor playerMonitor;

        private bool jump;

        private bool crouch;

        private bool ragdolledInFightMode;

        private float colliderPosValue = 1f;

        private bool heightJumpBool;

        private float counter1;

        private float colPosition = 1f;

        private DurationTimer delayJumpTimer = new DurationTimer();

        private float timer = 1.5f;

        public bool Controlled
        {
            get
            {
                if (!health.Dead() && !ragdollHelper.Ragdolled && !hitHandler.Running())
                {
                    return controlled;
                }
                return false;
            }
        }

        private void Awake()
        {
            _rigidbody = this.GetComponentSafe<Rigidbody>();
            animatorHandler = this.GetComponentSafe<PlayerAnimatorHandler>();
            characterController = this.GetComponentSafe<CharacterControl>();
            characterModesHandler = this.GetComponentSafe<CharacterModesHandler>();
            health = this.GetComponentSafe<Health>();
            hitHandler = this.GetComponentSafe<HitHandler>();
            playerNavigator = this.GetComponentSafe<PlayerNavigator>();
            bikeDriver = this.GetComponentSafe<BikeDriver>();
            carDriver = this.GetComponentSafe<CarDriver>();
            bicycleDriver = this.GetComponentSafe<BicycleDriver>();
            gyroboardDriver = this.GetComponentSafe<GyroboardDriver>();
            skateboardDriver = this.GetComponentSafe<SkateboardDriver>();
            helicopterDriver = this.GetComponentSafe<HelicopterDriver>();
            tankDriver = this.GetComponentSafe<TankDriver>();
            mechDriver = this.GetComponentSafe<MechDriver>();
            airplaneDriver = this.GetComponentSafe<AirplaneDriver>();
            advancedFightController = this.GetComponentSafe<AdvancedFightController>();
            shotController = this.GetComponentSafe<ShotController>();
            grenadeThrowController = this.GetComponentSafe<GrenadeThrowController>();
            glideController = this.GetComponentSafe<GlideController>();
            climbController = GetComponent<ClimbController>();
            ropeController = GetComponent<RopeController>();
            flyController = GetComponent<FlyController>();
            laserEyesController = GetComponent<LaserEyesController>();
            weaponInvetory = this.GetComponentSafe<WeaponInventory>();
            magicController = GetComponent<MagicController>();
            magicShield = GetComponent<MagicShield>();
            ragdollHelper = this.GetComponentSafe<RagdollHelper>();
            characterRotator = GetComponent<CharacterRotator>();
            playerMonitor = GetComponent<PlayerMonitor>();
            bikeDriver.BeforeRun += BeforeVehicleControllerStart;
            carDriver.BeforeRun += BeforeVehicleControllerStart;
            bicycleDriver.BeforeRun += BeforeVehicleControllerStart;
            gyroboardDriver.BeforeRun += BeforeVehicleControllerStart;
            skateboardDriver.BeforeRun += BeforeVehicleControllerStart;
            helicopterDriver.BeforeRun += BeforeVehicleControllerStart;
            tankDriver.BeforeRun += BeforeVehicleControllerStart;
            mechDriver.BeforeRun += BeforeVehicleControllerStart;
            airplaneDriver.BeforeRun += BeforeVehicleControllerStart;
            playerNavigator.OnReachTargetHandle += OnReachTargetHandle;
            ragdollHelper.OnRaggdolled += OnRaggdolled;
            cameraTransform = UnityEngine.Camera.main.transform;
            capsuleCollider = GetComponent<CapsuleCollider>();
            colPosition = capsuleCollider.center.y;
        }

        private void OnDestroy()
        {
            bikeDriver.BeforeRun -= BeforeVehicleControllerStart;
            carDriver.BeforeRun -= BeforeVehicleControllerStart;
            bicycleDriver.BeforeRun -= BeforeVehicleControllerStart;
            gyroboardDriver.BeforeRun -= BeforeVehicleControllerStart;
            skateboardDriver.BeforeRun -= BeforeVehicleControllerStart;
            helicopterDriver.BeforeRun -= BeforeVehicleControllerStart;
            tankDriver.BeforeRun -= BeforeVehicleControllerStart;
            mechDriver.BeforeRun -= BeforeVehicleControllerStart;
            airplaneDriver.BeforeRun -= BeforeVehicleControllerStart;
            playerNavigator.OnReachTargetHandle -= OnReachTargetHandle;
            ragdollHelper.OnRaggdolled -= OnRaggdolled;
        }

        private void Update()
        {
            bool flag = !health.Dead() && !ragdollHelper.Ragdolled && !hitHandler.Running() && controlled;
            runningMode = characterModesHandler.GetRunningMode();
            if (!jump && flag)
            {
                jump = InputUtils.Jump.IsDown;
            }
            if (climbController != null && jump)
            {
                climbController.Stop();
            }
            if (jump)
            {
                heightJumpBool = true;
            }
            if (heightJumpBool)
            {
                HeightJump();
            }
            if (flag && InputUtils.Vehicle.IsDown)
            {
                CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_vehicle_button, () =>
                {
                    if (runningMode == CharacterMode.Car)
                    {
                        carDriver.Stop();
                    }
                    else if (runningMode == CharacterMode.Bike)
                    {
                        bikeDriver.Stop();
                    }
                    else if (runningMode == CharacterMode.Bicycle)
                    {
                        bicycleDriver.Stop();
                    }
                    else if (runningMode == CharacterMode.Gyroboard)
                    {
                        gyroboardDriver.Stop();
                    }
                    else if (runningMode == CharacterMode.Skateboard)
                    {
                        skateboardDriver.Stop();
                    }
                    else if (runningMode == CharacterMode.Helicopter)
                    {
                        helicopterDriver.Stop();
                    }
                    else if (runningMode == CharacterMode.Tank)
                    {
                        tankDriver.Stop();
                    }
                    else if (runningMode == CharacterMode.Mech)
                    {
                        mechDriver.Stop();
                    }
                    else if (runningMode == CharacterMode.Airplane)
                    {
                        airplaneDriver.Stop();
                    }
                    else if (!TryRunVehicle())
                    {
                        playerNavigator.NavigateToVehicle();
                    }
                });
            }
            bool flag2 = flag && InputUtils.Magic.IsDown;
            bool flag3 = flag && InputUtils.Attack.IsPressed;
            bool flag4 = kickEnabled && flag && InputUtils.Kick.IsPressed;
            bool flag5 = flag && InputUtils.Attack.IsDown;
            if ((bool)magicShield && magicShield.IsMovementBlocked)
            {
                flag2 = false;
                flag3 = false;
                flag4 = false;
                flag5 = false;
            }
            if (flag3 | flag2)
            {
                runningMode = characterModesHandler.GetRunningMode();
                if (runningMode == CharacterMode.Default)
                {
                    advancedFightController.Run();
                }
                else if (runningMode == CharacterMode.Fly && flag2)
                {
                    advancedFightController.Run();
                }
            }
            if (flag && InputUtils.Grenade.IsDown && grenadeThrowController.Runnable())
            {
                runningMode = characterModesHandler.GetRunningMode();
                if (runningMode == CharacterMode.Default || runningMode == CharacterMode.AdvancedFight || runningMode == CharacterMode.Gun || runningMode == CharacterMode.Fly)
                {
                    shotController.Stop();
                    advancedFightController.Stop();
                    grenadeThrowController.Run();
                }
            }
            float num = flag ? ETCInput.GetAxis("HorizontalJoystick") : 0f;
            float num2 = flag ? ETCInput.GetAxis("VerticalJoystick") : 0f;
            if ((bool)magicShield && magicShield.IsMovementBlocked)
            {
                num = 0f;
                num2 = 0f;
            }
            cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1f, 0f, 1f)).normalized;
            move = num2 * cameraForward + num * cameraTransform.right;
            move = base.transform.InverseTransformDirection(move);
            runningMode = characterModesHandler.GetRunningMode();
            if (climbController != null && move.z > 0.7f && (runningMode == CharacterMode.Fly || ((runningMode == CharacterMode.Default || runningMode == CharacterMode.Gun) && characterController.Grounded)) && climbController.HittingWall())
            {
                if (runningMode == CharacterMode.Fly)
                {
                    flyController.Stop();
                }
                else if (runningMode == CharacterMode.Gun)
                {
                    shotController.Stop();
                }
                climbController.Run();
            }
            bool flag6 = ropeController != null && flag && InputUtils.Rope.IsDown;
            bool flag7 = flyController != null && flag && InputUtils.Fly.IsDown;
            if ((bool)magicShield && magicShield.IsMovementBlocked)
            {
                flag7 = false;
                flag6 = false;
            }
            if (flag6 | flag7)
            {
                if (climbController != null)
                {
                    climbController.FastStop();
                }
                shotController.Stop();
                advancedFightController.Stop();
            }
            runningMode = characterModesHandler.GetRunningMode();
            if (runningMode == CharacterMode.Climb)
            {
                animatorHandler.OnGround = true;
            }

            if ((runningMode == CharacterMode.Default || runningMode == CharacterMode.AdvancedFight || runningMode == CharacterMode.Gun) && glideController.Runnable())
            {
                shotController.Stop();
                advancedFightController.Stop();
                glideController.Run();
            }
            if (ropeController != null)
            {
                runningMode = characterModesHandler.GetRunningMode();
                if (((runningMode == CharacterMode.Default || runningMode == CharacterMode.Glide) & flag6) && ropeController.TryFindTargetPosition(out RaycastHit hit))
                {
                    glideController.Stop();
                    ropeController.Run(hit);
                }
            }
            if (flyController != null)
            {
                runningMode = characterModesHandler.GetRunningMode();
                if (((runningMode == CharacterMode.Default || runningMode == CharacterMode.Glide) & flag7) && flyController.Runnable())
                {
                    glideController.Stop();
                    flyController.Run();
                }
            }
            characterController.BlockRotation = (_rigidbody.isKinematic || (ropeController != null && ropeController.Running()) || (flyController != null && flyController.Running()) || (climbController != null && climbController.Running()) || animatorHandler.RollFromGlideState.RunningNowOrNext);
            if (ropeController != null)
            {
                RopeResult ropeResult = ropeController.Control(flag6, climbController != null && climbController.Runnable());
                if (climbController != null && climbController.Runnable() && ropeResult.targetReached && ropeResult.hit.transform.gameObject.layer == 13)
                {
                    climbController.RunForced(ropeResult.hit);
                }
            }
            if (flyController != null)
            {
                bool upPressed = flag && InputUtils.FlyUp.IsPressed;
                bool downPressed = flag && InputUtils.FlyDown.IsPressed;
                if ((bool)magicShield && magicShield.IsMovementBlocked)
                {
                    flag7 = false;
                    upPressed = false;
                    downPressed = false;
                }
                flyController.Control(flag7, upPressed, downPressed, num, num2);
            }
            bool flag8 = false;
            if ((bool)magicController && flag2)
            {
                magicController.Run();
                flag8 = true;
            }
            if ((bool)magicController && !flag8)
            {
                flag8 = magicController.MagicRun();
            }
            if (!flag8)
            {
                advancedFightController.Control(flag3, flag5);
                shotController.Control(flag3, flag5);
            }
            if (laserEyesController != null && laserEyesController.Running() && advancedFightController.Running())
            {
                advancedFightController.Stop();
            }
            bool flag9 = flag && InputUtils.RunFast.IsPressed;
            if (((num != 0f || num2 != 0f) | flag6 | flag7 | flag9 | flag4 | flag3) || jump || ragdollHelper.Ragdolled)
            {
                playerNavigator.Interrupt();
            }
            if (flag && InputUtils.SwitchWeapon.IsDown)
            {
                weaponInvetory.SwitchGun();
            }
            float num3 = flag ? UnityEngine.Input.GetAxis("Mouse ScrollWheel") : 0f;
            if (num3 > 0f)
            {
                weaponInvetory.SwitchGun();
            }
            else if (num3 < 0f)
            {
                weaponInvetory.SwitchGunBack();
            }
            if (flag && InputUtils.Interact.IsDown)
            {
                CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_Interact, () =>
                {
                    InteractiveObject interactiveObject = playerMonitor.InteractiveObjectSensor.InteractiveObject;
                    if (interactiveObject != null)
                    {
                        interactiveObject.Interact();
                    }
                });
            }
            UpdateWalkTree(flag3);
        }

        private void FixedUpdate()
        {
            bool flag = !health.Dead() && !ragdollHelper.Ragdolled && controlled;
            float num = flag ? ETCInput.GetAxis("HorizontalJoystick") : 0f;
            float num2 = flag ? ETCInput.GetAxis("VerticalJoystick") : 0f;
            bool flag2 = flag && InputUtils.RunFast.IsPressed;
            if ((bool)magicShield && magicShield.IsMovementBlocked)
            {
                num = 0f;
                num2 = 0f;
            }
            if (climbController != null)
            {
                climbController.Control(num, num2);
            }
            Vector3 normalized = Vector3.Scale(cameraTransform.forward, new Vector3(1f, 0f, 1f)).normalized;
            Vector3 a = num2 * normalized + num * cameraTransform.right;
            glideController.Control(a);
            if (flag)
            {
                if (advancedFightController.Running())
                {
                    advancedFightController.UpdateStrafe(normalized, num, num2);
                }
                else
                {
                    animatorHandler.StrafeX.BlendTo(flag2 ? 0f : num);
                    animatorHandler.StrafeY.BlendTo(flag2 ? 1f : num2);
                }
            }
            else
            {
                animatorHandler.StrafeX.BlendTo(0f);
                animatorHandler.StrafeY.BlendTo(0f);
            }
            CharacterMode characterMode = characterModesHandler.GetRunningMode();
            if (characterMode == CharacterMode.Bike || characterMode == CharacterMode.Car || characterMode == CharacterMode.Bicycle || characterMode == CharacterMode.Helicopter || characterMode == CharacterMode.Climb)
            {
                return;
            }
            if (ragdollHelper.Ragdolled || ragdollHelper.StandingUp || !flag || (characterRotator != null && characterRotator.FixingRotation))
            {
                if (!playerNavigator.enabled)
                {
                    characterController.Move(Vector3.zero, runFast: false, crouch: false, jump: false, default(Vector3), Time.fixedDeltaTime);
                }
                return;
            }
            crouch = UnityEngine.Input.GetKey(KeyCode.C);
            if (UnityEngine.Input.GetKey(KeyCode.LeftShift))
            {
                a *= 0.5f;
            }
            if (!playerNavigator.enabled)
            {
                characterController.Move(a, flag2, crouch, jump, normalized, Time.fixedDeltaTime);
            }
            jump = false;
        }

        private void BeforeVehicleControllerStart()
        {
            advancedFightController.Stop();
            shotController.Stop();
            grenadeThrowController.Stop();
            if (climbController != null)
            {
                climbController.Stop();
            }
            if (ropeController != null)
            {
                ropeController.Stop();
            }
        }

        private void OnReachTargetHandle()
        {
            TryRunVehicle(useTargetTrigger: true);
        }

        private bool TryRunVehicle(bool useTargetTrigger = false)
        {
            bikeDriver.Run(onlyThrowOutDriver: false, useTargetTrigger);
            if (bikeDriver.Running())
            {
                return true;
            }
            carDriver.Run(onlyThrowOutDriver: false, useTargetTrigger);
            if (carDriver.Running())
            {
                return true;
            }
            bicycleDriver.Run(onlyThrowOutDriver: false, useTargetTrigger);
            if (bicycleDriver.Running())
            {
                return true;
            }
            gyroboardDriver.Run(onlyThrowOutDriver: false, useTargetTrigger);
            if (gyroboardDriver.Running())
            {
                return true;
            }
            skateboardDriver.Run(onlyThrowOutDriver: false, useTargetTrigger);
            if (skateboardDriver.Running())
            {
                return true;
            }
            helicopterDriver.Run(onlyThrowOutDriver: false, useTargetTrigger);
            if (helicopterDriver.Running())
            {
                return true;
            }
            mechDriver.Run(onlyThrowOutDriver: false, useTargetTrigger);
            if (mechDriver.Running())
            {
                return true;
            }
            airplaneDriver.Run(onlyThrowOutDriver: false, useTargetTrigger);
            if (airplaneDriver.Running())
            {
                return true;
            }
            tankDriver.Run(onlyThrowOutDriver: false, useTargetTrigger);
            return tankDriver.Running();
        }

        private void OnRaggdolled(bool ragdolled)
        {
            if (!ragdolled)
            {
                if (ragdolledInFightMode)
                {
                    advancedFightController.Run();
                    ragdolledInFightMode = false;
                }
                return;
            }
            if (flyController != null && flyController.Running())
            {
                flyController.Stop();
            }
            if (climbController != null && climbController.Running())
            {
                climbController.Stop();
            }
            if (glideController.Running())
            {
                glideController.Stop();
            }
            if (advancedFightController.Running())
            {
                advancedFightController.Stop();
                if (!health.Dead())
                {
                    ragdolledInFightMode = true;
                }
            }
        }

        private void UpdateWalkTree(bool attackPressed)
        {
            animatorHandler.WalkTreeBlend.Update(Time.deltaTime);
            animatorHandler.IdleSubBlend.Update(Time.deltaTime);
            animatorHandler.StrafeX.Update(Time.deltaTime);
            animatorHandler.StrafeY.Update(Time.deltaTime);
            float num = (laserEyesController != null && laserEyesController.Running()) ? 1f : ((advancedFightController.Running() && advancedFightController.NeedStrafeWalk()) ? 1f : ((!shotController.Running() || !attackPressed) ? 0f : 1f));
            if (animatorHandler.WalkTreeBlend.TargetValue != num)
            {
                animatorHandler.WalkTreeBlend.BlendTo(num);
            }
            int num2 = advancedFightController.Running() ? 1 : 0;
            if (animatorHandler.IdleSubBlend.TargetValue != (float)num2)
            {
                animatorHandler.IdleSubBlend.BlendTo(num2);
            }
        }

        private void HeightJump()
        {
            if (animatorHandler.AirborneState.Running || animatorHandler.FrontFlipState.Running)
            {
                colliderPosValue = animatorHandler.JumpHeight;
                if (colliderPosValue > colPosition)
                {
                    capsuleCollider.center = new Vector3(0f, colliderPosValue, 0f);
                }
                else if (colliderPosValue < colPosition + 0.5f)
                {
                    colliderPosValue = colPosition;
                }
                counter1 += 1f;
                if (!delayJumpTimer.Running())
                {
                    if (animatorHandler.AirborneState.Running)
                    {
                        timer = 0.5f;
                    }
                    else if (animatorHandler.FrontFlipState.Running)
                    {
                        timer = 1.3f;
                    }
                    delayJumpTimer.Run(timer);
                }
                if (delayJumpTimer.Running() && delayJumpTimer.Done())
                {
                    capsuleCollider.center = new Vector3(0f, colPosition, 0f);
                    heightJumpBool = false;
                    counter1 = 0f;
                    delayJumpTimer.Stop();
                }
            }
            else if (animatorHandler.OnGround && counter1 > 3f)
            {
                capsuleCollider.center = new Vector3(0f, colPosition, 0f);
                heightJumpBool = false;
                counter1 = 0f;
            }
        }

        public bool UseVehicle()
        {
            if (runningMode == CharacterMode.Car || runningMode == CharacterMode.Bike || runningMode == CharacterMode.Bicycle || runningMode == CharacterMode.Gyroboard || runningMode == CharacterMode.Skateboard || runningMode == CharacterMode.Helicopter || runningMode == CharacterMode.Tank || runningMode == CharacterMode.Mech)
            {
                return true;
            }
            return false;
        }
    }
}
