using App.Camera;
using App.GUI;
using App.Player.Definition;
using App.Spawn;
using App.Util;
using App.Vehicles;
using App.Vehicles.Bicycle;
using App.Vehicles.Car.Navigation;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace App.Player
{
    public abstract class AbstractVehicleDriver : MonoBehaviour, IResetable
    {
        public bool isPlayer;

        protected ShotController shotController;

        protected PlayerAnimatorHandler animatorHandler;

        protected Animator animator;

        private RigidbodyHelper rigidbodyHelper;

        private Collider _collider;

        private NavMeshObstacle obstacle;

        private CharacterControl characterControl;

        private RagdollHelper ragdollHelper;

        private VehicleHandleSensor vehicleHandleSensor;

        private hideDistance characterHider;

        protected CameraManager cameraManager;

        protected PanelsManager panelsManager;

        protected VehicleComponents vehicleComponents;

        protected bool running;

        protected bool moveTowardVehicleHandle;

        protected bool onlyThrowOutDriver;

        protected bool isPassenger;

        private float initialVehicleHealth;

        private AnimatorEventBugFix eventBugFix = new AnimatorEventBugFix(1);

        public bool SittingInVehicle
        {
            get
            {
                if (running)
                {
                    return SittingIn();
                }
                return false;
            }
        }

        public VehicleComponents Vehicle => vehicleComponents;

        public VehicleType DriverType => GetVehicleType();

        public event Action BeforeRun;

        public event Action AfterRun;

        public event Action AfterStop;

        public event Action OnKickOutDriver;

        public event Action OnKickedOutOffVehicle;

        protected abstract VehicleType GetVehicleType();

        protected abstract AnimatorState GetSittingState();

        protected abstract AnimatorState GetThrowOutDriverState();

        protected abstract bool GetUseVehicleParameter();

        protected abstract void SetUseVehicleParameter(bool useVehicle);

        protected virtual void ClearBeforeRun()
        {
            eventBugFix.Clear();
        }

        protected virtual void OnHandleKickOutOffVehicle(bool relocateCharacter, bool relocateForward)
        {
        }

        protected virtual void OnResetStates()
        {
        }

        public void ResetStates()
        {
            SetVehicled(vehicled: false);
            onlyThrowOutDriver = false;
            OnResetStates();
        }

        public void OnThrowOutDriver()
        {
            if (eventBugFix.SetFiredAndTest(0) && running)
            {
                AbstractVehicleDriver x = vehicleComponents.KickOffCurrentDriver();
                if (this.OnKickOutDriver != null && x != null)
                {
                    this.OnKickOutDriver();
                }
            }
        }

        public virtual bool Runnable(bool useTargetTrigger = false)
        {
            bool flag = useTargetTrigger ? vehicleHandleSensor.TargetTriggered() : vehicleHandleSensor.Triggered();
            VehicleComponents triggeredVehicle = GetTriggeredVehicle();
            if (triggeredVehicle != null && triggeredVehicle.type != GetVehicleType())
            {
                return false;
            }
            if (!running && flag)
            {
                return characterControl.Grounded;
            }
            return false;
        }

        public void Run(bool onlyThrowOutDriver = false, bool useTargetTrigger = false, bool isPassenger = false)
        {
            if (!Runnable(useTargetTrigger))
            {
                return;
            }
            Component component = useTargetTrigger ? vehicleHandleSensor.GetTargetTrigger() : vehicleHandleSensor.GetTrigger();
            VehicleComponents component2 = component.transform.root.GetComponent<VehicleComponents>();
            DoorReservator doorReservator = isPassenger ? component2.passengerDoorReservator : component2.doorReservator;
            if (!(component2 == null) && component2.type == GetVehicleType() && (!(doorReservator != null) || !doorReservator.Reserved))
            {
                if (this.BeforeRun != null)
                {
                    this.BeforeRun();
                }
                ClearBeforeRun();
                this.onlyThrowOutDriver = onlyThrowOutDriver;
                this.isPassenger = isPassenger;
                animatorHandler.MirrorGetInVehicle = (component.transform != component2.handleTrigger);
                if (doorReservator != null)
                {
                    doorReservator.Reserve(base.transform);
                }
                moveTowardVehicleHandle = true;
                if (component2.type == VehicleType.Bicycle || component2.type == VehicleType.Gyroboard || component2.type == VehicleType.Skateboard)
                {
                    animatorHandler.ThrowOutDriver = (component2.streetVehicleModesHelper.Mode == StreetVehicleMode.Player || component2.streetVehicleModesHelper.Mode == StreetVehicleMode.AI);
                }
                else if (!isPassenger)
                {
                    animatorHandler.ThrowOutDriver = (component2.vehicleModesHandler != null && (component2.vehicleModesHandler.mode == VehicleMode.AI || component2.vehicleModesHandler.mode == VehicleMode.Player));
                }
                SetVehicled(vehicled: true, component2);
                if (this.AfterRun != null)
                {
                    this.AfterRun();
                }
            }
        }

        public void RunDirectlySitting(VehicleComponents vehicleComponents)
        {
            if (this.BeforeRun != null)
            {
                this.BeforeRun();
            }
            ClearBeforeRun();
            isPassenger = false;
            SetUseVehicleParameter(useVehicle: true);
            base.transform.position = vehicleComponents.sitPoint.position;
            base.transform.rotation = vehicleComponents.sitPoint.rotation;
            GetSittingState().RunPrompt();
            SetVehicled(vehicled: true, vehicleComponents);
            vehicleComponents.driver = base.transform;
        }

        public virtual bool Running()
        {
            return running;
        }

        public virtual void Stop()
        {
            if (!moveTowardVehicleHandle && !SittingIn() && !GetThrowOutDriverState().Running)
            {
                return;
            }
            if (SittingIn())
            {
                DoorReservator doorReservator = isPassenger ? vehicleComponents.passengerDoorReservator : vehicleComponents.doorReservator;
                if (!GetUseVehicleParameter() || doorReservator.Reserved)
                {
                    return;
                }
                doorReservator.Reserve(base.transform);
            }
            if (characterHider != null)
            {
                characterHider.ResetStats();
            }
            if (vehicleComponents.playerGarbageTruckControl != null)
            {
                GarbageCapacity.Instance.Deactivate();
            }
            moveTowardVehicleHandle = false;
            SetUseVehicleParameter(useVehicle: false);
        }

        public void HandleKickOutOffVehicle(bool relocateCharacter = true, bool relocateForward = false)
        {
            if (relocateCharacter)
            {
                Transform transform = isPassenger ? vehicleComponents.passengerHandleTrigger : vehicleComponents.handleTrigger;
                Vector3 zero = Vector3.zero;
                zero = ((!relocateForward) ? (isPassenger ? (base.transform.right * 0.5f) : (base.transform.right * -0.5f)) : (base.transform.forward * 2f + base.transform.up * 1f));
                base.transform.position = transform.position + zero;
            }
            SetVehicled(vehicled: false, null, !isPlayer);
            ragdollHelper.Ragdolled = true;
            if (isPlayer && !cameraManager.RunningWreckCamera())
            {
                cameraManager.SetPlayerCamera();
                panelsManager.ShowPanel(PanelType.Game);
            }
            if (this.OnKickedOutOffVehicle != null)
            {
                this.OnKickedOutOffVehicle();
            }
            OnHandleKickOutOffVehicle(relocateCharacter, relocateForward);
        }

        public bool ReceivedEnoughDamage()
        {
            if (running && !isPlayer && GetUseVehicleParameter() && vehicleComponents.health != null)
            {
                return initialVehicleHealth - vehicleComponents.health.GetCurrentHealth() > 0.1f;
            }
            return false;
        }

        protected virtual bool SittingIn()
        {
            return GetSittingState().Running;
        }

        protected virtual void Awake()
        {
            shotController = GetComponent<ShotController>();
            animatorHandler = this.GetComponentSafe<PlayerAnimatorHandler>();
            animator = this.GetComponentSafe<Animator>();
            rigidbodyHelper = this.GetComponentSafe<RigidbodyHelper>();
            _collider = this.GetComponentSafe<Collider>();
            characterControl = this.GetComponentSafe<CharacterControl>();
            obstacle = GetComponent<NavMeshObstacle>();
            ragdollHelper = this.GetComponentSafe<RagdollHelper>();
            vehicleHandleSensor = this.GetComponentSafe<VehicleHandleSensor>();
            characterHider = GetComponent<hideDistance>();
            if (isPlayer)
            {
                cameraManager = ServiceLocator.Get<CameraManager>();
                panelsManager = ServiceLocator.Get<PanelsManager>();
            }
        }

        protected virtual void SetVehicled(bool vehicled, VehicleComponents vehicleComponents = null, bool fixRotation = true)
        {
            if (vehicled)
            {
                running = true;
                base.transform.parent = (isPassenger ? vehicleComponents.passengerSitPoint.transform : vehicleComponents.sitPoint.transform);
                if (!isPlayer && vehicleComponents.health != null)
                {
                    initialVehicleHealth = vehicleComponents.health.GetCurrentHealth();
                }
                this.vehicleComponents = vehicleComponents;
            }
            else
            {
                running = false;
                base.transform.parent = null;
                SetUseVehicleParameter(useVehicle: false);
                moveTowardVehicleHandle = false;
                if ((bool)this.vehicleComponents)
                {
                    DoorReservator doorReservator = isPassenger ? this.vehicleComponents.passengerDoorReservator : this.vehicleComponents.doorReservator;
                    if (doorReservator != null)
                    {
                        doorReservator.ReleaseReservation(base.transform);
                    }
                    if (base.transform.Equals(this.vehicleComponents.driver))
                    {
                        this.vehicleComponents.driver = null;
                        if (this.vehicleComponents.vehicleModesHandler != null)
                        {
                            this.vehicleComponents.vehicleModesHandler.SetMode(VehicleMode.Empty);
                        }
                        if (this.vehicleComponents.streetVehicleModesHelper != null)
                        {
                            this.vehicleComponents.streetVehicleModesHelper.SetEmptyStanding();
                        }
                    }
                    else if (base.transform.Equals(this.vehicleComponents.passenger))
                    {
                        this.vehicleComponents.passenger = null;
                    }
                }
                if (fixRotation)
                {
                    base.transform.rotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
                }
            }
            if (isPlayer && this.vehicleComponents != null)
            {
                NavMeshObstacle component = this.vehicleComponents.GetComponent<NavMeshObstacle>();
                if (component != null)
                {
                    component.carving = vehicled;
                }
            }
            rigidbodyHelper.SetKinematic(vehicled, this);
            _collider.enabled = !vehicled;
            characterControl.enabled = !vehicled;
            if (obstacle != null)
            {
                obstacle.enabled = !vehicled;
            }
            if (!vehicled)
            {
                TryInvokeAfterStop();
            }
            ReportDriverState(vehicled);
        }

        protected void TryInvokeAfterStop()
        {
            if (this.AfterStop != null)
            {
                this.AfterStop();
            }
        }

        protected VehicleComponents GetTriggeredVehicle()
        {
            Component trigger = vehicleHandleSensor.GetTrigger();
            if (trigger == null)
            {
                return null;
            }
            return trigger.transform.root.GetComponent<VehicleComponents>();
        }

        private void ReportDriverState(bool isVehicled)
        {
            if (isPlayer)
            {
                ServiceLocator.SendMessage(isVehicled ? MessageID.Player.VehicleEnter : MessageID.Player.VehicleLeave, this);
            }
        }
    }
}
