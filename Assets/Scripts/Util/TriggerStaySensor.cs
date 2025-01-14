using UnityEngine;

namespace Util
{
	public class TriggerStaySensor : MonoBehaviour
	{
		public delegate void TriggerStayEventHandler(Collider other);

		public event TriggerStayEventHandler _OnTriggerStay;

		private void OnTriggerStay(Collider other)
		{
			if (this._OnTriggerStay != null)
			{
				this._OnTriggerStay(other);
			}
		}
	}
}
