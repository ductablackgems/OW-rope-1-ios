using UnityEngine;

public class TouchRotate : MonoBehaviour
{
	public float sensitivityX;

	public float sensitivityY;

	public float sensitivityZ;

	public Touch touchZero;

	private void Update()
	{
		if (UnityEngine.Input.touchCount == 1 && UnityEngine.Input.touchCount < 2)
		{
			touchZero = UnityEngine.Input.GetTouch(0);
			float magnitude = (touchZero.position - touchZero.deltaPosition).magnitude;
			base.transform.localEulerAngles += new Vector3(magnitude * sensitivityX, magnitude * sensitivityY, magnitude * sensitivityZ);
		}
	}
}
