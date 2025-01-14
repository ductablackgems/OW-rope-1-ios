using System.Collections.Generic;
using UnityEngine;

namespace App.Spawn.Pooling
{
	public class Poolable : MonoBehaviour
	{
		public float autoPushTime = -1f;

		private Stack<GameObject> poolables;

		private Transform inactiveParent;

		private DurationTimer autoPushTimer = new DurationTimer();

		private Resetor resetor;

		private bool resetorSearched;

		public bool IsNew
		{
			get;
			set;
		}

		public Resetor GetResetor()
		{
			if (!resetorSearched)
			{
				resetor = GetComponent<Resetor>();
				resetorSearched = true;
			}
			return resetor;
		}

		public void Register(Stack<GameObject> poolables, Transform inactiveParent)
		{
			this.poolables = poolables;
			this.inactiveParent = inactiveParent;
		}

		public void Push()
		{
			poolables.Push(base.gameObject);
			base.transform.parent = inactiveParent;
		}

		public void UpdatePushTime(float pushTime)
		{
			autoPushTime = pushTime;
			if (autoPushTime > 0f)
			{
				autoPushTimer.Run(autoPushTime);
			}
		}

		private void OnEnable()
		{
			if (autoPushTime > 0f)
			{
				autoPushTimer.Run(autoPushTime);
			}
		}

		private void Update()
		{
			if (autoPushTimer.Done())
			{
				autoPushTimer.Stop();
				Push();
			}
		}
	}
}
