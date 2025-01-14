using UnityEngine;

public class PinchMove2 : MonoBehaviour
{
	public float sensitivityX;

	public float sensitivityY;

	public float sensitivityZ;

	public Touch touchZero;

	public Touch touchOne;

	private void Update()
	{
		if (UnityEngine.Input.touchCount == 2 && UnityEngine.Input.touchCount < 3)
		{
			touchZero = UnityEngine.Input.GetTouch(0);
			touchOne = UnityEngine.Input.GetTouch(1);
			Vector2 a = touchZero.position - touchZero.deltaPosition;
			Vector2 b = touchOne.position - touchOne.deltaPosition;
			float magnitude = (a - b).magnitude;
			float magnitude2 = (touchZero.position - touchOne.position).magnitude;
			float num = magnitude - magnitude2;
			base.transform.localPosition += new Vector3(num * sensitivityX, num * sensitivityY, num * sensitivityZ);
		}
	}
}
