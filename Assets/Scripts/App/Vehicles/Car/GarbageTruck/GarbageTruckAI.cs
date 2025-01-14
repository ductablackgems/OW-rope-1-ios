using App.Vehicles.Car.Navigation;
using App.Vehicles.Car.Navigation.Modes.Manual;
using System.Collections.Generic;
using UnityEngine;

namespace App.Vehicles.Car.GarbageTruck
{
	public class GarbageTruckAI : MonoBehaviour
	{
		private VehicleModesHandler vehicleModesHandler;

		private AIVehicleModesHandler aiVehicleModesHandler;

		private ManualVehicleController manualVehicleController;

		private TargettingVehicleController targettingVehicleController;

		private CarController carCtrl;

		private SkidBrake skidBrake;

		private GarbageTruckManager garbageTruckManager;

		private DurationTimer actionDumpsterTimer = new DurationTimer();

		private DurationTimer checkDumpsterTimer = new DurationTimer();

		private DurationTimer liftingDumpsterTimer = new DurationTimer();

		private DurationTimer reversingTimer = new DurationTimer();

		private List<GameObject> containers;

		private GameObject[] landfills;

		private int iterator = -1;

		private AIPointController actualAIPoint;

		private AIPointController prevAIPoint;

		private ContainerDetectorController containerDetector;

		private bool collecting;

		private bool canMove = true;

		private float defaultTopSpeed;

		public Transform Target
		{
			get
			{
				return targettingVehicleController.TargetTransform;
			}
			set
			{
				targettingVehicleController.SetTarget(value);
				if (value == null)
				{
					if (aiVehicleModesHandler.mode == AIVehicleMode.Target)
					{
						aiVehicleModesHandler.SetMode(AIVehicleMode.Common);
					}
				}
				else if (aiVehicleModesHandler.mode != AIVehicleMode.Target)
				{
					aiVehicleModesHandler.SetMode(AIVehicleMode.Target);
				}
			}
		}

		private void Awake()
		{
			vehicleModesHandler = this.GetComponentInChildrenSafe<VehicleModesHandler>();
			aiVehicleModesHandler = this.GetComponentInChildrenSafe<AIVehicleModesHandler>();
			manualVehicleController = this.GetComponentInChildrenSafe<ManualVehicleController>();
			targettingVehicleController = this.GetComponentInChildrenSafe<TargettingVehicleController>();
			carCtrl = GetComponent<CarController>();
			skidBrake = this.GetComponentInChildrenSafe<SkidBrake>();
			defaultTopSpeed = carCtrl.MaxSpeed;
			garbageTruckManager = GetComponent<GarbageTruckManager>();
			containerDetector = base.gameObject.GetComponentInChildren<ContainerDetectorController>();
			GameObject[] gameObjects = ServiceLocator.GetGameObjects("Dumpster");
			containers = new List<GameObject>();
			if (gameObjects != null)
			{
				for (int i = 0; i < gameObjects.Length; i++)
				{
					if (gameObjects[i].gameObject.name == "AIpoint")
					{
						containers.Add(gameObjects[i]);
					}
				}
			}
			landfills = ServiceLocator.GetGameObjects("Dump");
			checkDumpsterTimer.Run(2f);
		}

		private void Start()
		{
			FindTarget();
		}

		private void FixedUpdate()
		{
			if (vehicleModesHandler.mode == VehicleMode.AI)
			{
				if (checkDumpsterTimer.Done())
				{
					checkDumpsterTimer.Run(2f);
					if (!Target)
					{
						FindTarget();
					}
					if (!Target)
					{
						checkDumpsterTimer.Stop();
						checkDumpsterTimer.Run(2f);
						return;
					}
					skidBrake.ForceBrake(1f);
					garbageTruckManager.LiftDown();
					checkDumpsterTimer.Stop();
				}
				if (actionDumpsterTimer.Done() && !collecting && (bool)Target)
				{
					GoBack();
				}
				if (!(Target != null))
				{
					return;
				}
				if (reversingTimer.Done())
				{
					reversingTimer.Stop();
				}
				if (liftingDumpsterTimer.Done())
				{
					GoBack();
					return;
				}
				float magnitude = (new Vector3(Target.transform.position.x, 0f, Target.transform.position.z) - new Vector3(base.transform.position.x, 0f, base.transform.position.z)).magnitude;
				if ((bool)actualAIPoint && Target == actualAIPoint.finish && canMove)
				{
					if (Vector3.Distance(actualAIPoint.virtualBack.position, base.transform.position) <= Vector3.Distance(actualAIPoint.finish.position, base.transform.position) && Target.gameObject.name != "AIpoint")
					{
						Target = actualAIPoint.transform;
					}
					if (Vector3.Distance(actualAIPoint.virtualLeft.position, base.transform.position) <= Vector3.Distance(actualAIPoint.finish.position, base.transform.position) && Target.gameObject.name != "AIpoint" && magnitude <= 7f)
					{
						Target = actualAIPoint.transform;
					}
					if (Vector3.Distance(actualAIPoint.virtualRight.position, base.transform.position) <= Vector3.Distance(actualAIPoint.finish.position, base.transform.position) && Target.gameObject.name != "AIpoint" && magnitude <= 7f)
					{
						Target = actualAIPoint.transform;
					}
					float num = Quaternion.Angle(garbageTruckManager.transform.rotation, Target.transform.rotation);
					float num2 = Vector3.Angle(Target.transform.position - garbageTruckManager.transform.position, base.transform.forward);
					if (num >= 0f && num <= 180f && num2 >= -90f && num2 <= 90f)
					{
						RotateToDumpster();
					}
					if (magnitude <= 4.45f && containerDetector.ActualContainer != null && !collecting)
					{
						StartCollect();
					}
				}
				if (Target.gameObject.name == "AIpoint" && magnitude <= 15f && (bool)Target.transform.parent.GetComponent<ContainerParameters>() && (bool)actualAIPoint && !collecting)
				{
					Target = actualAIPoint.finish;
					carCtrl.SetNewSpeed(10f);
					skidBrake.ForceBrake(1f);
				}
			}
			else if (vehicleModesHandler.mode == VehicleMode.Player)
			{
				ResetParameters();
			}
		}

