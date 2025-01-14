using UnityEngine;

namespace Forge3D
{
	public class F3DShotgun : MonoBehaviour
	{
		private ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[16];

		private ParticleSystem ps;

		private void Start()
		{
			ps = GetComponent<ParticleSystem>();
		}

		private void OnParticleCollision(GameObject other)
		{
			int safeCollisionEventSize = ps.GetSafeCollisionEventSize();
			if (collisionEvents.Length < safeCollisionEventSize)
			{
				collisionEvents = new ParticleCollisionEvent[safeCollisionEventSize];
			}
			int num = ps.GetCollisionEvents(other, collisionEvents);
			for (int i = 0; i < num; i++)
			{
				F3DAudioController.instance.ShotGunHit(collisionEvents[i].intersection);
				Rigidbody component = other.GetComponent<Rigidbody>();
				if ((bool)component)
				{
					Vector3 intersection = collisionEvents[i].intersection;
					Vector3 force = collisionEvents[i].velocity.normalized * 50f;
					component.AddForceAtPosition(force, intersection);
				}
			}
		}
	}
}
