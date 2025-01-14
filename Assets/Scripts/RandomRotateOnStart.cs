using UnityEngine;

public class RandomRotateOnStart : MonoBehaviour
{
	public Vector3 NormalizedRotateVector = new Vector3(0f, 1f, 0f);

	private Transform t;

	private bool isInitialized;

	private void Start()
	{
		t = base.transform;
		t.Rotate(NormalizedRotateVector * UnityEngine.Random.Range(0, 360));
		isInitialized = true;
	}

	private void OnEnable()
	{
		if (isInitialized)
		{
			t.Rotate(NormalizedRotateVector * UnityEngine.Random.Range(0, 360));
		}
	}
}