		private void Reversing()
		{
			SetManualController();
			manualVehicleController.VerticalInput = -1f;
			manualVehicleController.HorizontalInput = 0f;
		}

		private void GoBackRight()
		{
			SetManualController();
			manualVehicleController.VerticalInput = -1f;
			manualVehicleController.HorizontalInput = 1f;
		}

		private void GoBackLeft()
		{
			SetManualController();
			manualVehicleController.VerticalInput = -1f;
			manualVehicleController.HorizontalInput = -1f;
		}

		private void GoFrontRight()
		{
			SetManualController();
			manualVehicleController.VerticalInput = 1f;
			manualVehicleController.HorizontalInput = 1f;
		}

		private void GoFrontLeft()
		{
			manualVehicleController.VerticalInput = 1f;
			manualVehicleController.HorizontalInput = -1f;
		}

		private void GoFront()
		{
			SetManualController();
			manualVehicleController.VerticalInput = 1f;
			manualVehicleController.HorizontalInput = 0f;
		}

		private void GoTarget()
		{
			if (!actualAIPoint || !Target || !(actualAIPoint.finish == Target) || collecting)
			{
				return;
			}
			float num = Vector3.Angle(Target.transform.position - garbageTruckManager.transform.position, base.transform.forward);
			Quaternion.Angle(Target.transform.rotation, base.transform.rotation);
			Vector3 from = Target.transform.position - garbageTruckManager.transform.position;
			Vector3 forward = garbageTruckManager.transform.forward;
			int num2 = (Vector3.SignedAngle(from, forward, Vector3.up) < 0f) ? 1 : (-1);
			float magnitude = (new Vector3(Target.transform.position.x, 0f, Target.transform.position.z) - new Vector3(base.transform.position.x, 0f, base.transform.position.z)).magnitude;
			if (reversingTimer.Running())
			{
				SetManualController();
				manualVehicleController.VerticalInput = -1f;
				float num3 = Mathf.Clamp(num, 0f - carCtrl.MaxSteer, carCtrl.MaxSteer);
				manualVehicleController.HorizontalInput = (0f - num3) / carCtrl.MaxSteer;
			}
			else if (magnitude <= 7f)
			{
				if (num < -7.5f)
				{
					SetManualController();
					manualVehicleController.VerticalInput = -1f;
					float num4 = Mathf.Clamp(num * -1f, 0f, carCtrl.MaxSteer);
					manualVehicleController.HorizontalInput = num4 / carCtrl.MaxSteer;
					reversingTimer.Run(5f);
				}
				else if (num > 7.5f)
				{
					SetManualController();
					manualVehicleController.VerticalInput = -1f;
					float num5 = Mathf.Clamp(num, 0f, carCtrl.MaxSteer);
					manualVehicleController.HorizontalInput = 0f - num5 / carCtrl.MaxSteer;
					reversingTimer.Run(5f);
				}
				else
				{
					SetManualController();
					manualVehicleController.VerticalInput = 1f;
					manualVehicleController.HorizontalInput = 0f;
				}
			}
			else if (num < -7.5f)
			{
				SetManualController();
				manualVehicleController.VerticalInput = 1f;
				float num6 = Mathf.Clamp(num, 0f, carCtrl.MaxSteer);
				manualVehicleController.HorizontalInput = num6 / carCtrl.MaxSteer * (float)num2;
			}
			else if (num > 7.5f)
			{
				SetManualController();
				manualVehicleController.VerticalInput = 1f;
				float num7 = Mathf.Clamp(num, 0f, carCtrl.MaxSteer);
				manualVehicleController.HorizontalInput = num7 / carCtrl.MaxSteer * (float)num2;
			}
			else
			{
				SetManualController();
				manualVehicleController.VerticalInput = 1f;
				manualVehicleController.HorizontalInput = 0f;
			}
		}

