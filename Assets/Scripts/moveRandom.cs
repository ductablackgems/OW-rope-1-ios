using UnityEngine;

public class moveRandom : MonoBehaviour
{
	public float speed = 1f;

	public float timeUpdate = 2f;

	private Rigidbody rb;

	private float step;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void LateUpdate()
	{
		step += Time.deltaTime;
		if (step > timeUpdate * Time.deltaTime)
		{
			rb.velocity = new Vector3(UnityEngine.Random.Range(0f - speed, speed), UnityEngine.Random.Range(0f - speed, speed), UnityEngine.Random.Range(0f - speed, speed));
			step = 0f;
		}
	}

	public void OnDisable()
	{
		rb.velocity = Vector3.zero;
	}
}
