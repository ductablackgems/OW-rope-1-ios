using App.GUI;
using App.Missions;
using App.Player;
using App.Util;
using App.Vehicles.Car.Navigation;
using System.Collections.Generic;
using UnityEngine;

namespace App.Vehicles.Car.GarbageTruck
{
	public class GarbageTruckDispatching : MonoBehaviour
	{
		private VehicleModesHandler vehicleModesHandler;

		private MissionManager missionManager;

		private RagdollInteractor ragdollInteraction;

		private Health playerHealth;

		private Health truckHealth;

		private DurationTimer NewMissionTimer = new DurationTimer();

		private DurationTimer FiledMissionTimer = new DurationTimer();

		private GarbageTruckManager targetGarbageTruckManager;

		private bool isTargeted;

		private int missionKey = -1;

		private List<GameObject> dumpsters;

		private GameObject[] dumps;

		private int actualDumpster = -1;

		private bool goToDump;

		private GameObject nearestContainer;

		private GameObject nearestDump;

		private bool canNewMission;

		private bool firstSit;

		private void Awake()
		{
			vehicleModesHandler = this.GetComponentSafe<VehicleModesHandler>();
			missionManager = ServiceLocator.Get<MissionManager>();
			targetGarbageTruckManager = base.gameObject.GetComponentSafe<GarbageTruckManager>();
			playerHealth = ServiceLocator.GetGameObject("Player").GetComponentSafe<Health>();
			ragdollInteraction = playerHealth.GetComponent<RagdollInteractor>();
			truckHealth = GetComponent<Health>();
			GameObject[] gameObjects = ServiceLocator.GetGameObjects("Dumpster");
			dumpsters = new List<GameObject>();
			for (int i = 0; i < gameObjects.Length; i++)
			{
				if (gameObjects[i].name == "Dumpster")
				{
					dumpsters.Add(gameObjects[i]);
				}
			}
			dumps = ServiceLocator.GetGameObjects("Dump");
			if ((bool)targetGarbageTruckManager)
			{
				targetGarbageTruckManager.MaxContainerCapacity = 4;
			}
		}

