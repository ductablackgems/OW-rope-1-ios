using App.Util;
using UnityEngine;

namespace App.Vehicles.Car.GarbageTruck
{
	public class DumpsterController : MonoBehaviour
	{
		public GameObject FreshDumpster;

		public GameObject Wreck;

		private Health playerHealth;

		private Vector3 defaultPosition;

		private Quaternion defaultRotation;

		private Rigidbody rigidbody;

		private DurationTimer TrackDestroyTimer = new DurationTimer();

		private ContainerParameters containerParameters;

		private void Awake()
		{
			defaultPosition = FreshDumpster.transform.position;
			defaultRotation = FreshDumpster.transform.rotation;
			playerHealth = ServiceLocator.GetGameObject("Player").GetComponentSafe<Health>();
			rigidbody = Wreck.GetComponent<Rigidbody>();
			containerParameters = FreshDumpster.GetComponent<ContainerParameters>();
			OnReviveDumpster();
		}

		private void Update()
		{
			if (Wreck.activeSelf)
			{
				if (!playerHealth)
				{
					playerHealth = ServiceLocator.GetGameObject("Player").GetComponentSafe<Health>();
				}
				if ((playerHealth.transform.position - base.transform.position).magnitude > 50f && TrackDestroyTimer.Done())
				{
					TrackDestroyTimer.Stop();
					OnReviveDumpster();
				}
			}
			if ((bool)containerParameters && containerParameters.PhysicActivated)
			{
				if (!playerHealth)
				{
					playerHealth = ServiceLocator.GetGameObject("Player").GetComponentSafe<Health>();
				}
				if ((playerHealth.transform.position - base.transform.position).magnitude > 50f)
				{
					OnReviveDumpster();
					containerParameters.DeactivatePhysic();
				}
			}
		}

		public void OnReviveDumpster(bool useDefaultPosition = true)
		{
			FreshDumpster.SetActive(value: true);
			Wreck.SetActive(value: false);
			if (useDefaultPosition)
			{
				FreshDumpster.transform.position = defaultPosition;
				FreshDumpster.transform.rotation = defaultRotation;
			}
		}

		public void OnDestroyDumpster()
		{
			if ((bool)Wreck)
			{
				Wreck.transform.position = FreshDumpster.transform.position;
				Wreck.transform.rotation = FreshDumpster.transform.rotation;
				if ((bool)FreshDumpster && FreshDumpster.activeSelf)
				{
					FreshDumpster.SetActive(value: false);
				}
				if ((bool)Wreck && !Wreck.activeSelf)
				{
					Wreck.SetActive(value: true);
				}
				rigidbody.AddExplosionForce(150000f, base.transform.position, 10f);
				TrackDestroyTimer.Run(45f);
			}
		}
	}
}
