using App.Vehicles.Car.Navigation;
using System.Collections.Generic;
using UnityEngine;

namespace App.Vehicles.Car.GarbageTruck
{
	public class ContainerParameters : MonoBehaviour
	{
		public BoxCollider MainCollider;

		public GameObject Colliders;

		public GameObject garbageTruckDetector;

		public GameObject contactCollider;

		public List<ParticleSystem> effects;

		public ParticleSystem stinkEffect;

		public GameObject leftDoor;

		public GameObject rightDoor;

		public GameObject leftPulsingLine;

		public GameObject rightPulsingLine;

		private Transform myDefaultParent;

		private Vector3 myDefaultPosition;

		private Quaternion myDefaultRotation;

		private BoxCollider myCollider;

		private AudioSource audioSource;

		private Animator animator;

		private AIPointController aiPointController;

		private DumpsterController dumpsterController;

		private bool inLift;

		private DurationTimer checkerTimer = new DurationTimer();

		private DurationTimer animationTimer = new DurationTimer();

		private DurationTimer maxBlinkingTimer = new DurationTimer();

		private DurationTimer toDrainTimer = new DurationTimer();

		private DurationTimer animatorEnableTimer = new DurationTimer();

		private DurationTimer blinkingTimer = new DurationTimer();

		private DurationTimer colliderActivatorTimer = new DurationTimer();

		private bool missionStarted;

		private Quaternion defaultLeftDoorRotation;

		private Vector3 defaultLeftDoorPosition;

		private Quaternion defaultRightDoorRotation;

		private Vector3 defaultRightDoorPosition;

		private Color defaultEmmisionColor;

		private bool isPlayer;

		public bool IsFull
		{
			get;
			private set;
		}

		public bool Collected
		{
			get;
			private set;
		}

		public bool PhysicActivated
		{
			get;
			private set;
		}

		public bool InMission
		{
			get;
			private set;
		}

		private void Awake()
		{
			Collected = false;
			IsFull = true;
			myDefaultParent = base.transform.parent;
			myDefaultRotation = base.transform.rotation;
			myDefaultPosition = base.transform.position;
			myCollider = GetComponent<BoxCollider>();
			animator = GetComponentInChildren<Animator>();
			audioSource = GetComponent<AudioSource>();
			dumpsterController = GetComponentInParent<DumpsterController>();
			aiPointController = GetComponentInChildren<AIPointController>();
			EndMission();
			animator.enabled = false;
			if ((bool)leftDoor)
			{
				defaultLeftDoorPosition = leftDoor.transform.localPosition;
				defaultLeftDoorRotation = leftDoor.transform.localRotation;
			}
			if ((bool)rightDoor)
			{
				defaultRightDoorPosition = rightDoor.transform.localPosition;
				defaultRightDoorRotation = rightDoor.transform.localRotation;
			}
			Material material = GetComponent<Renderer>().material;
			defaultEmmisionColor = material.GetColor("_EmissionColor");
			stinkEffect.Stop();
			stinkEffect.gameObject.SetActive(value: false);
		}

		private void Update()
		{
			if (colliderActivatorTimer.Done())
			{
				colliderActivatorTimer.Stop();
				ActivateColliders();
			}
			if (blinkingTimer.Done())
			{
				blinkingTimer.Stop();
				animator.enabled = true;
				animator.Play("Blinking");
			}
			if (animatorEnableTimer.Done())
			{
				animatorEnableTimer.Stop();
				animator.enabled = false;
			}
			if (maxBlinkingTimer.Done())
			{
				maxBlinkingTimer.Stop();
				if (!InMission)
				{
					animator.Play("Default");
					animator.enabled = false;
				}
				else
				{
					maxBlinkingTimer.Run(120f);
				}
			}
			if (toDrainTimer.Done())
			{
				audioSource.Play();
				if ((IsFull && !isPlayer) || (InMission && isPlayer))
				{
					for (int i = 0; i < effects.Count; i++)
					{
						effects[i].Play();
					}
				}
				toDrainTimer.Stop();
			}
			if (checkerTimer.Done())
			{
				IsFull = true;
				Collected = false;
				aiPointController.IsFull = false;
				contactCollider.SetActive(value: true);
				checkerTimer.Stop();
			}
			if (animationTimer.Done())
			{
				animator.Play("Default");
				stinkEffect.Stop();
				stinkEffect.gameObject.SetActive(value: false);
				animator.enabled = false;
				leftPulsingLine.SetActive(value: false);
				rightPulsingLine.SetActive(value: false);
				animationTimer.Stop();
			}
		}

		public bool Collect()
		{
			bool inMission = InMission;
			if (IsFull)
			{
				Collected = true;
				IsFull = false;
				checkerTimer.Run(300f);
				if (InMission)
				{
					DeactivateMissionDumpster();
				}
			}
			return inMission;
		}

		public void SetAsCollected()
		{
			Collected = true;
		}

		private void SetBlinking()
		{
			if (!inLift)
			{
				animator.enabled = true;
				animator.Play("Blinking");
				stinkEffect.gameObject.SetActive(value: true);
				stinkEffect.Play();
				leftPulsingLine.SetActive(value: true);
				rightPulsingLine.SetActive(value: true);
				maxBlinkingTimer.Run(120f);
				blinkingTimer.Run(0.2f);
			}
		}

