using App.Settings;
using System;
using UnityEngine;

namespace App.GUI
{
	public class ControlTypeSpecificItem : MonoBehaviour
	{
		public ControlMode[] SupportedControls = new ControlMode[2]
		{
			ControlMode.simple,
			ControlMode.touch
		};

		public bool AffectVisibilityOnly;

		private bool isActive;

		private ControlMode currentMode;

		private void Start()
		{
			isActive = GetActiveState();
			SetActiveState(isActive);
		}

		private void OnEnable()
		{
			isActive = GetActiveState();
			SetActiveState(isActive);
		}

		private bool GetActiveState()
		{
			currentMode = SettingsManager.GameSettings.ControlMode;
			bool result = false;
			for (int i = 0; i < SupportedControls.Length; i++)
			{
				if (SupportedControls[i] == currentMode)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private void SetActiveState(bool isActive)
		{
			if (!AffectVisibilityOnly)
			{
				base.gameObject.SetActive(isActive);
				return;
			}
			CanvasGroup component = GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.alpha = Convert.ToSingle(isActive);
				return;
			}
			ETCButton component2 = GetComponent<ETCButton>();
			if (component2 != null)
			{
				component2.visible = isActive;
			}
		}
	}
}
