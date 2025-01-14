using UnityEngine;

namespace Forge3D
{
	[ExecuteInEditMode]
	public class F3DParticleScale : MonoBehaviour
	{
		[Range(0f, 20f)]
		public float ParticleScale = 1f;

		public bool ScaleGameobject = true;

		private float prevScale;

		private void Start()
		{
			prevScale = ParticleScale;
		}

		private void ScaleShurikenSystems(float scaleFactor)
		{
		}

		private void ScaleTrailRenderers(float scaleFactor)
		{
			TrailRenderer[] componentsInChildren = GetComponentsInChildren<TrailRenderer>();
			foreach (TrailRenderer obj in componentsInChildren)
			{
				obj.startWidth *= scaleFactor;
				obj.endWidth *= scaleFactor;
			}
		}

		private void Update()
		{
		}
	}
}