		public void StartLift(Transform parent)
		{
			if (inLift)
			{
				return;
			}
			VehicleModesHandler componentInParents = parent.GetComponentInParents<VehicleModesHandler>();
			if ((bool)componentInParents)
			{
				if (componentInParents.mode == VehicleMode.Player)
				{
					isPlayer = true;
				}
				else
				{
					isPlayer = false;
				}
			}
			else
			{
				isPlayer = false;
			}
			inLift = true;
			DeactivateCollider();
			base.transform.parent = parent;
			garbageTruckDetector.SetActive(value: false);
			animator.enabled = false;
			animator.Play("Default");
			leftPulsingLine.SetActive(value: false);
			rightPulsingLine.SetActive(value: false);
			PlayAnimation();
			toDrainTimer.Run(5f);
			stinkEffect.Stop();
		}

		public void EndLif(bool toDefaultPosition = false)
		{
			if (inLift)
			{
				inLift = false;
				ResetParent();
				animator.enabled = false;
				animator.Play("Default");
				leftPulsingLine.SetActive(value: false);
				rightPulsingLine.SetActive(value: false);
				stinkEffect.Stop();
				stinkEffect.gameObject.SetActive(value: false);
				garbageTruckDetector.SetActive(value: true);
				ActivateCollider();
				if (toDefaultPosition)
				{
					Reset();
					base.transform.position = myDefaultPosition;
					base.transform.rotation = myDefaultRotation;
				}
			}
		}

		public void SetAsMissionDumpster()
		{
			aiPointController.IsFull = true;
			SetBlinking();
			InMission = true;
		}

		public bool CheckAI()
		{
			return aiPointController.IsFull;
		}

		public void DeactivateMissionDumpster()
		{
			InMission = false;
			aiPointController.IsFull = false;
			animator.enabled = false;
			animator.Play("Default");
			stinkEffect.Stop();
			stinkEffect.gameObject.SetActive(value: false);
			leftPulsingLine.SetActive(value: false);
			rightPulsingLine.SetActive(value: false);
			animationTimer.Stop();
			Reset();
			EndMission();
		}

		private void Reset()
		{
			if ((bool)leftDoor)
			{
				leftDoor.transform.localPosition = defaultLeftDoorPosition;
				leftDoor.transform.localRotation = defaultLeftDoorRotation;
			}
			if ((bool)rightDoor)
			{
				rightDoor.transform.localPosition = defaultRightDoorPosition;
				rightDoor.transform.localRotation = defaultRightDoorRotation;
			}
			stinkEffect.Stop();
			stinkEffect.gameObject.SetActive(value: false);
			animator.enabled = true;
			animator.Play("Up");
			animator.Play("Default");
			animatorEnableTimer.Run(0.1f);
		}

		private void DeactivateCollider()
		{
			myCollider.enabled = false;
		}

		public void ActivateCollider()
		{
			if (!inLift)
			{
				myCollider.enabled = true;
			}
		}

		public void ResetParent()
		{
			base.transform.parent = myDefaultParent;
		}

		private void PlayAnimation()
		{
			animator.enabled = true;
			animator.Play("Up");
			animationTimer.Run(11.433f);
		}

		public void OnDestroyDumper()
		{
			ResetParent();
			EndLif();
			EndMission();
			dumpsterController.OnDestroyDumpster();
		}

		public void StartMission()
		{
			missionStarted = true;
		}

		public void EndMission()
		{
			missionStarted = false;
		}

		public void ClearAIPoint()
		{
			aiPointController.IsFull = false;
		}

		public void ActivatePhysic()
		{
			if (!PhysicActivated)
			{
				toDrainTimer.Stop();
				PhysicActivated = true;
				base.gameObject.AddComponent<Rigidbody>();
				GetComponent<Rigidbody>().mass = 100f;
				GetComponent<Rigidbody>().useGravity = true;
				GetComponent<Rigidbody>().isKinematic = false;
				new Vector3(base.transform.position.x - 0f, base.transform.position.y - 5f, base.transform.position.z - 5f);
				GetComponent<Rigidbody>().AddForce((base.transform.TransformDirection(base.transform.forward) + new Vector3(0f, 0.5f, 0f)) * 25000f);
				ResetParent();
				DeactivateColliders();
				audioSource.Stop();
				Reset();
			}
		}

		public void DeactivatePhysic()
		{
			if (PhysicActivated)
			{
				PhysicActivated = false;
				UnityEngine.Object.Destroy(GetComponent<Rigidbody>());
				ActivateColliders();
			}
		}

		public void SetParent(Transform parent)
		{
			base.transform.parent = parent;
		}

		public void DeactivateEmmiters()
		{
			leftPulsingLine.SetActive(value: false);
			rightPulsingLine.SetActive(value: false);
			stinkEffect.Stop();
			Reset();
		}

		private void DeactivateColliders()
		{
			if ((bool)Colliders)
			{
				Colliders.SetActive(value: false);
			}
			if ((bool)MainCollider)
			{
				MainCollider.enabled = false;
			}
			colliderActivatorTimer.Run(0.2f);
		}

		private void ActivateColliders()
		{
			if ((bool)Colliders)
			{
				Colliders.SetActive(value: true);
			}
			if ((bool)MainCollider)
			{
				MainCollider.enabled = true;
			}
		}
	}
}
