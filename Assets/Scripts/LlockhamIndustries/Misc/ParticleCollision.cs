using UnityEngine;

namespace LlockhamIndustries.Misc
{
	public class ParticleCollision : MonoBehaviour
	{
		public ParticleSystem partSystem;

		private void OnCollisionEnter(Collision collision)
		{
			Object.Instantiate(partSystem, base.transform.position, partSystem.transform.rotation, base.transform.parent).gameObject.name = "Splash Particles";
		}
	}
}
