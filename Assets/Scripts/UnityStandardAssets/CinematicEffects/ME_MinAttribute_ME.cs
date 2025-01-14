using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	public sealed class ME_MinAttribute_ME : PropertyAttribute
	{
		public readonly float min;

		public ME_MinAttribute_ME(float min)
		{
			this.min = min;
		}
	}
}
