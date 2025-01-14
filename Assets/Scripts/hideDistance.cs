using System.Collections.Generic;
using UnityEngine;

public class hideDistance : MonoBehaviour
{
	public int maxDistance = 50;

	private Transform player;

	private GameObject child;

	private int interval = 2;

	private float nextTime;

	private float playerDistance;

	private bool stop = true;

	private Behaviour[] comps;

	private Transform[] childs;

	private int optimalNum;

	private Dictionary<object, bool> componentsEnabled = new Dictionary<object, bool>();

	private bool visible = true;

	private void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		optimalNum = PlayerPrefs.GetInt("Quality");
		if (optimalNum == 2)
		{
			maxDistance = 70;
		}
		else if (optimalNum == 1)
		{
			maxDistance = 45;
		}
		else if (optimalNum == 0)
		{
			maxDistance = 30;
		}
		else if (optimalNum == 3)
		{
			maxDistance = 80;
		}
		comps = GetComponents<Behaviour>();
		childs = GetComponentsInChildren<Transform>();
	}

	private void OnEnable()
	{
		stop = false;
		nextTime = Time.time + (float)interval;
	}

	private void OnDisable()
	{
		stop = true;
		visible = true;
		ApplyComponentStates();
		Transform[] array = childs;
		foreach (Transform transform in array)
		{
			if (IsValidObject(transform.gameObject))
			{
				transform.gameObject.SetActive(value: true);
				if ((bool)transform.GetComponent<Rigidbody>())
				{
					break;
				}
			}
		}
	}

	private void Update()
	{
		if (!stop && Time.time >= nextTime)
		{
			CheckRenderer();
			nextTime += interval;
		}
	}

	public void ResetStats()
	{
		visible = true;
		stop = true;
		ApplyComponentStates();
		Transform[] array = childs;
		foreach (Transform transform in array)
		{
			if (IsValidObject(transform.gameObject))
			{
				transform.gameObject.SetActive(value: true);
				if ((bool)transform.GetComponent<Rigidbody>())
				{
					return;
				}
			}
		}
		componentsEnabled.Clear();
		nextTime = Time.time + (float)interval;
	}

	private void CheckRenderer()
	{
		if (base.transform == base.transform.root)
		{
			stop = true;
			return;
		}
		playerDistance = Vector3.Distance(base.transform.position, player.position);
		if (playerDistance > (float)maxDistance)
		{
			if (!visible)
			{
				return;
			}
			visible = false;
			CopyComponentStates();
			DisableAllComponents();
			Transform[] array = childs;
			foreach (Transform transform in array)
			{
				if (IsValidObject(transform.gameObject))
				{
					transform.gameObject.SetActive(value: false);
					if ((bool)transform.GetComponent<Rigidbody>())
					{
						break;
					}
				}
			}
		}
		else
		{
			if (visible)
			{
				return;
			}
			visible = true;
			ApplyComponentStates();
			Transform[] array = childs;
			foreach (Transform transform2 in array)
			{
				if (IsValidObject(transform2.gameObject))
				{
					transform2.gameObject.SetActive(value: true);
					if ((bool)transform2.GetComponent<Rigidbody>())
					{
						break;
					}
				}
			}
		}
	}

	private void CopyComponentStates()
	{
		Behaviour[] array = comps;
		foreach (Behaviour behaviour in array)
		{
			componentsEnabled[behaviour] = behaviour.enabled;
		}
	}

	private void DisableAllComponents()
	{
		Behaviour[] array = comps;
		foreach (Behaviour behaviour in array)
		{
			if (behaviour != this)
			{
				behaviour.enabled = false;
			}
		}
	}

	private void ApplyComponentStates()
	{
		Behaviour[] array = comps;
		foreach (Behaviour behaviour in array)
		{
			if (componentsEnabled.TryGetValue(behaviour, out bool value) && value)
			{
				behaviour.enabled = value;
			}
		}
	}

	private static bool IsValidObject(GameObject obj)
	{
		if (obj.tag != "Enemy")
		{
			return obj.tag != "Interaction";
		}
		return false;
	}
}
