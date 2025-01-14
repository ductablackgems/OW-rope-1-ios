using App.Vehicles.Airplane;
using UnityEngine;

namespace App.Player
{
	public class AirplanePropellerAnimator : MonoBehaviour
	{
		[SerializeField]
		private Transform m_PropellorModel;

		[SerializeField]
		private Transform m_PropellorBlur;

		[SerializeField]
		private Texture2D[] m_PropellorBlurTextures;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_ThrottleBlurStart = 0.25f;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_ThrottleBlurEnd = 0.5f;

		[SerializeField]
		private float m_MaxRpm = 2000f;

		[SerializeField]
		private Vector3 m_RotateAxis = new Vector3(0f, 0f, 1f);

		private AirplaneController m_Plane;

		private int m_PropellorBlurState = -1;

		private const float k_RpmToDps = 60f;

		private Renderer m_PropellorModelRenderer;

		private Renderer m_PropellorBlurRenderer;

		private void Awake()
		{
			m_Plane = GetComponent<AirplaneController>();
			m_PropellorModelRenderer = m_PropellorModel.GetComponent<Renderer>();
			m_PropellorBlurRenderer = m_PropellorBlur.GetComponent<Renderer>();
			m_PropellorBlur.parent = m_PropellorModel;
			SetActiveModel(isActivte: true);
		}

		private void Update()
		{
			if (!m_Plane.IsActive)
			{
				SetActiveModel(isActivte: true);
				return;
			}
			Vector3 eulers = m_RotateAxis * m_MaxRpm * m_Plane.Throttle * Time.deltaTime * 60f;
			m_PropellorModel.Rotate(eulers);
			int num = 0;
			if (m_Plane.Throttle > m_ThrottleBlurStart)
			{
				num = Mathf.FloorToInt(Mathf.InverseLerp(m_ThrottleBlurStart, m_ThrottleBlurEnd, m_Plane.Throttle) * (float)(m_PropellorBlurTextures.Length - 1));
			}
			if (num == m_PropellorBlurState)
			{
				return;
			}
			m_PropellorBlurState = num;
			if (m_PropellorBlurState == 0)
			{
				SetActiveModel(isActivte: true);
				return;
			}
			SetActiveModel(isActivte: false);
			if (m_PropellorBlurTextures.Length != 0)
			{
				m_PropellorBlurRenderer.material.mainTexture = m_PropellorBlurTextures[m_PropellorBlurState];
			}
		}

		private void SetActiveModel(bool isActivte)
		{
			m_PropellorModelRenderer.enabled = isActivte;
			m_PropellorBlurRenderer.enabled = !isActivte;
		}
	}
}
