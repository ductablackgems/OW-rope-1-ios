using System;
using UnityEngine;

namespace App.Util
{
	public class CollectableItem : MonoBehaviour
	{
		[Tooltip("0 = item wont be destroyed")]
		[SerializeField]
		private float autoDestroyTime = 30f;

		[Space]
		[LocalizeID]
		[SerializeField]
		private int nameID;

		private DurationTimer m_Timer = new DurationTimer();

		public int NameID => nameID;

		public event Action<CollectableItem> Collected;

		public void Collect()
		{
			if (this.Collected != null)
			{
				this.Collected(this);
			}
		}

		private void OnEnable()
		{
			if (!(autoDestroyTime <= 0f) && !m_Timer.Running())
			{
				m_Timer.Run(autoDestroyTime);
			}
		}

		private void Update()
		{
			if (!(autoDestroyTime <= 0f) && m_Timer.Done())
			{
				base.gameObject.SetActive(value: false);
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
