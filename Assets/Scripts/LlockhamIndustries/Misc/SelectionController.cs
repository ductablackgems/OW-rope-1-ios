using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
	[RequireComponent(typeof(Selector))]
	[RequireComponent(typeof(GenericCameraController))]
	public class SelectionController : MonoBehaviour
	{
		public LayerMask Layers;

		private Selector selector;

		private GenericCameraController controller;

		private void Awake()
		{
			selector = GetComponent<Selector>();
			controller = GetComponent<GenericCameraController>();
		}

		private void Update()
		{
			if (controller.Camera != null && Input.GetMouseButtonDown(1) && Physics.Raycast(controller.Camera.ScreenPointToRay(UnityEngine.Input.mousePosition), out RaycastHit hitInfo, float.PositiveInfinity, Layers.value))
			{
				CommandSelectables(hitInfo.point);
			}
		}

		private void CommandSelectables(Vector3 Point)
		{
			List<Selectable> selection = selector.Selection;
			if (selection != null)
			{
				foreach (Selectable item in selection)
				{
					Locomotion component = item.GetComponent<Locomotion>();
					if (component != null)
					{
						CommandUnit(component, Point);
					}
				}
			}
		}

		private void CommandUnit(Locomotion Unit, Vector3 Point)
		{
			Point.y = 0f;
			Vector3 position = Unit.transform.position;
			position.y = 0f;
			Vector3 vector2 = Unit.Direction = (Unit.Movement = (position - Point).normalized);
		}
	}
}