		private void Update()
		{
			if (!firstSit && vehicleModesHandler.mode == VehicleMode.Player)
			{
				firstSit = true;
				NewMissionTimer.Run(5f);
			}
			if (isTargeted && (bool)ragdollInteraction && ragdollInteraction.standUpBlocked)
			{
				AbortMission();
			}
			if (isTargeted && (targetGarbageTruckManager == null || playerHealth.Dead()))
			{
				AbortMission();
			}
			if (dumpsters == null || dumpsters.Count == 0)
			{
				return;
			}
			if (isTargeted && (playerHealth.transform.position - base.transform.position).magnitude > 30f)
			{
				AbortMission();
			}
			if (isTargeted && !missionManager.CompareMission(missionKey))
			{
				AbortMission();
			}
			if (vehicleModesHandler.mode != VehicleMode.Player)
			{
				return;
			}
			if (NewMissionTimer.Done())
			{
				NewMissionTimer.Stop();
				targetGarbageTruckManager = base.gameObject.GetComponentSafe<GarbageTruckManager>();
				targetGarbageTruckManager.actualMissionPiece = MissionPiece.Empty;
				actualDumpster = -1;
				isTargeted = false;
				goToDump = false;
				targetGarbageTruckManager.ResetCapacity();
				canNewMission = true;
			}
			if (FiledMissionTimer.Done())
			{
				NewMissionTimer.Run(10f);
				FiledMissionTimer.Stop();
			}
			if ((bool)targetGarbageTruckManager && vehicleModesHandler.mode == VehicleMode.Player && !isTargeted && targetGarbageTruckManager.actualMissionPiece == MissionPiece.Empty && canNewMission)
			{
				targetGarbageTruckManager.ResetCapacity();
				NewMissionTimer.Stop();
				actualDumpster = -1;
				goToDump = false;
				isTargeted = true;
				canNewMission = false;
				StartMission();
				missionKey = missionManager.StartMission(string.Format(LocalizationManager.Instance.GetText(5014), 4), delegate
				{
					bool flag = targetGarbageTruckManager != null;
				});
				targetGarbageTruckManager.actualMissionPiece = MissionPiece.Collecting;
			}
			if ((bool)targetGarbageTruckManager && (bool)nearestContainer && nearestContainer.GetComponent<ContainerParameters>().PhysicActivated && targetGarbageTruckManager.actualMissionPiece == MissionPiece.Collecting)
			{
				FindOtherContainer();
			}
			if ((bool)targetGarbageTruckManager && actualDumpster != targetGarbageTruckManager.AllCapacity && targetGarbageTruckManager.actualMissionPiece == MissionPiece.Collecting)
			{
				FindOtherContainer();
			}
			if ((bool)targetGarbageTruckManager && targetGarbageTruckManager.IsFull && targetGarbageTruckManager.actualMissionPiece == MissionPiece.Landfill && !goToDump)
			{
				goToDump = true;
				if ((bool)nearestContainer)
				{
					nearestContainer.GetComponent<ContainerParameters>().DeactivateMissionDumpster();
				}
				nearestDump = null;
				for (int i = 0; i < dumps.Length; i++)
				{
					if (dumps[i].activeSelf)
					{
						if (nearestDump == null)
						{
							nearestDump = dumps[i];
						}
						else if (Vector3.Distance(base.transform.position, nearestDump.transform.position) > Vector3.Distance(base.transform.position, dumps[i].transform.position))
						{
							nearestDump = dumps[i];
						}
					}
				}
				if ((bool)nearestDump)
				{
					missionManager.SetMapTarget(missionKey, nearestDump.transform.position, TargetCursorType.Landfill);
					float magnitude = (base.transform.position - nearestDump.transform.position).magnitude;
					float remainTime = 120f;
					FiledMissionTimer.Stop();
					FiledMissionTimer.Run(120f);
					missionManager.SetRemainTime(missionKey, remainTime);
					if ((bool)nearestDump.GetComponent<LandfillController>())
					{
						nearestDump.GetComponent<LandfillController>().ActivateEffect();
					}
				}
			}
			if (targetGarbageTruckManager != null && targetGarbageTruckManager.IsFull && targetGarbageTruckManager.actualMissionPiece == MissionPiece.Collecting)
			{
				missionManager.SetJobText(missionKey, LocalizationManager.Instance.GetText(5015));
				targetGarbageTruckManager.actualMissionPiece = MissionPiece.Landfill;
			}
			if (targetGarbageTruckManager != null && !targetGarbageTruckManager.IsFull && targetGarbageTruckManager.actualMissionPiece == MissionPiece.Landfill)
			{
				targetGarbageTruckManager.actualMissionPiece = MissionPiece.Collecting;
				goToDump = false;
			}
			if (isTargeted && targetGarbageTruckManager != null && targetGarbageTruckManager.actualMissionPiece == MissionPiece.Done)
			{
				if ((bool)nearestContainer)
				{
					nearestContainer.GetComponent<ContainerParameters>().DeactivateMissionDumpster();
				}
				if ((bool)nearestDump && (bool)nearestDump.GetComponent<LandfillController>())
				{
					nearestDump.GetComponent<LandfillController>().DeactivateEffect();
				}
				targetGarbageTruckManager.InLandfill = false;
				missionManager.FinishMission(missionKey, success: true);
				NewMissionTimer.Run(10f);
				isTargeted = false;
			}
		}

		private void AbortMission()
		{
			missionManager.FinishMission(missionKey, success: false);
			targetGarbageTruckManager.actualMissionPiece = MissionPiece.Empty;
			if ((bool)nearestContainer)
			{
				nearestContainer.GetComponent<ContainerParameters>().DeactivateMissionDumpster();
			}
			if ((bool)nearestDump && (bool)nearestDump.GetComponent<LandfillController>())
			{
				nearestDump.GetComponent<LandfillController>().DeactivateEffect();
			}
			NewMissionTimer.Run(10f);
			isTargeted = false;
			EndMission();
		}

