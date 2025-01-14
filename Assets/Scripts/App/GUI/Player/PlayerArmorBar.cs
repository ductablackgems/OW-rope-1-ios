using App.Player;
using UnityEngine;

namespace App.GUI.Player
{
	public class PlayerArmorBar : MonoBehaviour
	{
		private Armor armor;

		public SpriteRenderer spriteRenderer;

		private UISlider slider;

		private void Awake()
		{
			armor = ServiceLocator.GetGameObject("Player").GetComponentSafe<Armor>();
			slider = this.GetComponentSafe<UISlider>();
		}

		private void Update()
		{
			if (armor != null)
			{
				slider.value = armor.CurrentArmor;
				spriteRenderer.size = new Vector2(armor.CurrentArmor * 0.89f, 0.13f);
			}
		}
	}
}
