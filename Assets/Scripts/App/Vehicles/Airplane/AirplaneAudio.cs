using UnityEngine;

namespace App.Vehicles.Airplane
{
	public class AirplaneAudio : MonoBehaviour
	{
		[SerializeField]
		private AudioSource m_EngineAudio;

		[SerializeField]
		private float m_EngineMinThrottlePitch = 0.4f;

		[SerializeField]
		private float m_EngineMaxThrottlePitch = 2f;

		[SerializeField]
		private float m_EngineFwdSpeedMultiplier = 0.002f;

		[SerializeField]
		private AudioSource m_WindAudio;

		[SerializeField]
		private float m_WindBasePitch = 0.2f;

		[SerializeField]
		private float m_WindSpeedPitchFactor = 0.004f;

		[SerializeField]
		private float m_WindMaxSpeedVolume = 100f;

		[SerializeField]
		private AudioSource m_CrashAudio;

		private IAirplaneController m_Controller;

		private Rigidbody m_Rigidbody;

		private float m_EngineInitialVolume;

		private float m_WindInitialVolume;

		private bool m_IsRunning;

		public void PlayCrashSound()
		{
			if (!m_CrashAudio.isPlaying)
			{
				m_CrashAudio.Play();
			}
		}

		private void Awake()
		{
			m_Controller = GetComponent<IAirplaneController>();
			m_Rigidbody = GetComponent<Rigidbody>();
			m_EngineInitialVolume = m_EngineAudio.volume;
			m_WindInitialVolume = m_WindAudio.volume;
		}

		private void Update()
		{
			if (SetIsRunning(m_Controller.IsActive))
			{
				float t = Mathf.InverseLerp(0f, m_Controller.MaxEnginePower, m_Controller.EnginePower);
				m_EngineAudio.pitch = Mathf.Lerp(m_EngineMinThrottlePitch, m_EngineMaxThrottlePitch, t);
				m_EngineAudio.pitch += m_Controller.ForwardSpeed * m_EngineFwdSpeedMultiplier;
				m_EngineAudio.volume = Mathf.InverseLerp(0f, m_Controller.MaxEnginePower * m_EngineInitialVolume, m_Controller.EnginePower);
				float magnitude = m_Rigidbody.velocity.magnitude;
				m_WindAudio.pitch = m_WindBasePitch + magnitude * m_WindSpeedPitchFactor;
				m_WindAudio.volume = Mathf.InverseLerp(0f, m_WindMaxSpeedVolume, magnitude) * m_WindInitialVolume;
			}
		}

		private bool SetIsRunning(bool isRunning)
		{
			if (m_IsRunning == isRunning)
			{
				return m_IsRunning;
			}
			if (isRunning)
			{
				m_EngineAudio.Play();
				m_WindAudio.Play();
			}
			else
			{
				m_EngineAudio.Stop();
				m_WindAudio.Stop();
			}
			m_IsRunning = isRunning;
			return m_IsRunning;
		}
	}
}
