using UnityEngine;

namespace App
{
	public class TriggerMonitor
	{
		private Component trigger;

		private Component markedTrigger;

		public void MarkTrigger(Component trigger)
		{
			this.trigger = trigger;
			markedTrigger = trigger;
		}

		public void MarkTriggerExit()
		{
			trigger = null;
			markedTrigger = null;
		}

		public Component GetTrigger()
		{
			return trigger;
		}

		public Component GetMarketTrigger()
		{
			return markedTrigger;
		}

		public bool IsTriggered()
		{
			return trigger != null;
		}

		public void FixedUpdate()
		{
			if (markedTrigger == null)
			{
				trigger = null;
			}
			markedTrigger = null;
		}
	}
}
