using UnityEngine;

namespace App.Player
{
	public class VehicleHandleSensor : MonoBehaviour
	{
		private TriggerMonitor handleTrigger = new TriggerMonitor();

		private TriggerMonitor targetHandleTrigger = new TriggerMonitor();

		private Transform targetHandle;

		public void SetTargetHandle(Transform targetHandle)
		{
			this.targetHandle = targetHandle;
			targetHandleTrigger.MarkTriggerExit();
		}

		public Transform GetTargetHandle()
		{
			return targetHandle;
		}

		public Component GetTrigger()
		{
			return handleTrigger.GetTrigger();
		}

		public Component GetTargetTrigger()
		{
			return targetHandleTrigger.GetTrigger();
		}

		public bool Triggered()
		{
			return handleTrigger.IsTriggered();
		}

		public bool TargetTriggered()
		{
			return targetHandleTrigger.IsTriggered();
		}

		private void FixedUpdate()
		{
			handleTrigger.FixedUpdate();
			targetHandleTrigger.FixedUpdate();
		}

		private void OnTriggerStay(Collider other)
		{
			if (other.CompareTag("HandleTrigger"))
			{
				handleTrigger.MarkTrigger(other);
			}
			if (other.transform == targetHandle)
			{
				targetHandleTrigger.MarkTrigger(other);
			}
		}
	}
}
