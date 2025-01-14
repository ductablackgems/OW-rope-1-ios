using UnityEngine;

namespace App.Util
{
	[RequireComponent(typeof(UISlider))]
	public class EnergyBar : MonoBehaviour
	{
		public EnergyScript energy;
		public SpriteRenderer spriteRender;

		private UISlider slider;

		private void Awake()
		{
			slider = this.GetComponentSafe<UISlider>();
		}

		private void Update()
		{
			slider.value = energy.GetCurrentEnergy();
			spriteRender.size = new Vector2(energy.GetCurrentEnergy() * 0.89f, 0.13f);
		}
	}
}
