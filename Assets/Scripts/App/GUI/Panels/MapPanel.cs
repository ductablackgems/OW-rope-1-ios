using App.Util;
using UnityEngine;

namespace App.GUI.Panels
{
	public class MapPanel : AbstractPanel
	{
		public MoveMap moveMap;

		public UnityEngine.Camera bigMapView;

		public float mouseScrollSpeed = 50f;

		public override PanelType GetPanelType()
		{
			return PanelType.Map;
		}

		public void ZoomMap(float value)
		{
			bigMapView.orthographicSize += value;
		}

		private void OnEnable()
		{
			Time.timeScale = 0.0001f;
			moveMap.enabled = true;
		}

		private void OnDisable()
		{
			Time.timeScale = 1f;
			if (moveMap != null)
			{
				moveMap.enabled = false;
			}
		}

		private void Update()
		{
			float axis = InputUtils.GetAxis("Camera Zoom");
			ZoomMap(axis * mouseScrollSpeed);
		}
	}
}
