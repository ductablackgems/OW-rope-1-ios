using UnityEngine;

public class TowerRadar : MonoBehaviour
{
	private float angle;

	private void Start()
	{
		angle = 0f;
	}

	private void Update()
	{
		base.gameObject.transform.localEulerAngles = new Vector3(0f, angle, 0f);
		angle += Time.deltaTime * 93f;
		if (angle >= 360f)
		{
			angle -= 360f;
		}
	}
}
