using App.GUI.Panels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace App.GUI
{
	public class ZoomMapButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		public bool positive;

		private MapPanel mapPanel;

		private bool ButtonOn;

		private void Awake()
		{
			mapPanel = ServiceLocator.Get<MapPanel>();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			ButtonOn = true;
			MoveMap.zoomOn = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			ButtonOn = false;
			MoveMap.zoomOn = false;
		}

		private void OnEnable()
		{
			MoveMap.zoomOn = false;
			ButtonOn = false;
		}

		private void OnDisable()
		{
			MoveMap.zoomOn = false;
			ButtonOn = false;
		}

		private void Update()
		{
			if (!ButtonOn)
			{
				return;
			}
			if (positive)
			{
				if (mapPanel.bigMapView.orthographicSize < -80f)
				{
					mapPanel.ZoomMap(50f * Time.deltaTime * 5000f);
				}
				else
				{
					ButtonOn = false;
				}
			}
			else if (mapPanel.bigMapView.orthographicSize > -1000f)
			{
				mapPanel.ZoomMap(-50f * Time.deltaTime * 5000f);
			}
			else
			{
				ButtonOn = false;
				MoveMap.zoomOn = false;
			}
		}
	}
}
