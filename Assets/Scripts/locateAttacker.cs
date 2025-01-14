using UnityEngine;

public class locateAttacker : MonoBehaviour
{
	public GameObject attacker;

	private ParticleSystem ps;

	private ParticleSystem.MainModule main;

	private ParticleSystem.ShapeModule shape;

	private Transform master;

	private Transform attackerpos;

	private float dist;

	private void OnEnable()
	{
		master = base.gameObject.transform.GetChild(0).GetChild(0);
		master.localEulerAngles = new Vector3(0f, -90f, 0f);
		ps = master.GetComponent<ParticleSystem>();
		main = ps.main;
		shape = ps.shape;
		attackerpos = attacker.GetComponent<Transform>();
	}

	private void Update()
	{
		base.transform.LookAt(attackerpos);
		dist = Vector3.Distance(attackerpos.position, master.position);
		shape.radius = dist;
		main.startSpeedMultiplier = (0f - dist) / main.startLifetimeMultiplier - dist * 0.05f;
	}
}
