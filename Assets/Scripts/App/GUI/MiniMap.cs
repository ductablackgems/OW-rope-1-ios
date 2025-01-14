using App.ATM;
using App.Quests;
using App.Shop;
using App.Spawn;
using System;
using UnityEngine;

namespace App.GUI
{
	public class MiniMap : MonoBehaviour
	{
		public const float TankVisibilityDistance = 600f;

		public const float MechVisibilityDistance = 600f;

		public UnityEngine.Camera miniMapCamera;

		public GameObject playerIconPrefab;

		public Transform mapPlaneTransform;

		public Transform miniMapViewTransform;

		public RectTransform targetCursor;

		public RectTransform objectiveCursor;

		public GameObject taxiTargetIcon;

		public GameObject fireTargetIcon;

		public GameObject busStopTargetIcon;

		public GameObject containerTargetIcon;

		public GameObject landfillTargetIcon;

		public GameObject ambulanceTargetIcon;

		public GameObject policeTargetIcon;

		public GameObject objectiveIcon;

		public GameObject hangarIcon;

		public GameObject[] tankIcons = new GameObject[2];

		public GameObject[] mechIcons = new GameObject[2];

		public GameObject[] questProviderIcons = new GameObject[0];

		public GameObject[] atmIcons = new GameObject[0];

		private Transform playerTransform;

		private Transform cameraTransform;

		private Transform playerIconTransfom;

		private TankSpawner tankSpawner;

		private MechSpawner mechSpawner;

		private QuestManager questManager;

		private ATMManager atmManager;

		private INavigationItem objectiveNavigation;

		private Vector3 target;

		private TankSpawnPoint[] activatedTankPoints = new TankSpawnPoint[2];

		private MechSpawnPoint[] activatedMechPoints = new MechSpawnPoint[2];

		public Transform PlayerIconTransform => playerIconTransfom;

		public void ActivateTargetCursor(Vector3 target, TargetCursorType targetCursorType)
		{
			bool flag = targetCursorType == TargetCursorType.Objective;
			DeactivateTargetCursor(flag);
			GameObject gameObject;
			switch (targetCursorType)
			{
			case TargetCursorType.Ambulance:
				gameObject = ambulanceTargetIcon;
				break;
			case TargetCursorType.Fire:
				gameObject = fireTargetIcon;
				break;
			case TargetCursorType.Container:
				gameObject = containerTargetIcon;
				break;
			case TargetCursorType.Landfill:
				gameObject = landfillTargetIcon;
				break;
			case TargetCursorType.BusStop:
				gameObject = busStopTargetIcon;
				break;
			case TargetCursorType.Police:
				gameObject = policeTargetIcon;
				break;
			case TargetCursorType.Objective:
				gameObject = objectiveIcon;
				break;
			default:
				gameObject = taxiTargetIcon;
				break;
			}
			RectTransform rectTransform;
			if (flag)
			{
				rectTransform = objectiveCursor;
			}
			else
			{
				this.target = target;
				rectTransform = targetCursor;
			}
			rectTransform.gameObject.SetActive(value: true);
			SetIconPosition(gameObject, target);
			gameObject.SetActive(value: true);
		}

		public void DeactivateTargetCursor(bool isObjective = false)
		{
			if (isObjective)
			{
				if (!(objectiveCursor == null))
				{
					objectiveCursor.gameObject.SetActive(value: false);
					objectiveIcon.SetActive(value: false);
				}
			}
			else if (targetCursor != null)
			{
				targetCursor.gameObject.SetActive(value: false);
				taxiTargetIcon.SetActive(value: false);
				fireTargetIcon.SetActive(value: false);
				containerTargetIcon.SetActive(value: false);
				landfillTargetIcon.SetActive(value: false);
				ambulanceTargetIcon.SetActive(value: false);
				busStopTargetIcon.SetActive(value: false);
				policeTargetIcon.SetActive(value: false);
				hangarIcon.SetActive(value: false);
			}
		}

