using App.GUI;
using UnityEngine;

public class MoveMap : MonoBehaviour
{
	public Camera bigMapView;

	public MiniMap miniMap;

	private Vector3 offset;

	private Vector3 currentStartPosition;

	private Vector3 curScreenPoint;

	public static bool zoomOn;

	public float speedMove = 25;

	private void Start()
	{
		currentStartPosition = bigMapView.transform.position;
	}

	private void OnEnable()
	{
		bigMapView.transform.position = new Vector3(miniMap.PlayerIconTransform.position.x, currentStartPosition.y, miniMap.PlayerIconTransform.position.z);
	}

	private void OnMouseDown()
	{
		offset = bigMapView.transform.position;
		curScreenPoint = Vector3.zero;
	}

	private void OnMouseDrag()
	{
		if (!zoomOn)
		{
			curScreenPoint += new Vector3(0f - UnityEngine.Input.GetAxis("Mouse X"), 0f, 0f - UnityEngine.Input.GetAxis("Mouse Y")) * 5;
			Vector3 position = offset + curScreenPoint;
			bigMapView.transform.position = position;
		}
	}
}
