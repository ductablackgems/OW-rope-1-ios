using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class particleCollisionCount : MonoBehaviour
{
	public ParticleSystem ps;

	public GameObject target;

	public List<ParticleCollisionEvent> collisionEvents;

	public Text display;

	private int hit;

	private void Update()
	{
		collisionEvents = new List<ParticleCollisionEvent>();
		hit += ps.GetCollisionEvents(target, collisionEvents);
		display.text = hit.ToString() + " HITS!";
	}
}
