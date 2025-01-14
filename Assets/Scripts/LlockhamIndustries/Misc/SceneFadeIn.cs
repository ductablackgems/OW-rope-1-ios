using LlockhamIndustries.Decals;
using System.Collections;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
	[RequireComponent(typeof(ProjectionRenderer))]
	public class SceneFadeIn : MonoBehaviour
	{
		public float holdTime;

		public float inTime;

		private ProjectionRenderer projectionRenderer;

		private void OnEnable()
		{
			projectionRenderer = GetComponent<ProjectionRenderer>();
			projectionRenderer.enabled = true;
			StartCoroutine(FadeIn());
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}

		private IEnumerator FadeIn()
		{
			float timeElapsed = 0f;
			Color color = projectionRenderer.Properties[0].color;
			while (timeElapsed < holdTime + inTime)
			{
				timeElapsed += Time.deltaTime;
				float t = Mathf.Pow(Mathf.Clamp01(1f - (timeElapsed - holdTime) / inTime), 0.6f);
				projectionRenderer.SetColor(0, Color.Lerp(Color.white, color, t));
				projectionRenderer.UpdateProperties();
				yield return new WaitForEndOfFrame();
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
