using UnityEngine;

namespace App.Shop.Slider
{
	public class UILayerTester : MonoBehaviour
	{
		private Transform uiRootTransform;

		public bool CollideWithUI(Touch touch)
		{
			Ray ray = new Ray(CalculateRayPosition(touch), Vector3.forward);
			int layerMask = 32;
			return Physics.Raycast(ray, 1f, layerMask);
		}

		protected void Awake()
		{
			uiRootTransform = ServiceLocator.Get<UIRoot>().transform;
		}

		private Vector3 CalculateRayPosition(Touch touch)
		{
			return uiRootTransform.position + new Vector3((touch.position.x - (float)(Screen.width / 2)) * uiRootTransform.localScale.x, (touch.position.y - (float)(Screen.height / 2)) * uiRootTransform.localScale.y, -0.5f);
		}
	}
}
