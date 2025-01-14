using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	public sealed class MinAttribute_FPS : PropertyAttribute
	{
		public readonly float min;

		public MinAttribute_FPS(float min)
		{
			this.min = min;
		}
	}
}
