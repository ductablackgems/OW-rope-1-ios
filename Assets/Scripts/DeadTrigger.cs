using App.Util;
using UnityEngine;

public class DeadTrigger : MonoBehaviour
{
	public bool destroy;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			other.GetComponent<Health>().Kill();
		}
		else if (destroy)
		{
			UnityEngine.Object.Destroy(other.gameObject);
		}
	}
}
