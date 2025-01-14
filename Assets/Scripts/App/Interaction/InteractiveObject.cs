using System;
using UnityEngine;

namespace App.Interaction
{
	public class InteractiveObject : MonoBehaviour
	{
		[SerializeField]
		private float interactionDistance = 3f;

		protected bool isInitialized;

		public float InteractionDistance => interactionDistance;

		public event Action Interaction;

		protected virtual void OnInitialized()
		{
		}

		protected virtual void OnInteract()
		{
		}

		protected virtual void OnUpdate()
		{
		}

		public void Initialize()
		{
			if (!isInitialized)
			{
				OnInitialized();
				isInitialized = true;
			}
		}

		public void Interact()
		{
			OnInteract();
			if (this.Interaction != null)
			{
				this.Interaction();
			}
		}

		private void Update()
		{
			OnUpdate();
		}

		private void OnDestroy()
		{
			this.Interaction = null;
		}
	}
}