		private void Awake()
		{
			playerTransform = ServiceLocator.GetGameObject("Player").transform;
			cameraTransform = UnityEngine.Camera.main.transform;
			playerIconTransfom = UnityEngine.Object.Instantiate(playerIconPrefab).transform;
			tankSpawner = ServiceLocator.Get<TankSpawner>();
			mechSpawner = ServiceLocator.Get<MechSpawner>();
			questManager = ServiceLocator.Get<QuestManager>();
			atmManager = ServiceLocator.Get<ATMManager>();
			tankSpawner.OnActivatePoint += OnActivateTankPoint;
			tankSpawner.OnDeactivatePoint += OnDeactivateTankPoint;
			mechSpawner.OnActivatePoint += OnActivateMechPoint;
			mechSpawner.OnDeactivatePoint += OnDeactivateMechPoint;
			if (questManager != null)
			{
				questManager.QuestChanged += OnQuestChanged;
			}
			DeactivateTargetCursor();
			DeactivateTargetCursor(isObjective: true);
			DeactivateIcons(tankIcons);
			DeactivateIcons(mechIcons);
			DeactivateIcons(atmIcons);
		}

		private void Start()
		{
			SetQuestNPCs();
			ShowATMs();
			ShowHangar();
		}

		private void OnDestroy()
		{
			tankSpawner.OnActivatePoint -= OnActivateTankPoint;
			tankSpawner.OnDeactivatePoint -= OnDeactivateTankPoint;
			mechSpawner.OnActivatePoint -= OnActivateMechPoint;
			mechSpawner.OnDeactivatePoint -= OnDeactivateMechPoint;
			if (questManager != null)
			{
				questManager.QuestChanged -= OnQuestChanged;
			}
		}

		private void Update()
		{
			playerIconTransfom.transform.rotation = Quaternion.Euler(90f, playerTransform.eulerAngles.y, 0f);
			playerIconTransfom.transform.position = new Vector3(playerTransform.position.x, mapPlaneTransform.position.y + 5f, playerTransform.position.z);
			miniMapViewTransform.transform.rotation = Quaternion.Euler(90f, cameraTransform.eulerAngles.y + 180f, 0f);
			miniMapViewTransform.transform.position = new Vector3(playerTransform.position.x, mapPlaneTransform.position.y + 10f, playerTransform.position.z);
			if (objectiveNavigation != null && objectiveIcon != null && objectiveIcon.activeSelf)
			{
				SetIconPosition(objectiveIcon, objectiveNavigation.Position);
				UpdateCursor(objectiveCursor, objectiveNavigation.Position);
			}
			if (taxiTargetIcon.activeSelf || fireTargetIcon.activeSelf || ambulanceTargetIcon.activeSelf || containerTargetIcon.activeSelf || landfillTargetIcon.activeSelf || busStopTargetIcon.activeSelf || policeTargetIcon.activeSelf)
			{
				UpdateCursor(targetCursor, target);
			}
			UpdateTankPointsVisibility(activatedTankPoints, tankIcons, 600f);
			TankSpawnPoint[] activatedPoints = activatedMechPoints;
			UpdateTankPointsVisibility(activatedPoints, mechIcons, 600f);
		}

		private void OnActivateTankPoint(TankSpawnPoint point)
		{
			ActivateTankPoint(point, activatedTankPoints, tankIcons);
		}

		private void OnDeactivateTankPoint(TankSpawnPoint point)
		{
			DeactivateTankPoint(point, activatedTankPoints, tankIcons);
		}

		private void OnActivateMechPoint(TankSpawnPoint point)
		{
			TankSpawnPoint[] activatedSpawPoints = activatedMechPoints;
			ActivateTankPoint(point, activatedSpawPoints, mechIcons);
		}

		private void OnDeactivateMechPoint(TankSpawnPoint point)
		{
			TankSpawnPoint[] activatedSpawPoints = activatedMechPoints;
			DeactivateTankPoint(point, activatedSpawPoints, mechIcons);
		}

