using System;
using System.Collections.Generic;
using UnityEngine;

namespace Forge3D
{
	[Serializable]
	public class F3DTurretConstructor : MonoBehaviour
	{
		public GameObject[] Breeches;

		public GameObject[] Barrels;

		public GameObject Base;

		public GameObject Swivel;

		public GameObject Mount;

		public GameObject Head;

		public int turretIndex;

		public bool needUpdateListOfTemplates;

		public int GetSelectedType()
		{
			return turretIndex;
		}

		public void ChangeTurretIndex(int index, TurretStructure struc)
		{
			turretIndex = index;
			UpdateFullTurret(struc);
		}

		private bool CheckForName(string[] names, string name)
		{
			for (int i = 0; i < names.Length; i++)
			{
				if (names[i] == name)
				{
					return true;
				}
			}
			return false;
		}

		public void UpdateFullTurret(TurretStructure struc)
		{
			if (Application.isPlaying)
			{
				return;
			}
			UpdateTurret(struc);
			if (struc.NeedLOD)
			{
				InstallLods();
			}
			if (struc.HasTurretScript)
			{
				InstallTurretController();
				return;
			}
			F3DTurret component = GetComponent<F3DTurret>();
			if (component != null)
			{
				component.destroyIt = true;
			}
		}

		public void UnlinkGameObject()
		{
			UnityEngine.Object.DestroyImmediate(this);
		}

		private void InstallLods()
		{
			if (base.transform == null || base.transform.childCount == 0)
			{
				return;
			}
			Transform child = base.transform.GetChild(0);
			if (!(child != null) || !(child.gameObject.GetComponent<LODGroup>() == null))
			{
				return;
			}
			LODGroup lODGroup = child.gameObject.AddComponent<LODGroup>();
			Renderer[] componentsInChildren = child.gameObject.GetComponentsInChildren<Renderer>();
			List<List<Renderer>> list = new List<List<Renderer>>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				string text = componentsInChildren[i].gameObject.name;
				if (text[text.Length - 1] == ')')
				{
					string oldValue = "(Clone)";
					text = text.Replace(oldValue, "");
				}
				int result = -1;
				if (!int.TryParse(text[text.Length - 1].ToString(), out result))
				{
					continue;
				}
				if (list.Count <= result + 1)
				{
					for (int j = 0; j < result + 1 - list.Count; j++)
					{
						list.Add(new List<Renderer>());
					}
				}
				list[result].Add(componentsInChildren[i]);
			}
			LOD[] array = new LOD[list.Count];
			for (int k = 0; k < array.Length; k++)
			{
				array[k].renderers = list[k].ToArray();
				float num = 10f / (float)array.Length * (float)(k + 1);
				if ((float)k < (float)array.Length * 0.5f)
				{
					array[k].screenRelativeTransitionHeight = (-1.5f * num + 10f) / 10f + 0.02f;
				}
				else
				{
					array[k].screenRelativeTransitionHeight = (-0.5f * num + 5f) / 10f + 0.02f;
				}
			}
			lODGroup.SetLODs(array);
			lODGroup.RecalculateBounds();
		}