		public void MissionFailed()
		{
			AbortMission();
			GarbageCapacity.Instance.Deactivate();
		}

		public void MissionFailedInTruck(float nextMissionDelay)
		{
			AbortMission();
			NewMissionTimer.Stop();
			NewMissionTimer.Run(nextMissionDelay);
		}

		private void FindOtherContainer()
		{
			int num = actualDumpster;
			actualDumpster = targetGarbageTruckManager.AllCapacity;
			if ((bool)nearestContainer)
			{
				nearestContainer.GetComponent<ContainerParameters>().DeactivateMissionDumpster();
			}
			nearestContainer = null;
			for (int i = 0; i < dumpsters.Count; i++)
			{
				ContainerParameters component = dumpsters[i].GetComponent<ContainerParameters>();
				if ((bool)component && component.IsFull && !component.CheckAI() && !component.PhysicActivated)
				{
					if (nearestContainer == null)
					{
						nearestContainer = dumpsters[i];
					}
					else if (Vector3.Distance(base.transform.position, nearestContainer.transform.position) > Vector3.Distance(base.transform.position, dumpsters[i].transform.position))
					{
						nearestContainer = dumpsters[i];
					}
				}
			}
			if ((bool)nearestContainer)
			{
				nearestContainer.GetComponent<ContainerParameters>().SetAsMissionDumpster();
				missionManager.SetMapTarget(missionKey, nearestContainer.transform.position, TargetCursorType.Container);
				float magnitude = (base.transform.position - nearestContainer.transform.position).magnitude;
				missionManager.SetRewards(missionKey, 500);
				float remainTime = 120f;
				FiledMissionTimer.Stop();
				FiledMissionTimer.Run(120f);
				missionManager.SetRemainTime(missionKey, remainTime);
			}
			if (actualDumpster != targetGarbageTruckManager.AllCapacity)
			{
				actualDumpster = targetGarbageTruckManager.AllCapacity;
			}
			missionManager.SetJobText(missionKey, string.Format(LocalizationManager.Instance.GetText(5014), 4 - targetGarbageTruckManager.AllCapacity));
			GarbageCapacity.Instance.SetValue(targetGarbageTruckManager.AllCapacity, 2f, num < targetGarbageTruckManager.AllCapacity);
		}

		private void StartMission()
		{
			for (int i = 0; i < dumpsters.Count; i++)
			{
				if ((bool)dumpsters[i].GetComponent<ContainerParameters>())
				{
					dumpsters[i].GetComponent<ContainerParameters>().StartMission();
				}
				else if ((bool)dumpsters[i].GetComponent<DumpsterController>() && (bool)dumpsters[i].GetComponent<DumpsterController>().FreshDumpster && (bool)dumpsters[i].GetComponent<DumpsterController>().FreshDumpster.GetComponent<ContainerParameters>())
				{
					dumpsters[i].GetComponent<DumpsterController>().FreshDumpster.GetComponent<ContainerParameters>().StartMission();
				}
			}
		}

		private void EndMission()
		{
			for (int i = 0; i < dumpsters.Count; i++)
			{
				if ((bool)dumpsters[i].GetComponent<ContainerParameters>())
				{
					dumpsters[i].GetComponent<ContainerParameters>().EndMission();
				}
				else if ((bool)dumpsters[i].GetComponent<DumpsterController>() && (bool)dumpsters[i].GetComponent<DumpsterController>().FreshDumpster && (bool)dumpsters[i].GetComponent<DumpsterController>().FreshDumpster.GetComponent<ContainerParameters>())
				{
					dumpsters[i].GetComponent<DumpsterController>().FreshDumpster.GetComponent<ContainerParameters>().EndMission();
				}
			}
		}
	}
}
