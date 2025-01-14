using UnityEngine;

namespace App.Player.ColorManager
{
	public class PlayerColorManager : MonoBehaviour
	{
		public SkinnedMeshRenderer _renderer;

		public Material[] materials;

		public int ColorIndex
		{
			get;
			private set;
		}

		public void SetColorIndex(int colorIndex)
		{
			if (colorIndex < materials.Length && colorIndex >= 0)
			{
				ColorIndex = colorIndex;
				_renderer.material = materials[colorIndex];
			}
		}
	}
}
