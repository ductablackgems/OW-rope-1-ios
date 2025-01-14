using UnityEngine;

public class rotateButton : MonoBehaviour
{
	public Transform objectTransform;

	public Vector3 rotation;

	public void onButton()
	{
		objectTransform.localEulerAngles += rotation;
	}
}