		private void OnQuestChanged(Quest quest)
		{
			GameplayObjective currentObjective = quest.CurrentObjective;
			if (!(currentObjective == null))
			{
				objectiveNavigation = currentObjective;
				if (objectiveNavigation.ShowOnMap && currentObjective.CurrentState == GameplayObjective.State.Active)
				{
					ActivateTargetCursor(objectiveNavigation.Position, TargetCursorType.Objective);
				}
				else
				{
					DeactivateTargetCursor(isObjective: true);
				}
			}
		}

		private void ActivateTankPoint(TankSpawnPoint point, TankSpawnPoint[] activatedSpawPoints, GameObject[] icons)
		{
			if (Array.IndexOf(activatedSpawPoints, point) >= 0)
			{
				return;
			}
			int num = 0;
			while (true)
			{
				if (num < activatedSpawPoints.Length)
				{
					if (!(activatedSpawPoints[num] != null))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			GameObject gameObject = icons[num];
			Vector3 position = point.transform.position;
			gameObject.transform.position = new Vector3(position.x, gameObject.transform.position.y, position.z);
			activatedSpawPoints[num] = point;
		}

		private void DeactivateTankPoint(TankSpawnPoint spawnPoint, TankSpawnPoint[] activatedSpawPoints, GameObject[] icons)
		{
			int num = Array.IndexOf(activatedSpawPoints, spawnPoint);
			if (num >= 0 && !(icons[num] == null))
			{
				icons[num].SetActive(value: false);
				activatedSpawPoints[num] = null;
			}
		}

		private static void DeactivateIcons(GameObject[] icons)
		{
			foreach (GameObject gameObject in icons)
			{
				if (!(gameObject == null))
				{
					gameObject.SetActive(value: false);
				}
			}
		}

		private void UpdateTankPointsVisibility(TankSpawnPoint[] activatedPoints, GameObject[] icons, float visibleDistance)
		{
			for (int i = 0; i < activatedPoints.Length; i++)
			{
				TankSpawnPoint tankSpawnPoint = activatedPoints[i];
				if (!(tankSpawnPoint == null))
				{
					float magnitude = (tankSpawnPoint.transform.position - playerTransform.position).magnitude;
					icons[i].SetActive(magnitude < visibleDistance);
				}
			}
		}

		private void SetQuestNPCs()
		{
			if (!(questManager == null))
			{
				QuestProvider[] questProviders = questManager.QuestProviders;
				for (int i = 0; i < questProviders.Length && i < questProviderIcons.Length; i++)
				{
					INavigationItem navigationItem = questProviders[i];
					SetIconPosition(questProviderIcons[i], navigationItem.Position);
				}
			}
		}

		private void UpdateCursor(RectTransform cursor, Vector3 target)
		{
			Vector3 forward = cameraTransform.forward;
			forward.y = 0f;
			Vector3 vector = target - playerTransform.position;
			vector.y = 0f;
			cursor.gameObject.SetActive(vector.magnitude * 1.05f > Mathf.Abs(miniMapCamera.orthographicSize));
			float num = Vector3.Angle(forward, vector);
			if (cameraTransform.InverseTransformDirection(vector).x >= 0f)
			{
				num *= -1f;
			}
			cursor.localRotation = Quaternion.Euler(0f, 0f, num);
		}

		private void ShowATMs()
		{
			ATMInteractiveObject[] terminals = atmManager.Terminals;
			for (int i = 0; i < terminals.Length; i++)
			{
				GameObject obj = atmIcons[i];
				SetIconPosition(atmIcons[i], terminals[i].transform.position);
				obj.SetActive(value: true);
			}
		}

		private void ShowHangar()
		{
			AirplaneShoppingZone airplaneShoppingZone = ServiceLocator.Get<AirplaneShoppingZone>();
			SetIconPosition(hangarIcon, airplaneShoppingZone.transform.position);
			hangarIcon.SetActive(value: true);
		}

		private static void SetIconPosition(GameObject icon, Vector3 position)
		{
			Transform transform = icon.transform;
			position.y = transform.position.y;
			transform.position = position;
		}
	}
}
