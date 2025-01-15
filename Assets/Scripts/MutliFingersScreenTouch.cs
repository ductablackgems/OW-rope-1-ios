using HedgehogTeam.EasyTouch;
using UnityEngine;

public class MutliFingersScreenTouch : MonoBehaviour
{
	public GameObject touchGameObject;

	private void OnEnable()
	{
		EasyTouch.On_TouchStart += On_TouchStart;
	}

	private void OnDestroy()
	{
		EasyTouch.On_TouchStart -= On_TouchStart;
	}

	private void On_TouchStart(Gesture gesture)
	{
		if (gesture.pickedObject == null)
		{
			Vector3 touchToWorldPoint = gesture.GetTouchToWorldPoint(5f);
			Object.Instantiate(touchGameObject, touchToWorldPoint, Quaternion.identity).GetComponent<FingerTouch>().InitTouch(gesture.fingerIndex);
		}
	}
}
