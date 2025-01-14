using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App.Vehicles.Deformations
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(ImpactDeformable))]
	public class DeformationController : MonoBehaviour
	{
		[HideInInspector]
		public bool applyComponents;

		[Space(6f)]
		[Tooltip("use .6 as default, for large areas (e.g. Bus) use .4")]
		public float hardness = 0.6f;

		public GameObject mainParent;

		[Tooltip("Main Cabine parts that can deform but do not drop off")]
		public GameObject[] mainCabs;

		[Tooltip("Parts that can deform and can drop off. Once array is filled up, check Apply Components to apply")]
		public GameObject[] parts;

		public bool componentsApplied;

		public bool removeComponents;

		public bool tagAndLayerApplied;

		private ArrayList colliders;

		private ArrayList impacts;

		private ArrayList drops;

		private string impactlayerName = "Impact";

		private List<ImpactDeformable> impactDeformables;

		public bool AreComponentsApplied => componentsApplied;

		private void Start()
		{
			colliders = new ArrayList();
			impacts = new ArrayList();
			drops = new ArrayList();
		}

		private void Update()
		{
			if (!componentsApplied)
			{
				if (applyComponents)
				{
					Apply();
				}
				applyComponents = false;
			}
			if (removeComponents)
			{
				RemoveComponents();
				removeComponents = false;
			}
			if (!tagAndLayerApplied)
			{
				ApplyTagAndLayer();
				tagAndLayerApplied = true;
			}
		}

		public List<ImpactDeformable> GetDeformables()
		{
			if (impactDeformables == null)
			{
				impactDeformables = new List<ImpactDeformable>();
				if (mainCabs != null && mainCabs.Length != 0)
				{
					GameObject[] array = mainCabs;
					foreach (GameObject gameObject in array)
					{
						if (gameObject != null)
						{
							impactDeformables.Add(gameObject.GetComponent<ImpactDeformable>());
						}
					}
				}
				if (parts != null && parts.Length != 0)
				{
					GameObject[] array = parts;
					foreach (GameObject gameObject2 in array)
					{
						if (gameObject2 != null)
						{
							impactDeformables.Add(gameObject2.GetComponent<ImpactDeformable>());
						}
					}
				}
			}
			return impactDeformables;
		}

		private void Apply()
		{
			GameObject[] array;
			if (mainCabs != null && mainCabs.Length != 0)
			{
				array = mainCabs;
				foreach (GameObject item in array)
				{
					ApplyComponentsTo(item, canDrop: false);
				}
			}
			if (parts.Length == 0)
			{
				UnityEngine.Debug.Log("! exception: " + this + " ApplyComponents() Fill parts array by childs at first");
				applyComponents = false;
				return;
			}
			array = parts;
			foreach (GameObject item2 in array)
			{
				ApplyComponentsTo(item2);
			}
			componentsApplied = true;
		}

		private void ApplyComponentsTo(GameObject item, bool canDrop = true)
		{
			if (item.GetComponent<Collider>() == null)
			{
				item.AddComponent<BoxCollider>();
			}
			if (item.GetComponent<ImpactDeformable>() == null)
			{
				ImpactDeformable impactDeformable = item.AddComponent<ImpactDeformable>();
				impactDeformable.OverrideMaster = true;
				impactDeformable.Hardness = hardness;
			}
			if (canDrop && item.GetComponent<DropOff>() == null)
			{
				item.AddComponent<DropOff>();
			}
		}

		private void RemoveComponents()
		{
			for (int i = 0; i < parts.Length; i++)
			{
				GameObject gameObject = parts[i];
				BoxCollider[] components = gameObject.GetComponents<BoxCollider>();
				ArrayList arrayList = new ArrayList();
				for (int j = 0; j < components.Length; j++)
				{
					arrayList.Add(components[j]);
				}
				DropOff component = gameObject.GetComponent<DropOff>();
				ImpactDeformable component2 = gameObject.GetComponent<ImpactDeformable>();
				for (int num = arrayList.Count - 1; num >= 0; num--)
				{
					UnityEngine.Object.DestroyImmediate(arrayList[i] as BoxCollider);
				}
				UnityEngine.Object.DestroyImmediate(component);
				UnityEngine.Object.DestroyImmediate(component2);
			}
		}

		private void ApplyTagAndLayer()
		{
			if (mainCabs != null && mainCabs.Length != 0)
			{
				GameObject[] array = mainCabs;
				foreach (GameObject item in array)
				{
					ApplyTagsAndLayersTo(item);
				}
			}
			if (parts != null && parts.Length != 0)
			{
				GameObject[] array = parts;
				foreach (GameObject item2 in array)
				{
					ApplyTagsAndLayersTo(item2);
				}
			}
		}

		private void ApplyTagsAndLayersTo(GameObject item)
		{
			item.tag = "VehicleCollider";
			item.layer = LayerMask.NameToLayer(impactlayerName);
		}
	}
}
