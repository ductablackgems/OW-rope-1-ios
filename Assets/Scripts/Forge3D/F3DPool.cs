using System.Collections.Generic;
using UnityEngine;

namespace Forge3D
{
	public class F3DPool : MonoBehaviour
	{
		[HideInInspector]
		public string poolName = "GeneratedPool";

		[HideInInspector]
		public Transform[] templates = new Transform[0];

		[HideInInspector]
		public Transform[] templatesParent = new Transform[0];

		[HideInInspector]
		public int[] poolLength = new int[0];

		[HideInInspector]
		public int[] poolLengthCurrent = new int[0];

		[HideInInspector]
		public int[] poolLengthMax = new int[0];

		[HideInInspector]
		public string broadcastSpawnName = "OnSpawned";

		[HideInInspector]
		public string broadcastDespawnName = "OnDespawned";

		[HideInInspector]
		public bool delayedSpawnInInstall = true;

		[HideInInspector]
		public int objectsPerUpdate = -1;

		[HideInInspector]
		public bool needBroadcasting = true;

		[HideInInspector]
		public bool optimizeSpawn = true;

		[HideInInspector]
		public int targetFPS = 50;

		[HideInInspector]
		public bool pooling = true;

		[HideInInspector]
		public bool needDebugging;

		[HideInInspector]
		public bool needSort = true;

		[HideInInspector]
		public bool needParenting = true;

		private float normalValue;

		private Dictionary<Transform, List<Transform>> readyObjects = new Dictionary<Transform, List<Transform>>();

		private Dictionary<Transform, List<Transform>> occupiedObjects = new Dictionary<Transform, List<Transform>>();

		private bool installing = true;

		public int GetTemplatesCount()
		{
			if (templates != null)
			{
				return templates.Length;
			}
			return -1;
		}

		private bool CheckForExistingTemplate(Transform obj)
		{
			for (int i = 0; i < templates.Length; i++)
			{
				if (obj == templates[i])
				{
					return true;
				}
			}
			return false;
		}

		public bool Despawn(Transform obj)
		{
			bool result = false;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (num3 = 0; num3 < occupiedObjects.Count; num3++)
			{
				List<Transform> list = occupiedObjects[templates[num3]];
				int count = list.Count;
				for (num = 0; num < count; num++)
				{
					if (list[num] == obj)
					{
						result = true;
						list.RemoveAt(num);
						readyObjects[templates[num3]].Add(obj);
						obj.gameObject.SetActive(value: false);
						if (needSort && needParenting)
						{
							obj.parent = templatesParent[num2];
						}
						if (needBroadcasting && broadcastDespawnName != "")
						{
							obj.BroadcastMessage(broadcastDespawnName, SendMessageOptions.DontRequireReceiver);
						}
						break;
					}
				}
				num2++;
			}
			return result;
		}

