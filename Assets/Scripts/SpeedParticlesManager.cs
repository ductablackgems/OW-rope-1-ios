using App.Player;
using UnityEngine;

public class SpeedParticlesManager : MonoBehaviour
{
	private Rigidbody _rigidbody;

	public GameObject partiklyAir;

	private CharacterControl characterController;

	private FlyController flyController;

	private RopeController ropeController;

	private float targetMag = 12f;

	private void Awake()
	{
		_rigidbody = this.GetComponentSafe<Rigidbody>();
		characterController = this.GetComponentSafe<CharacterControl>();
		flyController = GetComponent<FlyController>();
		ropeController = GetComponent<RopeController>();
	}

	private void FixedUpdate()
	{
		bool runningFast = characterController.RunningFast;
		if (characterController.Grounded && !runningFast)
		{
			if (partiklyAir.activeSelf)
			{
				partiklyAir.SetActive(value: false);
			}
			return;
		}
		if (ropeController != null && ropeController.Running())
		{
			if (ropeController.pullToRopeSpeed > 35f)
			{
				partiklyAir.transform.parent.LookAt(ropeController.particleDir);
				partiklyAir.SetActive(value: true);
			}
			else
			{
				partiklyAir.SetActive(value: false);
			}
			return;
		}
		if (runningFast)
		{
			targetMag = 7f;
		}
		else if (flyController != null)
		{
			if (flyController.Running())
			{
				targetMag = 15f;
			}
			else
			{
				targetMag = 12f;
			}
		}
		else
		{
			targetMag = 12f;
		}
		if (_rigidbody.velocity.magnitude > targetMag)
		{
			partiklyAir.transform.parent.forward = _rigidbody.velocity;
			partiklyAir.SetActive(value: true);
		}
		else if (partiklyAir.activeSelf)
		{
			partiklyAir.SetActive(value: false);
		}
	}
}
