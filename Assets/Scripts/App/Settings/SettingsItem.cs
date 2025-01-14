using UnityEngine;

namespace App.Settings
{
	public abstract class SettingsItem
	{
		[SerializeField]
		private string id;

		[SerializeField]
		private bool isDisabled;

		public string ID => id;

		public bool IsDisabled => isDisabled;
	}
}
