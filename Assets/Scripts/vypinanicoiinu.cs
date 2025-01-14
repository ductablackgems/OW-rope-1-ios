using UnityEngine;

public class vypinanicoiinu : MonoBehaviour
{
	public GameObject child;

	private GameObject player;

	public float hideDistance = 50f;

	public bool tank;

	private bool zasek;

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		InvokeRepeating("vypni", 0f, 2f);
		zasek = true;
	}

	public void vypni()
	{
		if (Vector3.Distance(base.transform.position, player.transform.position) > hideDistance)
		{
			child.SetActive(value: false);
		}
		else
		{
			child.SetActive(value: true);
			zasek = false;
		}
		if (tank && !zasek && Vector3.Distance(base.transform.position, player.transform.position) > hideDistance + 100f)
		{
			zasek = true;
		}
	}
}
