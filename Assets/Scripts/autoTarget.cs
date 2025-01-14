using UnityEngine;

public class autoTarget : MonoBehaviour
{
	public float speed;

	private float step;

	public Transform[] targets;

	private int count;

	private int next;

	private void Start()
	{
		base.transform.position = targets[0].position;
	}

	private void Update()
	{
		step = speed * Time.deltaTime;
		if (count == targets.Length - 1)
		{
			next = 0;
		}
		else
		{
			next = count + 1;
		}
		base.transform.position = Vector3.MoveTowards(base.transform.position, targets[next].position, step);
		if (base.transform.position == targets[next].position)
		{
			if (count == targets.Length - 1)
			{
				count = 0;
			}
			else
			{
				count++;
			}
		}
	}
}