		public bool Despawn(Transform obj, Transform template)
		{
			bool flag = false;
			for (int i = 0; i < templates.Length; i++)
			{
				if (!(templates[i] == template))
				{
					continue;
				}
				int count = occupiedObjects[template].Count;
				for (int j = 0; j < count; j++)
				{
					if (occupiedObjects[template][j] == obj)
					{
						flag = true;
						occupiedObjects[template].RemoveAt(j);
						obj.gameObject.SetActive(value: false);
						if (needSort)
						{
							obj.parent = templatesParent[i];
						}
						readyObjects[template].Add(obj);
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
			return flag;
		}

		public int GetTemplatePosition(Transform obj)
		{
			int result = -1;
			for (int i = 0; i < templates.Length; i++)
			{
				if (obj == templates[i])
				{
					result = i;
				}
			}
			return result;
		}

		public Transform SpawnAudio(Transform obj, AudioClip clip, Vector3 pos, Transform par)
		{
			int templatePosition = GetTemplatePosition(obj);
			for (int i = 0; i < readyObjects[templates[templatePosition]].Count; i++)
			{
				if (!readyObjects[templates[templatePosition]][i].gameObject.activeSelf)
				{
					Transform transform = readyObjects[templates[templatePosition]][i];
					readyObjects[templates[templatePosition]].RemoveAt(i);
					occupiedObjects[templates[templatePosition]].Add(transform);
					transform.position = pos;
					if (par == null)
					{
						transform.SetParent(null);
					}
					else
					{
						transform.SetParent(par);
					}
					transform.gameObject.SetActive(value: true);
					if (needBroadcasting && broadcastSpawnName != "")
					{
						transform.BroadcastMessage(broadcastSpawnName, SendMessageOptions.DontRequireReceiver);
					}
					AudioSource component = transform.gameObject.GetComponent<AudioSource>();
					if (component != null)
					{
						component.clip = clip;
					}
					return transform;
				}
			}
			if (poolLengthCurrent[templatePosition] < poolLengthMax[templatePosition])
			{
				poolLengthCurrent[templatePosition]++;
				Transform transform2 = UnityEngine.Object.Instantiate(templates[templatePosition], Vector3.zero, Quaternion.identity);
				transform2.transform.parent = base.gameObject.transform;
				transform2.gameObject.SetActive(value: true);
				transform2.position = pos;
				if (par == null)
				{
					transform2.SetParent(null);
				}
				else
				{
					transform2.SetParent(par);
				}
				occupiedObjects[templates[templatePosition]].Add(transform2);
				if (needBroadcasting && broadcastSpawnName != "")
				{
					transform2.BroadcastMessage(broadcastSpawnName, SendMessageOptions.DontRequireReceiver);
				}
				AudioSource component2 = transform2.gameObject.GetComponent<AudioSource>();
				if (component2 != null)
				{
					component2.clip = clip;
				}
				return transform2;
			}
			return null;
		}

		public Transform Spawn(Transform obj, Transform par, Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion))
		{
			if (!CheckForExistingTemplate(obj))
			{
				if (needDebugging)
				{
					UnityEngine.Debug.LogWarning(obj.name + " isn't in " + base.gameObject.name + "'s pool");
				}
				return null;
			}
			int templatePosition = GetTemplatePosition(obj);
			for (int i = 0; i < readyObjects[obj].Count; i++)
			{
				if (!readyObjects[obj][i].gameObject.activeSelf)
				{
					Transform transform = readyObjects[obj][i];
					readyObjects[obj].RemoveAt(i);
					occupiedObjects[obj].Add(transform);
					transform.position = pos;
					transform.rotation = rot;
					if (par == null)
					{
						transform.SetParent(null);
					}
					else
					{
						transform.SetParent(par);
					}
					transform.gameObject.SetActive(value: true);
					if (needBroadcasting && broadcastSpawnName != "")
					{
						transform.BroadcastMessage(broadcastSpawnName, SendMessageOptions.DontRequireReceiver);
					}
					return transform;
				}
			}
			if (poolLengthCurrent[templatePosition] < poolLengthMax[templatePosition])
			{
				poolLengthCurrent[templatePosition]++;
				Transform transform2 = UnityEngine.Object.Instantiate(templates[templatePosition], Vector3.zero, Quaternion.identity);
				transform2.transform.parent = base.gameObject.transform;
				transform2.gameObject.SetActive(value: true);
				transform2.position = pos;
				transform2.rotation = rot;
				if (par == null)
				{
					transform2.SetParent(null);
				}
				else
				{
					transform2.SetParent(par);
				}
				occupiedObjects[obj].Add(transform2);
				if (needBroadcasting && broadcastSpawnName != "")
				{
					transform2.BroadcastMessage(broadcastSpawnName, SendMessageOptions.DontRequireReceiver);
				}
				return transform2;
			}
			return null;
		}

		public Transform Spawn(Transform obj, Vector3 pos, Quaternion rot, Transform par)
		{
			if (!CheckForExistingTemplate(obj))
			{
				if (needDebugging)
				{
					UnityEngine.Debug.LogWarning(obj.name + " isn't in " + base.gameObject.name + "'s pool");
				}
				return null;
			}
			int templatePosition = GetTemplatePosition(obj);
			for (int i = 0; i < readyObjects[obj].Count; i++)
			{
				if (!readyObjects[obj][i].gameObject.activeSelf)
				{
					Transform transform = readyObjects[obj][i];
					readyObjects[obj].RemoveAt(i);
					occupiedObjects[obj].Add(transform);
					transform.position = pos;
					transform.rotation = rot;
					if (par == null)
					{
						transform.SetParent(null);
					}
					else
					{
						transform.SetParent(par);
					}
					transform.gameObject.SetActive(value: true);
					if (needBroadcasting && broadcastSpawnName != "")
					{
						transform.BroadcastMessage(broadcastSpawnName, SendMessageOptions.DontRequireReceiver);
					}
					return transform;
				}
			}
			if (poolLengthCurrent[templatePosition] < poolLengthMax[templatePosition])
			{
				poolLengthCurrent[templatePosition]++;
				Transform transform2 = UnityEngine.Object.Instantiate(templates[templatePosition], Vector3.zero, Quaternion.identity);
				transform2.transform.parent = base.gameObject.transform;
				transform2.gameObject.SetActive(value: true);
				transform2.position = pos;
				transform2.rotation = rot;
				if (par == null)
				{
					transform2.SetParent(null);
				}
				else
				{
					transform2.SetParent(par);
				}
				occupiedObjects[obj].Add(transform2);
				if (needBroadcasting && broadcastSpawnName != "")
				{
					transform2.BroadcastMessage(broadcastSpawnName, SendMessageOptions.DontRequireReceiver);
				}
				return transform2;
			}
			return null;
		}

		public Transform Spawn(Transform obj, Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion))
		{
			if (!CheckForExistingTemplate(obj))
			{
				if (needDebugging)
				{
					UnityEngine.Debug.LogWarning(obj.name + " isn't in " + base.gameObject.name + "'s pool");
				}
				return null;
			}
			int templatePosition = GetTemplatePosition(obj);
			for (int i = 0; i < readyObjects[obj].Count; i++)
			{
				if (!readyObjects[obj][i].gameObject.activeSelf)
				{
					Transform transform = readyObjects[obj][i];
					readyObjects[obj].RemoveAt(i);
					occupiedObjects[obj].Add(transform);
					transform.position = pos;
					transform.rotation = rot;
					transform.SetParent(null);
					transform.gameObject.SetActive(value: true);
					if (needBroadcasting && broadcastSpawnName != "")
					{
						transform.BroadcastMessage(broadcastSpawnName, SendMessageOptions.DontRequireReceiver);
					}
					return transform;
				}
			}
			if (poolLengthCurrent[templatePosition] < poolLengthMax[templatePosition])
			{
				poolLengthCurrent[templatePosition]++;
				Transform transform2 = UnityEngine.Object.Instantiate(templates[templatePosition], Vector3.zero, Quaternion.identity);
				transform2.transform.parent = base.gameObject.transform;
				transform2.gameObject.SetActive(value: true);
				transform2.position = pos;
				transform2.rotation = rot;
				transform2.SetParent(null);
				occupiedObjects[obj].Add(transform2);
				if (needBroadcasting && broadcastSpawnName != "")
				{
					transform2.BroadcastMessage(broadcastSpawnName, SendMessageOptions.DontRequireReceiver);
				}
				return transform2;
			}
			return null;
		}

		public void SetTemplates(Transform[] newArray)
		{
			templates = newArray;
		}

		public void SetLength(int[] newPoolLenght)
		{
			poolLength = newPoolLenght;
		}

		public void SetLengthMax(int[] newPoolLengthMax)
		{
			poolLengthMax = newPoolLengthMax;
		}

		private void CalculateObjectsPerUpdate()
		{
			if (objectsPerUpdate == 0)
			{
				return;
			}
			if (objectsPerUpdate == -1)
			{
				objectsPerUpdate = 1;
				return;
			}
			objectsPerUpdate = (int)((float)objectsPerUpdate * (normalValue / Time.deltaTime));
			if (objectsPerUpdate == 0)
			{
				objectsPerUpdate = 1;
			}
		}

		private void DelayedSpawnInInstall()
		{
			if (optimizeSpawn)
			{
				CalculateObjectsPerUpdate();
			}
			int num = objectsPerUpdate;
			for (int i = 0; i < templates.Length; i++)
			{
				for (int j = 0; j < poolLength[i]; j++)
				{
					if (poolLengthCurrent[i] < poolLength[i])
					{
						InstantiateItem(templates[i], templatesParent[i], i);
						num--;
						if (num == 0)
						{
							break;
						}
					}
				}
				if (num == 0)
				{
					break;
				}
			}
			if (num != 0)
			{
				installing = false;
			}
		}

		private void NonDelayedSpawnInInstall()
		{
			for (int i = 0; i < templates.Length; i++)
			{
				for (int j = 0; j < poolLength[i]; j++)
				{
					if (poolLengthCurrent[i] < poolLength[i])
					{
						InstantiateItem(templates[i], templatesParent[i], i);
					}
				}
			}
		}

		private void Update()
		{
			if (installing)
			{
				if (!pooling)
				{
					installing = false;
					return;
				}
				if (delayedSpawnInInstall)
				{
					DelayedSpawnInInstall();
					return;
				}
				NonDelayedSpawnInInstall();
				installing = false;
			}
		}

		private Transform InstantiateItem(Transform temp, Transform par, int templatePosition)
		{
			Transform transform = UnityEngine.Object.Instantiate(temp, Vector3.zero, Quaternion.identity);
			transform.transform.SetParent(par);
			transform.gameObject.SetActive(value: false);
			readyObjects[temp].Add(transform);
			poolLengthCurrent[templatePosition]++;
			return transform;
		}

		public void Install()
		{
			poolLengthCurrent = new int[poolLength.Length];
			templatesParent = new Transform[templates.Length];
			for (int i = 0; i < templates.Length; i++)
			{
				if (!(templates[i] != null))
				{
					continue;
				}
				if (needParenting)
				{
					GameObject gameObject = new GameObject(templates[i].name);
					templatesParent[i] = gameObject.transform;
					gameObject.transform.SetParent(base.transform);
					List<Transform> list = new List<Transform>();
					if (list != null)
					{
						readyObjects.Add(templates[i], list);
					}
					occupiedObjects.Add(templates[i], new List<Transform>());
				}
				else
				{
					List<Transform> list2 = new List<Transform>();
					if (list2 != null)
					{
						readyObjects.Add(templates[i], list2);
					}
					occupiedObjects.Add(templates[i], new List<Transform>());
				}
			}
			if (targetFPS == 0)
			{
				targetFPS = 60;
			}
			normalValue = 1f / (float)targetFPS;
			installing = true;
		}
	}
}