		private void ClearManualNavigate()
		{
			manualVehicleController.VerticalInput = 0f;
			manualVehicleController.HorizontalInput = 0f;
			if (aiVehicleModesHandler.mode == AIVehicleMode.Manual)
			{
				Vector3 target = targettingVehicleController.Target;
				if (aiVehicleModesHandler.mode != AIVehicleMode.Target)
				{
					aiVehicleModesHandler.SetMode(AIVehicleMode.Target);
				}
			}
		}

		private void StayAINavigate()
		{
			SetManualController();
			manualVehicleController.VerticalInput = 0f;
			manualVehicleController.HorizontalInput = 0f;
		}

		private void ResetParameters()
		{
			carCtrl.SetNewSpeed(defaultTopSpeed);
			collecting = false;
			liftingDumpsterTimer.Stop();
			actionDumpsterTimer.Stop();
			Target = null;
		}

		private void StartCollect()
		{
			float duration = 15.7f;
			skidBrake.ForceBrake(duration);
			garbageTruckManager.SetActualContainer(Target.transform.parent.GetComponent<ContainerParameters>());
			garbageTruckManager.Collect();
			liftingDumpsterTimer.Run(duration);
			StayAINavigate();
			collecting = true;
			canMove = false;
		}

		public void StopCollect()
		{
			if (collecting)
			{
				ClearManualNavigate();
				ResetParameters();
				garbageTruckManager.LiftUp();
				CheckOtherDumpster();
			}
		}

		private void GoBack()
		{
			ResetParameters();
			if ((bool)actualAIPoint)
			{
				actualAIPoint.IsFull = false;
			}
			Target = null;
			canMove = true;
			collecting = true;
			Reversing();
		}

		public void CheckDistanceAndRotation()
		{
			if ((bool)Target)
			{
				float num = Vector3.Angle(Target.transform.position - garbageTruckManager.transform.position, base.transform.forward);
				if (num < -5f || num > 5f)
				{
					Reversing();
				}
			}
		}

		private void RotateToDumpster()
		{
			GoTarget();
			Transform parent = garbageTruckManager.transform.parent;
			garbageTruckManager.transform.parent = Target.transform;
			float num = garbageTruckManager.transform.localPosition.x;
			if (garbageTruckManager.transform.localPosition.x > 0f)
			{
				num -= 0.5f * Time.deltaTime;
				if (num < 0f)
				{
					num = 0f;
				}
			}
			else if (garbageTruckManager.transform.localPosition.x < 0f)
			{
				num += 0.5f * Time.deltaTime;
				if (num > 0f)
				{
					num = 0f;
				}
			}
			garbageTruckManager.transform.localPosition = new Vector3(num, garbageTruckManager.transform.localPosition.y, garbageTruckManager.transform.localPosition.z);
			garbageTruckManager.transform.rotation = Quaternion.RotateTowards(garbageTruckManager.transform.rotation, Quaternion.LookRotation(Target.transform.position - garbageTruckManager.transform.position), 7f * Time.deltaTime);
			garbageTruckManager.transform.parent = parent;
			Vector3.Angle(Target.transform.position - garbageTruckManager.transform.position, base.transform.forward);
			float magnitude = (new Vector3(Target.transform.position.x, 0f, Target.transform.position.z) - new Vector3(base.transform.position.x, 0f, base.transform.position.z)).magnitude;
		}

		private void CheckOtherDumpster()
		{
			prevAIPoint = prevAIPoint;
			actualAIPoint = null;
			checkDumpsterTimer.Run(2f);
		}

		private void FindTarget()
		{
			for (int i = 0; i < containers.Count; i++)
			{
				if (containers[i].activeSelf && Vector3.Distance(containers[i].transform.position, base.transform.position) <= 20f && (bool)containers[i].GetComponentInParent<ContainerParameters>() && (bool)containers[i].GetComponent<AIPointController>() && prevAIPoint != containers[i].GetComponent<AIPointController>() && !containers[i].GetComponent<AIPointController>().IsFull && containers[i].GetComponentInParent<ContainerParameters>().IsFull && !containers[i].GetComponentInParent<ContainerParameters>().PhysicActivated)
				{
					actualAIPoint = containers[i].GetComponent<AIPointController>();
					actualAIPoint.IsFull = true;
					Target = containers[i].transform;
					actionDumpsterTimer.Run(90f);
				}
			}
		}

		private void SetManualController()
		{
			if (aiVehicleModesHandler.mode != AIVehicleMode.Manual)
			{
				aiVehicleModesHandler.SetMode(AIVehicleMode.Manual);
			}
		}
	}
}
