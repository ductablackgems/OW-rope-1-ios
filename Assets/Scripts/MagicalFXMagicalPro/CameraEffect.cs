using UnityEngine;

namespace MagicalFXMagicalPro
{
	public static class CameraEffect
	{
		public static FX_Camera CameraFX;

		public static void Shake(Vector3 power)
		{
			if (CameraFX != null)
			{
				CameraFX.Shake(power);
			}
		}
	}
}
