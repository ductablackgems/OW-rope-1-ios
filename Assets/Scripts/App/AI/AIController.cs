using System;
using UnityEngine;

namespace App.AI
{
	public abstract class AIController : MonoBehaviour
	{
		public event Action<AIController> ControllerDestroy;

		protected virtual void OnInitialize()
		{
		}

		protected virtual void OnSpawn(Vector3 position)
		{
		}

		protected virtual void OnDespawn()
		{
		}

		protected virtual void OnRespawn()
		{
		}

		protected virtual void OnUpdate()
		{
		}

		public void Initialize()
		{
			OnInitialize();
		}

		public void Spawn(Vector3 position)
		{
			OnSpawn(position);
		}

		public void Despawn()
		{
			OnDespawn();
		}

		public void Respawn()
		{
			OnRespawn();
		}

		private void Update()
		{
			OnUpdate();
		}

		private void OnDestroy()
		{
			if (this.ControllerDestroy != null)
			{
				this.ControllerDestroy(this);
				this.ControllerDestroy = null;
			}
		}
	}
}
