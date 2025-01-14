using UnityEngine;

public class WaypointRider : MonoBehaviour
{
	private int _targetWaypoint;

	private Transform _waypoints;

	public float movementSpeed = 3f;

	private void Start()
	{
		_waypoints = GameObject.Find("Waypoints").transform;
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		handleWalkWaypoints();
	}

	private void handleWalkWaypoints()
	{
		Vector3 value = _waypoints.GetChild(_targetWaypoint).position - base.transform.position;
		Vector3 vector = Vector3.Normalize(value);
		if ((double)value.magnitude < 0.1)
		{
			if (_targetWaypoint + 1 >= _waypoints.childCount)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			_targetWaypoint++;
		}
		else
		{
			base.transform.position += vector * movementSpeed * Time.fixedDeltaTime;
		}
		Quaternion b = Quaternion.LookRotation(vector);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, 0.5f);
	}
}
