using UnityEngine;
using UnityEngine.EventSystems;

public class UIWindow : MonoBehaviour, IDragHandler, IEventSystemHandler, IPointerDownHandler
{
	public void OnDrag(PointerEventData eventData)
	{
		base.transform.position += (Vector3)eventData.delta;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		base.transform.SetAsLastSibling();
	}
}
