using UnityEngine;
using UnityEngine.UI;

namespace App.GUI
{
	public class HitIndicator : MonoBehaviour
	{
		private Image image;

		private bool isFadeIn;

		private Color minColor;

		public bool IsReady => GetIsReady();

		public Transform Target
		{
			get;
			private set;
		}

		public void Show(Transform target)
		{
			Target = target;
			isFadeIn = true;
			base.gameObject.SetActive(value: true);
		}

		private void Awake()
		{
			image = GetComponent<Image>();
			minColor = image.color;
			minColor.a = 0f;
			image.color = minColor;
		}

		private void Update()
		{
			float deltaTime = Time.deltaTime;
			if (isFadeIn)
			{
				isFadeIn = Fade(3f, deltaTime);
			}
			else if (!Fade(-0.5f, deltaTime))
			{
				Target = null;
				base.gameObject.SetActive(value: false);
			}
		}

		private bool GetIsReady()
		{
			return !base.gameObject.activeSelf;
		}

		private bool Fade(float speed, float deltaTime)
		{
			Color color = image.color;
			float num = (speed < 0f) ? 0f : 1f;
			if (color.a == num)
			{
				return false;
			}
			color.a = Mathf.Clamp01(color.a + deltaTime * speed);
			image.color = color;
			return true;
		}
	}
}