		private void InstallTurretController()
		{
			F3DTurret component = base.gameObject.GetComponent<F3DTurret>();
			if (component == null)
			{
				F3DTurret[] componentsInChildren = base.gameObject.GetComponentsInChildren<F3DTurret>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i] != null)
					{
						componentsInChildren[i].destroyIt = true;
					}
				}
				F3DTurret f3DTurret = base.gameObject.AddComponent<F3DTurret>();
				f3DTurret.smoothControlling = true;
				f3DTurret.HeadingLimit.x = -60f;
				f3DTurret.HeadingLimit.y = 60f;
				f3DTurret.ElevationLimit.x = -60f;
				f3DTurret.ElevationLimit.y = 60f;
				f3DTurret.ElevationTrackingSpeed = 30f;
				f3DTurret.HeadingTrackingSpeed = 30f;
				string[] names = new string[2]
				{
					"MOUNT",
					"Mount"
				};
				f3DTurret.Mount = FindGameObject(names, base.gameObject);
				string[] names2 = new string[2]
				{
					"SWIVEL",
					"Swivel"
				};
				f3DTurret.Swivel = FindGameObject(names2, base.gameObject);
			}
			else if (component.Swivel == null || component.Mount == null)
			{
				string[] names3 = new string[2]
				{
					"MOUNT",
					"Mount"
				};
				component.Mount = FindGameObject(names3, base.gameObject);
				string[] names4 = new string[2]
				{
					"SWIVEL",
					"Swivel"
				};
				component.Swivel = FindGameObject(names4, base.gameObject);
			}
		}

		private GameObject FindGameObject(string[] names, GameObject parent)
		{
			Transform[] componentsInChildren = parent.GetComponentsInChildren<Transform>();
			foreach (Transform transform in componentsInChildren)
			{
				if (!(transform != null))
				{
					continue;
				}
				string[] array = transform.name.Split('_');
				for (int j = 0; j < array.Length; j++)
				{
					if (CheckForName(names, array[j]))
					{
						return transform.gameObject;
					}
				}
			}
			return null;
		}

		public void UpdateTurret(TurretStructure curStructure)
		{
			if (curStructure != null)
			{
				UpdateBase(curStructure.Base);
				UpdateSwivel(curStructure.Swivel, curStructure.SwivelPrefix);
				UpdateHead(curStructure.Head, curStructure.HeadPrefix);
				UpdateMount(curStructure.Mount, curStructure.MountPrefix);
				UpdateBreeches(curStructure.WeaponBreeches, curStructure.WeaponSlotsNames, curStructure.WeaponBarrels, curStructure.WeaponBarrelSockets);
			}
		}

		private void UpdateBreeches(List<GameObject> breeches, List<string> socketNames, List<GameObject> barrels, List<string> barrelSocketNames)
		{
			int num = 0;
			if (Breeches != null)
			{
				for (num = 0; num < Breeches.Length; num++)
				{
					UnityEngine.Object.DestroyImmediate(Breeches[num]);
				}
			}
			List<GameObject> list = new List<GameObject>();
			List<GameObject> list2 = new List<GameObject>();
			if (!(Base != null))
			{
				return;
			}
			Transform[] componentsInChildren = Base.GetComponentsInChildren<Transform>();
			foreach (Transform transform in componentsInChildren)
			{
				for (num = 0; num < socketNames.Count; num++)
				{
					if (transform.name == socketNames[num])
					{
						if (breeches[num] != null)
						{
							GameObject gameObject = UnityEngine.Object.Instantiate(breeches[num], Vector3.zero, Quaternion.identity);
							gameObject.transform.parent = transform;
							gameObject.transform.position = transform.position;
							gameObject.transform.rotation = transform.rotation;
							gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
							list2.Add(UpdateBarrel(barrels[num], gameObject, barrelSocketNames[num]));
							list.Add(gameObject);
						}
						else
						{
							list2.Add(UpdateBarrel(barrels[num], transform.gameObject, barrelSocketNames[num]));
							list.Add(null);
						}
					}
				}
			}
			Breeches = list.ToArray();
		}

		private GameObject UpdateBarrel(GameObject barrelGO, GameObject parentGO, string socket)
		{
			if (parentGO == null || barrelGO == null)
			{
				return null;
			}
			Transform[] componentsInChildren = parentGO.GetComponentsInChildren<Transform>();
			foreach (Transform transform in componentsInChildren)
			{
				if (transform.name == socket)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(barrelGO, Vector3.zero, Quaternion.identity);
					gameObject.transform.parent = transform;
					gameObject.transform.position = transform.position;
					gameObject.transform.rotation = transform.rotation;
					gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
					return gameObject;
				}
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(barrelGO, Vector3.zero, Quaternion.identity);
			gameObject2.transform.parent = parentGO.transform;
			gameObject2.transform.position = parentGO.transform.position;
			gameObject2.transform.rotation = parentGO.transform.rotation;
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			return gameObject2;
		}

		private void UpdateHead(GameObject headGO, string prefix)
		{
			if (Head != null)
			{
				if (Head.transform.childCount > 0)
				{
					Transform[] componentsInChildren = Head.GetComponentsInChildren<Transform>();
					foreach (Transform transform in componentsInChildren)
					{
						if (transform != null && transform != Head.transform && transform.gameObject != base.gameObject)
						{
							UnityEngine.Object.DestroyImmediate(transform.gameObject);
						}
					}
				}
				UnityEngine.Object.DestroyImmediate(Head);
			}
			if (!(headGO != null))
			{
				return;
			}
			if (Base != null)
			{
				Transform[] componentsInChildren = Base.GetComponentsInChildren<Transform>();
				int i = 0;
				Transform transform2;
				while (true)
				{
					if (i < componentsInChildren.Length)
					{
						transform2 = componentsInChildren[i];
						if (transform2.name == prefix)
						{
							break;
						}
						i++;
						continue;
					}
					return;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(headGO, Vector3.zero, Quaternion.identity);
				gameObject.transform.parent = transform2;
				gameObject.transform.position = transform2.position;
				gameObject.transform.rotation = transform2.rotation;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				Head = gameObject;
			}
			else
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(headGO, Vector3.zero, Quaternion.identity);
				gameObject2.transform.parent = base.gameObject.transform;
				gameObject2.transform.position = base.gameObject.transform.position;
				gameObject2.transform.rotation = base.gameObject.transform.rotation;
				gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
				Head = gameObject2;
			}
		}

		private void UpdateMount(GameObject mountGO, string prefix)
		{
			if (Mount != null)
			{
				if (Mount.transform.childCount > 0)
				{
					Transform[] componentsInChildren = Mount.GetComponentsInChildren<Transform>();
					foreach (Transform transform in componentsInChildren)
					{
						if (transform != null && transform != Mount.transform && transform.gameObject != base.gameObject)
						{
							UnityEngine.Object.DestroyImmediate(transform.gameObject);
						}
					}
				}
				UnityEngine.Object.DestroyImmediate(Mount);
			}
			if (!(mountGO != null))
			{
				return;
			}
			if (Base != null)
			{
				Transform[] componentsInChildren = Base.GetComponentsInChildren<Transform>();
				int i = 0;
				Transform transform2;
				while (true)
				{
					if (i < componentsInChildren.Length)
					{
						transform2 = componentsInChildren[i];
						if (transform2.name == prefix)
						{
							break;
						}
						i++;
						continue;
					}
					return;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(mountGO, Vector3.zero, Quaternion.identity);
				gameObject.transform.parent = transform2;
				gameObject.transform.position = transform2.position;
				gameObject.transform.rotation = transform2.rotation;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				Mount = gameObject;
				if (Base == null)
				{
					Base = Mount;
					Swivel = Mount;
				}
			}
			else
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(mountGO, Vector3.zero, Quaternion.identity);
				gameObject2.transform.parent = base.gameObject.transform;
				gameObject2.transform.position = base.gameObject.transform.position;
				gameObject2.transform.rotation = base.gameObject.transform.rotation;
				gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
				Mount = gameObject2;
				Base = Mount;
				Swivel = Mount;
			}
		}

		private void UpdateSwivel(GameObject swivelGO, string prefix)
		{
			if (Swivel != null)
			{
				if (Swivel.transform.childCount > 0)
				{
					Transform[] componentsInChildren = Swivel.GetComponentsInChildren<Transform>();
					foreach (Transform transform in componentsInChildren)
					{
						if (transform != null && transform != Swivel.transform && transform.gameObject != base.gameObject)
						{
							UnityEngine.Object.DestroyImmediate(transform.gameObject);
						}
					}
				}
				UnityEngine.Object.DestroyImmediate(Swivel);
			}
			if (!(swivelGO != null))
			{
				return;
			}
			if (Base != null)
			{
				Transform[] componentsInChildren = Base.GetComponentsInChildren<Transform>();
				int i = 0;
				Transform transform2;
				while (true)
				{
					if (i < componentsInChildren.Length)
					{
						transform2 = componentsInChildren[i];
						if (transform2.name == prefix)
						{
							break;
						}
						i++;
						continue;
					}
					return;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(swivelGO, Vector3.zero, Quaternion.identity);
				gameObject.transform.parent = transform2;
				gameObject.transform.position = transform2.position;
				gameObject.transform.rotation = transform2.rotation;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				Swivel = gameObject;
				if (Base == null)
				{
					Base = Swivel;
				}
			}
			else
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(swivelGO, Vector3.zero, Quaternion.identity);
				gameObject2.transform.parent = base.gameObject.transform;
				gameObject2.transform.position = base.gameObject.transform.position;
				gameObject2.transform.rotation = base.gameObject.transform.rotation;
				gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
				Swivel = gameObject2;
				Base = Swivel;
			}
		}

		private void UpdateBase(GameObject baseGO)
		{
			if (base.gameObject.transform.childCount > 0)
			{
				Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
				foreach (Transform transform in componentsInChildren)
				{
					if (transform != null && transform.gameObject != base.gameObject)
					{
						UnityEngine.Object.DestroyImmediate(transform.gameObject);
					}
				}
			}
			if (baseGO != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(baseGO, Vector3.zero, Quaternion.identity);
				gameObject.transform.parent = base.gameObject.transform;
				gameObject.transform.position = base.gameObject.transform.position;
				gameObject.transform.rotation = base.gameObject.transform.rotation;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				Base = gameObject;
			}
			else
			{
				Base = null;
			}
		}
	}
}
