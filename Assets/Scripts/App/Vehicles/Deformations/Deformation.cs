using App.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace App.Vehicles.Deformations
{
	[RequireComponent(typeof(ImpactDeformable))]
	[RequireComponent(typeof(DeformationController))]
	public class Deformation : VehicleDeformationBase
	{
		private bool applyToHealth = true;

		[Tooltip("Health - damage sensitivity, default:1.0")]
		[Range(0.1f, 1.5f)]
		public float healthSensitivity = 1f;

		[Space(12f)]
		public GameObject body;

		[Space(12f)]
		[Tooltip("Distance in world space - how much the relevant mesh vertice can shift/move until wheel can drop off")]
		[Range(0.1f, 0.5f)]
		public float wheelDropLimit = 0.3f;

		public GameObject[] wheels;

		[Tooltip("Apply the same order as wheels - to create pairs")]
		public WheelCollider[] wheelColliders;

		[Space(12f)]
		[Tooltip("Sit point vs virtual anchor of sit point offset. Once virtual anchor is damaged kick out of vehicle is called. (1 sport, .7 truck)")]
		[Range(0.5f, 1.5f)]
		public float sitPointAnchorOffset = 1f;

		[Tooltip("Relevant mesh vertice shift limit")]
		[Range(0.1f, 0.5f)]
		public float sitPointKickOutSensitivity = 0.1f;

		[Tooltip("Objects visualy related to driver kick out of vehicle situation (e.g. front window)")]
		public DropOff[] kickOutRelated;

		private Vector3 velocityBeforeLastDamage;

		private bool isKickedOut;

		[Space(12f)]
		public bool isWreck;

		public bool canDebug;

		private ImpactDeformable impactDeformableMain;

		private DeformationController deformationController;

		private int lastDamageFrame;

		private float lastDamageTime;

		private float lastDamageAmount;

		private Collider mainCollider;

		private float mainColliderVolume;

		private float damage;

		private Rigidbody carRigidbody;

		private Mesh bodyMesh;

		private Renderer bodyRenderer;

		private Vector3[] bodyVertices;

		private List<int> closestBodyWheelVertices_index;

		private List<Vector3> closestBodyWheelVertices_origin;

		private int closestBodySitVertice_index;

		private Vector3 closestBodySitVertice_origin;

		private float lastTotalDamagePercentage;

		private float totalDamagePercentage;

		private float newDamagePercentage;

		private List<ImpactDeformable> impactDeformables;

		private Rigidbody _rigidbody;

		private bool isTankSquashAreaCollision;

		private float lateDamageDelay = 0.3f;

		private Dictionary<int, DeformationData> deformationsData;

		private float damageCoeficient = 0.065f;

		private BoxCollider[] parentcolliders;

		private BoxCollider[] childColliders;

		private MeshCollider[] meshColliders;

		private ArrayList meshCollidersAndItsBoxSubstituents;

		private bool isInitializedOnAwake;

		private Bounds myBounds;

		private Vector3 colliderSize;

		private Vector3 colliderCenter;

		private int[] a1 = new int[3]
		{
			-1,
			1,
			1
		};

		private int[] a2 = new int[3]
		{
			1,
			1,
			1
		};

		private int[] a3 = new int[3]
		{
			1,
			-1,
			1
		};

		private int[] a4 = new int[3]
		{
			1,
			1,
			-1
		};

		private void Awake()
		{
			if (!isInitializedOnAwake)
			{
				InitializeOnAwake();
			}
			if (!isWreck)
			{
				AddListeners();
			}
			wheelDropLimit = 1f;
			healthSensitivity *= 0.3f;
		}

		private void OnDisable()
		{
			if (!isWreck)
			{
				RemoveListeners();
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			if (body != null)
			{
				bodyRenderer = body.GetComponent<Renderer>();
				if ((bool)body.GetComponent<MeshFilter>())
				{
					bodyMesh = body.GetComponent<MeshFilter>().mesh;
				}
				else
				{
					bodyMesh = body.GetComponentInChildren<MeshFilter>().mesh;
				}
				bodyVertices = bodyMesh.vertices;
			}
			else
			{
				UnityEngine.Debug.LogException(new Exception(this + " body reference is missing"));
			}
			parentcolliders = base.transform.GetComponents<BoxCollider>();
			childColliders = base.transform.GetComponentsInChildren<BoxCollider>();
			meshColliders = base.transform.GetComponentsInChildren<MeshCollider>();
			meshCollidersAndItsBoxSubstituents = new ArrayList();
			_rigidbody = GetComponent<Rigidbody>();
			if (meshColliders != null && meshColliders.Length != 0)
			{
				MeshCollider[] array = meshColliders;
				foreach (MeshCollider meshCollider in array)
				{
					BoxCollider boxCollider = meshCollider.gameObject.AddComponent<BoxCollider>();
					boxCollider.size = meshCollider.sharedMesh.bounds.size;
					boxCollider.center = meshCollider.sharedMesh.bounds.center;
					boxCollider.enabled = false;
					ArrayList arrayList = new ArrayList();
					arrayList.Add(meshCollider);
					arrayList.Add(boxCollider);
					meshCollidersAndItsBoxSubstituents.Add(arrayList);
				}
			}
			if (!isWreck)
			{
				ComputeCoumpoudedCollider();
				InitializeSitPointAnchor();
				InitializeWheelAnchors();
			}
		}

		private void InitializeOnAwake()
		{
			deformationController = GetComponent<DeformationController>();
			impactDeformableMain = GetComponent<ImpactDeformable>();
			impactDeformables = new List<ImpactDeformable>();
			foreach (ImpactDeformable deformable in deformationController.GetDeformables())
			{
				impactDeformables.Add(deformable);
			}
			CollectDeformablesData();
			isInitializedOnAwake = true;
		}

		private void AddListeners()
		{
			foreach (ImpactDeformable impactDeformable in impactDeformables)
			{
				impactDeformable.OnDeform += OnDeform;
			}
		}

		private void RemoveListeners()
		{
			foreach (ImpactDeformable impactDeformable in impactDeformables)
			{
				impactDeformable.OnDeform -= OnDeform;
			}
		}

		private void OnDeformForce(ImpactDeformable impactDeformable, Vector3 point, Vector3 force)
		{
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (!isTankCollision && (bool)collision.transform.GetComponentInParent<TankManager>())
			{
				isTankCollision = true;
				OnTankCollision();
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!isTankSquashAreaCollision)
			{
				if (other.CompareTag("ImpactSquashArea"))
				{
					isTankSquashAreaCollision = true;
					OnTankSquashAreaTrigger();
				}
				if (other.tag == "Impactor" && applyToHealth)
				{
					applyToHealth = false;
				}
			}
		}

		private void OnTankSquashAreaTrigger()
		{
			applyToHealth = false;
			GameObject[] array = wheels;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			WheelCollider[] array2 = wheelColliders;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].radius = 0.001f;
			}
			if (!isWreck)
			{
				charactersInVehicle.Kill();
				DestroyDoors();
				health.Squash(isCritical: false, 9);
			}
		}

		private void OnTankCollision()
		{
			SetDeformablesForTankCollision();
			BoxCollider[] array = parentcolliders;
			foreach (BoxCollider collider in array)
			{
				FlatColliders(collider);
			}
			array = childColliders;
			foreach (BoxCollider collider2 in array)
			{
				FlatColliders(collider2);
			}
		}

		private void SetDeformablesForTankCollision()
		{
			foreach (ImpactDeformable impactDeformable in impactDeformables)
			{
				if (impactDeformable != null)
				{
					impactDeformable.Hardness = 0.12f;
					impactDeformable.DeformationsScale = new Vector3(1.5f, 0.9f, 1.5f);
					impactDeformable.RandomFactorDeformation = 0.35f;
					if ((bool)impactDeformable.GetComponent<DropOff>())
					{
						impactDeformable.GetComponent<DropOff>().impactLimit = 0.02f;
					}
				}
			}
		}

		private bool ArraysEquals(int[] arr1, int[] arr2)
		{
			int num = 0;
			for (int i = 0; i < arr1.Length; i++)
			{
				if (arr1[i] == arr2[i])
				{
					num++;
				}
			}
			return num == 3;
		}

		private void FlatColliderProgressively(BoxCollider collider)
		{
			colliderSize = collider.bounds.size;
			colliderSize.y -= colliderSize.y * 0.4f;
			collider.size = colliderSize;
		}

		private void FlatCollider(BoxCollider collider, float sizeY)
		{
			colliderSize = collider.bounds.size;
			colliderSize.y = sizeY;
			collider.size = colliderSize;
		}

		private void ComputeCoumpoudedCollider()
		{
			Collider[] components = GetComponents<Collider>();
			myBounds = new Bounds(base.transform.position, Vector3.zero);
			Collider[] array = components;
			foreach (Collider collider in array)
			{
				myBounds.Encapsulate(collider.bounds);
			}
			mainColliderVolume = GetVolume(myBounds.size);
		}

		public void WheelsOff()
		{
		}

		private void WheelOff(GameObject wheel, WheelCollider wheelCollider)
		{
			wheel.SetActive(value: false);
			wheelCollider.radius = 0.001f;
		}

		private void InitializeSitPointAnchor()
		{
			Vector3 vector = vehicleComponents.sitPoint.transform.position + base.transform.forward * sitPointAnchorOffset + base.transform.up * 0.5f;
			float num = 1000f;
			float num2 = 1000f;
			int num3 = 0;
			for (int i = 0; i < bodyVertices.Length; i++)
			{
				num = Vector3.Distance(body.transform.TransformPoint(bodyVertices[i]), vector);
				if (num < num2)
				{
					num3 = i;
					num2 = num;
				}
			}
			if (canDebug)
			{
				GameObject gameObject = new GameObject("sitPoint_anchor");
				gameObject.transform.position = vector;
				gameObject.transform.parent = body.transform;
				gameObject.transform.localRotation = Quaternion.identity;
				GameObject gameObject2 = new GameObject("sitPoint_closestVertice");
				gameObject2.transform.position = body.transform.TransformPoint(bodyVertices[num3]);
				gameObject2.transform.parent = body.transform;
				gameObject2.transform.localRotation = Quaternion.identity;
			}
			closestBodySitVertice_index = num3;
			closestBodySitVertice_origin = bodyVertices[num3];
		}

		private void CheckSitPointAnchor()
		{
			if (!(body == null) && !(vehicleComponents.driver == null))
			{
				bodyVertices = bodyMesh.vertices;
				Vector3 a = body.transform.TransformPoint(closestBodySitVertice_origin);
				Vector3 b = body.transform.TransformPoint(bodyVertices[closestBodySitVertice_index]);
				if (Vector3.Distance(a, b) > sitPointKickOutSensitivity)
				{
					KickOut();
				}
			}
		}

		private void KickOut()
		{
			if (kickOutRelated.Length != 0)
			{
				DropOff[] array = kickOutRelated;
				foreach (DropOff dropOff in array)
				{
					if (dropOff != null)
					{
						dropOff.Now();
					}
				}
			}
			RagdollHelper componentSafe = vehicleComponents.driver.GetComponentSafe<RagdollHelper>();
			vehicleComponents.KickOffCurrentDriver(relocateCharacter: true, relocateForward: true);
			componentSafe.SetRagdollVelocity(velocityBeforeLastDamage);
			isKickedOut = true;
		}

		private Vector3 ProjectPoint(Transform target, Vector3 aPoint)
		{
			Vector3 b = Vector3.Project(aPoint - target.position, target.forward);
			return target.position + b;
		}

		private void InitializeWheelAnchors()
		{
			if (body == null)
			{
				return;
			}
			closestBodyWheelVertices_index = new List<int>();
			closestBodyWheelVertices_origin = new List<Vector3>();
			for (int i = 0; i < wheels.Length; i++)
			{
				GameObject gameObject = wheels[i];
				Vector3 vector = body.transform.position - gameObject.transform.position;
				float x = vector.x;
				float z = vector.z;
				Vector3 vector2 = gameObject.transform.position + -vector * 0.3f + vector.normalized * -0.2f;
				float num = 1000f;
				float num2 = 1000f;
				int num3 = 0;
				for (int j = 0; j < bodyVertices.Length; j++)
				{
					num = Vector3.Distance(body.transform.TransformPoint(bodyVertices[j]), vector2);
					if (num < num2)
					{
						num3 = j;
						num2 = num;
					}
				}
				if (canDebug)
				{
					GameObject gameObject2 = new GameObject(gameObject.name + "_anchor");
					gameObject2.transform.position = vector2;
					gameObject2.transform.parent = body.transform;
					GameObject gameObject3 = new GameObject(gameObject.name + "_closestVertice");
					gameObject3.transform.position = body.transform.TransformPoint(bodyVertices[num3]);
					gameObject3.transform.parent = body.transform;
				}
				closestBodyWheelVertices_index.Insert(i, num3);
				closestBodyWheelVertices_origin.Insert(i, bodyVertices[num3]);
			}
		}

		private void CheckWheelAnchors()
		{
			if (body == null)
			{
				return;
			}
			bodyVertices = bodyMesh.vertices;
			for (int i = 0; i < wheels.Length; i++)
			{
				Vector3 a = body.transform.TransformPoint(closestBodyWheelVertices_origin[i]);
				Vector3 b = body.transform.TransformPoint(bodyVertices[closestBodyWheelVertices_index[i]]);
				if (Vector3.Distance(a, b) > wheelDropLimit)
				{
					WheelOff(wheels[i], wheelColliders[i]);
				}
			}
		}

		private void OnDeform(ImpactDeformable impactDeformableLocal)
		{
			velocityBeforeLastDamage = _rigidbody.velocity;
			applyToHealth = ((vehicleComponents.driver != null) ? true : false);
			CancelInvoke("LateDamage");
			Invoke("LateDamage", lateDamageDelay);
		}

		private void LateDamage()
		{
			if (!isKickedOut)
			{
				CheckSitPointAnchor();
			}
			CheckWheelAnchors();
			totalDamagePercentage = 0f;
			for (int i = 0; i < deformationsData.Count; i++)
			{
				DeformationData value = deformationsData.ElementAt(i).Value;
				if (value.impactDeformable != null)
				{
					float num = value.colliderVolume / mainColliderVolume;
					float num2 = GetDamage(value.impactDeformable) * num;
					if (value.impactDeformable.StructuralDamage > 0f)
					{
						totalDamagePercentage += num2;
					}
				}
			}
			newDamagePercentage = totalDamagePercentage - lastTotalDamagePercentage;
			lastTotalDamagePercentage = totalDamagePercentage;
			damage = health.maxHealth * newDamagePercentage;
			if (applyToHealth)
			{
				health.ApplyDamage(damage, 3);
			}
		}

		private void CollectDeformablesData()
		{
			deformationsData = new Dictionary<int, DeformationData>();
			foreach (ImpactDeformable impactDeformable in impactDeformables)
			{
				if (impactDeformable != null)
				{
					int instanceID = impactDeformable.GetInstanceID();
					if (!deformationsData.ContainsKey(instanceID))
					{
						deformationsData.Add(instanceID, new DeformationData(impactDeformable));
					}
				}
			}
		}

		private float GetDamage(ImpactDeformable impactDeformable)
		{
			return Mathf.Clamp01(impactDeformable.AverageStructuralDamage * healthSensitivity / damageCoeficient);
		}

		private float GetVolume(Vector3 vector3)
		{
			return vector3.x * vector3.y * vector3.z;
		}

		private void FlatColliders(BoxCollider collider)
		{
			if (canDebug)
			{
				MonoBehaviour.print(this + " FlatColliders() " + collider.name);
			}
			if (canDebug)
			{
				MonoBehaviour.print("    UP");
			}
			int[] array = new int[3];
			Vector3 vector = collider.transform.TransformDirection(Vector3.up);
			Vector3 vector2 = base.transform.position + Vector3.up * 10f - collider.transform.position;
			float num = Vector3.Dot(vector, vector2);
			if (num > 0f)
			{
				if (canDebug)
				{
					MonoBehaviour.print("Back");
				}
				array[0] = 1;
			}
			if (num < 0f)
			{
				if (canDebug)
				{
					MonoBehaviour.print("Front");
				}
				array[0] = -1;
			}
			if (num == 0f)
			{
				if (canDebug)
				{
					MonoBehaviour.print("Side");
				}
				array[0] = 0;
			}
			if (canDebug)
			{
				MonoBehaviour.print(vector);
			}
			if (canDebug)
			{
				MonoBehaviour.print(vector2);
			}
			if (canDebug)
			{
				MonoBehaviour.print(num);
			}
			if (canDebug)
			{
				MonoBehaviour.print("    FWD");
			}
			vector = collider.transform.TransformDirection(Vector3.forward);
			num = Vector3.Dot(vector, vector2);
			if (num > 0f)
			{
				if (canDebug)
				{
					MonoBehaviour.print("Back");
				}
				array[1] = 1;
			}
			if (num < 0f)
			{
				if (canDebug)
				{
					MonoBehaviour.print("Front");
				}
				array[1] = -1;
			}
			if (num == 0f)
			{
				if (canDebug)
				{
					MonoBehaviour.print("Side");
				}
				array[1] = 0;
			}
			if (canDebug)
			{
				MonoBehaviour.print(vector);
			}
			if (canDebug)
			{
				MonoBehaviour.print(vector2);
			}
			if (canDebug)
			{
				MonoBehaviour.print(num);
			}
			if (canDebug)
			{
				MonoBehaviour.print("    RIGHT");
			}
			vector = collider.transform.TransformDirection(Vector3.right);
			num = Vector3.Dot(vector, vector2);
			if (num > 0f)
			{
				if (canDebug)
				{
					MonoBehaviour.print("Back");
				}
				array[2] = 1;
			}
			if (num < 0f)
			{
				if (canDebug)
				{
					MonoBehaviour.print("Front");
				}
				array[2] = -1;
			}
			if (num == 0f)
			{
				if (canDebug)
				{
					MonoBehaviour.print("Side");
				}
				array[2] = 0;
			}
			if (canDebug)
			{
				MonoBehaviour.print(vector);
			}
			if (canDebug)
			{
				MonoBehaviour.print(vector2);
			}
			if (canDebug)
			{
				MonoBehaviour.print(num);
			}
			if (canDebug)
			{
				MonoBehaviour.print(array[0]);
			}
			if (canDebug)
			{
				MonoBehaviour.print(array[1]);
			}
			if (canDebug)
			{
				MonoBehaviour.print(array[2]);
			}
			bool flag = array.SequenceEqual(a1);
			bool flag2 = array.SequenceEqual(a2);
			bool flag3 = array.SequenceEqual(a3);
			bool flag4 = array.SequenceEqual(a4);
			if (canDebug)
			{
				MonoBehaviour.print(flag);
			}
			if (canDebug)
			{
				MonoBehaviour.print(flag2);
			}
			if (canDebug)
			{
				MonoBehaviour.print(flag3);
			}
			if (canDebug)
			{
				MonoBehaviour.print(flag4);
			}
			colliderSize = collider.size;
			colliderCenter = Vector3.zero;
			float num2 = 0.5f;
			float num3 = 0.2f;
			if (flag)
			{
				colliderSize.z *= num2;
				colliderCenter = new Vector3(0f, 0f, 0f - num3);
			}
			if (flag2)
			{
				colliderSize.y *= num2;
				colliderCenter = new Vector3(0f, 0f - num3, 0f);
			}
			if (flag3)
			{
				colliderSize.z *= num2;
				colliderCenter = new Vector3(0f, 0f, num3);
			}
			if (flag4)
			{
				colliderSize.x *= num2;
				colliderCenter = new Vector3(num3, 0f, 0f);
			}
			collider.size = colliderSize;
			collider.center += colliderCenter;
		}
	}
}
