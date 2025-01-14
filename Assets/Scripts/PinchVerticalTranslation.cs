using UnityEngine;

public class PinchVerticalTranslation : MonoBehaviour
{
	public float sensitivity = 0.5f;

	public Touch touchZero;

	public Touch touchOne;

	public Touch touchTwo;

	private void Update()
	{
		if (UnityEngine.Input.touchCount == 3)
		{
			touchZero = UnityEngine.Input.GetTouch(0);
			touchOne = UnityEngine.Input.GetTouch(1);
			touchTwo = UnityEngine.Input.GetTouch(2);
			Vector2 a = touchZero.position - touchZero.deltaPosition;
			Vector2 b = touchTwo.position - touchTwo.deltaPosition;
			float magnitude = (a - b).magnitude;
			float magnitude2 = (touchZero.position - touchTwo.position).magnitude;
			float num = magnitude - magnitude2;
			base.transform.position += new Vector3(0f, Mathf.Clamp(num * sensitivity, 1.2f, 8f), 0f);
		}
	}
}
