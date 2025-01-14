using UnityEngine;

namespace App.Abilities
{
	public class Ability : MonoBehaviour
	{
		public float Radius;

		public float Duration;

		public float CastTime;

		public bool CanMove = true;

		[Range(0f, 1f)]
		public float MinEnergyRequired;

		[Range(0f, 1f)]
		public float EnergyDrain;

		[Header("Animations")]
		[SerializeField]
		private int m_AnimParam;

		[Header("Sounds")]
		public AudioClip ActivationSound;

		private DurationTimer timer = new DurationTimer();

		private DurationTimer castTimer = new DurationTimer();

		private AudioSource audioSource;

		public GameObject Owner
		{
			get;
			private set;
		}

		public bool IsRunning
		{
			get;
			private set;
		}

		public int AnimParam
		{
			get;
			private set;
		}

		protected virtual void OnInitialized()
		{
		}

		protected virtual void OnActivated(Vector3 position)
		{
		}

		protected virtual void OnDeactivated()
		{
		}

		protected virtual bool OnCanActivate()
		{
			return true;
		}

		public void Initialize(GameObject owner)
		{
			Owner = owner;
			audioSource = GetComponent<AudioSource>();
			base.transform.localPosition = Vector3.zero;
			base.gameObject.SetActive(value: false);
			OnInitialized();
		}

		public void Activate(Vector3 position = default(Vector3))
		{
			if (!IsRunning && OnCanActivate())
			{
				base.gameObject.SetActive(value: true);
				if (Duration > 0f)
				{
					timer.Run(Duration);
				}
				if (CastTime > 0f)
				{
					castTimer.Run(CastTime);
				}
				AnimParam = m_AnimParam;
				IsRunning = true;
				OnActivated(position);
				PlaySound(ActivationSound);
			}
		}

		public void Deactivate()
		{
			if (IsRunning)
			{
				base.gameObject.SetActive(value: false);
				AnimParam = 0;
				IsRunning = false;
				OnDeactivated();
			}
		}

		private void Update()
		{
			if (IsRunning)
			{
				if (timer.Done())
				{
					timer.Stop();
					Deactivate();
				}
				if (castTimer.Done())
				{
					castTimer.Stop();
					AnimParam = 0;
				}
			}
		}

		protected void ResetDuration()
		{
			if (!(Duration <= 0f))
			{
				timer.Run(Duration);
			}
		}

		private void PlaySound(AudioClip clip)
		{
			if (!(audioSource == null))
			{
				audioSource.PlayOneShot(clip);
			}
		}
	}
}
