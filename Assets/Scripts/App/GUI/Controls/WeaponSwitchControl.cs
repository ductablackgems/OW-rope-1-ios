using App.Weapons;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace App.GUI.Controls
{
	public class WeaponSwitchControl : MonoBehaviour
	{
		[SerializeField]
		private Text textWeapon;

		[SerializeField]
		private Button buttonNext;

		[SerializeField]
		private Button buttonPrev;

		private WeaponController controller;

		public WeaponController Controller
		{
			get
			{
				return controller;
			}
			set
			{
				SetController(value);
			}
		}

		private void Awake()
		{
			AddListener(buttonNext, OnNextButtonClicked);
			AddListener(buttonPrev, OnPrevButtonClicked);
		}

		private void OnDestroy()
		{
			RemoveListeners(buttonNext);
			RemoveListeners(buttonPrev);
		}

		private void OnNextButtonClicked()
		{
			SwitchWeapon(nextWeapon: true);
		}

		private void OnPrevButtonClicked()
		{
			SwitchWeapon(nextWeapon: false);
		}

		private void SetController(WeaponController controller)
		{
			this.controller = controller;
			base.gameObject.SetActive(controller != null);
			if (!(controller == null))
			{
				UpdateWeaponName();
			}
		}

		private void SwitchWeapon(bool nextWeapon)
		{
			if (!(controller == null))
			{
				if (nextWeapon)
				{
					controller.SwitchWeapon();
				}
				else
				{
					controller.SwitchToPreviousWeapon();
				}
				UpdateWeaponName();
			}
		}

		private void UpdateWeaponName()
		{
			IWeapon currentWeapon = controller.GetCurrentWeapon();
			textWeapon.text = currentWeapon.GetGunName();
		}

		private static void RemoveListeners(Button button)
		{
			if (!(button == null))
			{
				button.onClick.RemoveAllListeners();
			}
		}

		private static void AddListener(Button button, UnityAction listener)
		{
			if (!(button == null))
			{
				button.onClick.AddListener(listener);
			}
		}
	}
}
