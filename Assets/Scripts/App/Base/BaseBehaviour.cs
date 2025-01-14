using UnityEngine;

namespace App.Base
{
	public abstract class BaseBehaviour : MonoBehaviour
	{
		public Vector3 Position
		{
			get
			{
				return base.transform.position;
			}
			set
			{
				base.transform.position = value;
			}
		}

		public Vector3 Forward => base.transform.forward;

		public void SetActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}
	}
}
