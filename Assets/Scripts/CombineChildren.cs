using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("Mesh/Combine Children")]
public class CombineChildren : MonoBehaviour
{
	public bool generateTriangleStrips = true;

	private void Start()
	{
		Component[] componentsInChildren = GetComponentsInChildren(typeof(MeshFilter));
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		Hashtable hashtable = new Hashtable();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			MeshFilter meshFilter = (MeshFilter)componentsInChildren[i];
			Renderer component = componentsInChildren[i].GetComponent<Renderer>();
			MeshCombineUtility.MeshInstance meshInstance = default(MeshCombineUtility.MeshInstance);
			meshInstance.mesh = meshFilter.sharedMesh;
			if (!(component != null) || !component.enabled || !(meshInstance.mesh != null))
			{
				continue;
			}
			meshInstance.transform = worldToLocalMatrix * meshFilter.transform.localToWorldMatrix;
			Material[] sharedMaterials = component.sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				meshInstance.subMeshIndex = Math.Min(j, meshInstance.mesh.subMeshCount - 1);
				ArrayList arrayList = (ArrayList)hashtable[sharedMaterials[j]];
				if (arrayList != null)
				{
					arrayList.Add(meshInstance);
					continue;
				}
				arrayList = new ArrayList();
				arrayList.Add(meshInstance);
				hashtable.Add(sharedMaterials[j], arrayList);
			}
			component.enabled = false;
		}
		foreach (DictionaryEntry item in hashtable)
		{
			MeshCombineUtility.MeshInstance[] combines = (MeshCombineUtility.MeshInstance[])((ArrayList)item.Value).ToArray(typeof(MeshCombineUtility.MeshInstance));
			if (hashtable.Count == 1)
			{
				if (GetComponent(typeof(MeshFilter)) == null)
				{
					base.gameObject.AddComponent(typeof(MeshFilter));
				}
				if (!GetComponent<MeshRenderer>())
				{
					base.gameObject.AddComponent<MeshRenderer>();
				}
				((MeshFilter)GetComponent(typeof(MeshFilter))).mesh = MeshCombineUtility.Combine(combines, generateTriangleStrips);
				GetComponent<Renderer>().material = (Material)item.Key;
				GetComponent<Renderer>().enabled = true;
			}
			else
			{
				GameObject gameObject = new GameObject("Combined mesh");
				gameObject.transform.parent = base.transform;
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.AddComponent(typeof(MeshFilter));
				gameObject.AddComponent<MeshRenderer>();
				gameObject.GetComponent<Renderer>().material = (Material)item.Key;
				((MeshFilter)gameObject.GetComponent(typeof(MeshFilter))).mesh = MeshCombineUtility.Combine(combines, generateTriangleStrips);
			}
		}
	}
}
