using UnityEngine;
using UnityEngine.EventSystems;

namespace App.Util
{
	public class EasyButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		public float maxPressedDuration = 0.2f;

		private bool pressed;

		private DurationTimer pressedTimer = new DurationTimer();

		public bool Pressed
		{
			get
			{
				if (pressed)
				{
					return pressedTimer.InProgress();
				}
				return false;
			}
		}

		public bool RealPressed => pressed;

		public void OnPointerDown(PointerEventData data)
		{
			pressed = true;
			pressedTimer.Run(maxPressedDuration);
		}

		public void OnPointerUp(PointerEventData data)
		{
			pressed = false;
			pressedTimer.Stop();
		}
	}
}
