using UnityEngine;

namespace App.Vehicles.Mech
{
	public class JetControl : MonoBehaviour
	{
		private AudioSource audioSource;

		private bool isActive;

		public bool IsActive
		{
			get
			{
				return isActive;
			}
			set
			{
				SetIsActivet(value);
			}
		}

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
			base.gameObject.SetActive(value: false);
		}

		private void SetIsActivet(bool isActive)
		{
			this.isActive = isActive;
			base.gameObject.SetActive(isActive);
			if (isActive)
			{
				audioSource.Play();
			}
			else
			{
				audioSource.Stop();
			}
		}
	}
}
