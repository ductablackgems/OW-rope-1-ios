using UnityEngine;

namespace App.Vehicles.Mech
{
	public class MechCabinRotator : MonoBehaviour
	{
		public enum ControlType
		{
			Stay,
			Home,
			PlayerControl,
			Target
		}

		public float horizontalSpeed = 100f;

		public float maxAngle = 90f;

		public Transform localVirtualTarget;

		private Transform virtualTarget;

		private Vector3 initialEuler;

		private AudioSource audioSource;

		private float soundTimer;

		public ControlType Type
		{
			get;
			set;
		}

		public void RotateTo(Vector3 position)
		{
			if (Type == ControlType.Target)
			{
				localVirtualTarget.position = position;
				Vector3 direction = localVirtualTarget.localPosition - base.transform.localPosition;
				RotateToDiretion(direction);
			}
		}

		private void Start()
		{
			initialEuler = base.transform.localRotation.eulerAngles;
			virtualTarget = ServiceLocator.GetGameObject("VirtualTarget").transform;
			audioSource = GetComponent<AudioSource>();
		}

		private void FixedUpdate()
		{
			if (Type == ControlType.Stay)
			{
				StopSound();
			}
			else if (Type == ControlType.Home)
			{
				StopSound();
				base.transform.localRotation = Quaternion.RotateTowards(base.transform.localRotation, Quaternion.Euler(initialEuler), Time.deltaTime * horizontalSpeed);
			}
			else if (Type == ControlType.PlayerControl)
			{
				localVirtualTarget.position = virtualTarget.position;
				Vector3 direction = localVirtualTarget.localPosition - base.transform.localPosition;
				RotateToDiretion(direction);
			}
		}

		private void RotateToDiretion(Vector3 direction)
		{
			Vector3 eulerAngles = Quaternion.LookRotation(direction).eulerAngles;
			float value = (eulerAngles.y > 180f) ? (eulerAngles.y - 360f) : eulerAngles.y;
			value = Mathf.Clamp(value, 0f - maxAngle, maxAngle);
			Quaternion localRotation = base.transform.localRotation;
			Quaternion b = Quaternion.Euler(initialEuler.x, value, initialEuler.z);
			base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, b, Time.deltaTime * horizontalSpeed);
			if (localRotation != base.transform.localRotation)
			{
				soundTimer = 0.2f;
			}
			soundTimer = Mathf.Max(soundTimer - Time.deltaTime, 0f);
			ActivateSound(soundTimer > 0f);
		}

		private void ActivateSound(bool isActive)
		{
			if (!(audioSource == null))
			{
				if (!isActive)
				{
					audioSource.Stop();
				}
				else if (!audioSource.isPlaying)
				{
					audioSource.Play();
				}
			}
		}

		private void StopSound()
		{
			if (!(audioSource == null) && audioSource.isPlaying)
			{
				audioSource.Stop();
			}
		}
	}
}
