using App.Player;
using System.Collections.Generic;
using UnityEngine;

namespace App.Interaction
{
	public class InteractiveObjectSensor
	{
		private const float SCAN_INTERVAL = 0.33f;

		private const float SCAN_DISTANCE = 3f;

		private float scanTimer;

		private bool isInitialized;

		private PlayerModel player;

		private List<InteractiveObject> list = new List<InteractiveObject>(4);

		public InteractiveObject InteractiveObject
		{
			get;
			private set;
		}

		public void Initialize()
		{
			player = ServiceLocator.GetPlayerModel();
			GameObject[] gameObjects = ServiceLocator.GetGameObjects("Interaction");
			for (int i = 0; i < gameObjects.Length; i++)
			{
				InteractiveObject component = gameObjects[i].GetComponent<InteractiveObject>();
				if (!(component == null))
				{
					list.Add(component);
				}
			}
			isInitialized = true;
		}

		public void Update()
		{
			if (isInitialized)
			{
				scanTimer -= Time.deltaTime;
				if (!(scanTimer > 0f))
				{
					scanTimer = 0.33f;
					Scan();
				}
			}
		}

		public void Register(InteractiveObject interactiveObject)
		{
			list.Add(interactiveObject);
		}

		public void Unregister(InteractiveObject interactiveObject)
		{
			list.Remove(interactiveObject);
		}

		private void Scan()
		{
			InteractiveObject = null;
			float num = float.MaxValue;
			for (int i = 0; i < list.Count; i++)
			{
				InteractiveObject interactiveObject = list[i];
				if (!(interactiveObject == null))
				{
					float num2 = Vector3.Distance(player.Transform.position, interactiveObject.transform.position);
					float num3 = (interactiveObject.InteractionDistance > 0f) ? interactiveObject.InteractionDistance : 3f;
					if (!(num2 > num3) && !(num2 > num))
					{
						InteractiveObject = interactiveObject;
						num = num2;
					}
				}
			}
		}
	}
}
