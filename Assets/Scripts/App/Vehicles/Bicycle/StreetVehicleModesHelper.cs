using App.Vehicles.Gyroboard;
using App.Vehicles.Skateboard;
using UnityEngine;

namespace App.Vehicles.Bicycle
{
	public class StreetVehicleModesHelper : MonoBehaviour
	{
		public WheelCollider[] wheelHubs = new WheelCollider[2];

		public GameObject defaultColliders;

		public GameObject playerStateColliders;

		public float playerStateMass;

		public float playerStateDrag;

		public float playerStateAngularDrag;

		public float freeStateDrag;

		public float freeStateAngularDrag;

		private VehicleType vehicleType;

		private Rigidbody _rigidbody;

		private MonoBehaviour aIVehicleController;

		private MonoBehaviour playerVehicleController;

		private BicycleAnimator bicycleAnimator;

		private SkateboardAudio skateboardAudio;

		private SkateboardStuckManager skateboardStuckManager;

		private DurationTimer emptyStandingTimer = new DurationTimer();

		private float defaultMass;

		private float defaultDrag;

		private float defaultAngularDrag;

		public Vector3 center;

		public Vector3 tensor;

		public StreetVehicleMode Mode
		{
			get;
			private set;
		}

		public void SetPlayerState()
		{
			emptyStandingTimer.Stop();
			Mode = StreetVehicleMode.Player;
			SetState(player: true);
			if (vehicleType == VehicleType.Bicycle)
			{
				_rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
			}
			else if (vehicleType == VehicleType.Skateboard)
			{
				_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
				skateboardAudio.enabled = true;
				skateboardStuckManager.enabled = true;
			}
			else
			{
				_rigidbody.constraints = (RigidbodyConstraints)80;
			}
			aIVehicleController.enabled = false;
			playerVehicleController.enabled = true;
		}

		public void SetAIState()
		{
			emptyStandingTimer.Stop();
			Mode = StreetVehicleMode.AI;
			SetState(player: false, ai: true);
			base.transform.rotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
			_rigidbody.constraints = (RigidbodyConstraints)94;
			aIVehicleController.enabled = true;
			playerVehicleController.enabled = false;
			if (vehicleType == VehicleType.Skateboard)
			{
				skateboardAudio.enabled = false;
				skateboardStuckManager.enabled = false;
			}
		}

		public void SetEmptyStanding()
		{
			emptyStandingTimer.Run(1f);
			Mode = StreetVehicleMode.EmptyStanding;
			SetState(player: false);
			base.transform.rotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
			_rigidbody.constraints = (RigidbodyConstraints)80;
			aIVehicleController.enabled = false;
			playerVehicleController.enabled = false;
			if (vehicleType == VehicleType.Bicycle)
			{
				bicycleAnimator.SetSpeed(0f, 0f);
			}
			else if (vehicleType == VehicleType.Skateboard)
			{
				skateboardAudio.enabled = false;
				skateboardStuckManager.enabled = false;
			}
		}

		public void SetFreeState()
		{
			emptyStandingTimer.Stop();
			Mode = StreetVehicleMode.Free;
			SetState(player: false, ai: false, free: true);
			_rigidbody.constraints = RigidbodyConstraints.None;
			aIVehicleController.enabled = false;
			playerVehicleController.enabled = false;
			if (vehicleType == VehicleType.Bicycle)
			{
				bicycleAnimator.SetSpeed(0f, 0f);
			}
			if (vehicleType == VehicleType.Skateboard)
			{
				skateboardAudio.enabled = false;
				skateboardStuckManager.enabled = false;
			}
		}

		private void Awake()
		{
			vehicleType = this.GetComponentSafe<VehicleComponents>().type;
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			center = _rigidbody.centerOfMass;
			tensor = _rigidbody.inertiaTensor;
			if (vehicleType == VehicleType.Bicycle)
			{
				aIVehicleController = this.GetComponentSafe<AIBicycleController>();
				playerVehicleController = this.GetComponentSafe<PlayerBicycleController>();
				bicycleAnimator = this.GetComponentSafe<BicycleAnimator>();
			}
			else if (vehicleType == VehicleType.Gyroboard)
			{
				aIVehicleController = this.GetComponentSafe<AIGyroboardController>();
				playerVehicleController = this.GetComponentSafe<PlayerGyroboardController>();
			}
			else if (vehicleType == VehicleType.Skateboard)
			{
				aIVehicleController = this.GetComponentSafe<AIGyroboardController>();
				playerVehicleController = this.GetComponentSafe<PlayerSkateboardController>();
				skateboardAudio = this.GetComponentSafe<SkateboardAudio>();
				skateboardStuckManager = this.GetComponentSafe<SkateboardStuckManager>();
			}
			defaultMass = _rigidbody.mass;
			defaultDrag = _rigidbody.drag;
			defaultAngularDrag = _rigidbody.angularDrag;
		}

		private void Start()
		{
			if (Mode == StreetVehicleMode.EmptyStanding)
			{
				emptyStandingTimer.Run(1f);
			}
		}

		private void Update()
		{
			if (emptyStandingTimer.Done())
			{
				emptyStandingTimer.Stop();
				SetFreeState();
			}
		}

		private void SetState(bool player, bool ai = false, bool free = false)
		{
			WheelCollider[] array = wheelHubs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(player);
			}
			defaultColliders.SetActive(!player && !ai);
			playerStateColliders.SetActive(player | ai);
			if (player)
			{
				_rigidbody.mass = playerStateMass;
				_rigidbody.drag = playerStateDrag;
				_rigidbody.angularDrag = playerStateAngularDrag;
				return;
			}
			_rigidbody.mass = defaultMass;
			if (free)
			{
				_rigidbody.drag = freeStateDrag;
				_rigidbody.angularDrag = freeStateAngularDrag;
			}
			else
			{
				_rigidbody.drag = defaultDrag;
				_rigidbody.angularDrag = defaultAngularDrag;
			}
		}
	}
}
