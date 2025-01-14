using App.SaveSystem;
using App.Util;
using com.ootii.Input;
using UnityEngine;

namespace App.Camera
{
	public class OotiiInputSource : MonoBehaviour, IInputSource
	{
		private SettingsSaveEntity settingsSave;

		public bool IsEnabled
		{
			get;
			set;
		}

		public float InputFromCameraAngle
		{
			get;
			set;
		}

		public float InputFromAvatarAngle
		{
			get;
			set;
		}

		public float MovementX => 0f;

		public float MovementY => 0f;

		public float MovementSqr => 0f;

		public float ViewX => InputUtils.GetHorizontalLookAxis() * settingsSave.screenSensitivyX;

		public float ViewY => InputUtils.GetVerticalLookAxis() * settingsSave.screenSensitivyY;

		public bool IsViewingActivated => true;

		public float GetValue(int rKey)
		{
			return 0f;
		}

		public float GetValue(string rAction)
		{
			return 0f;
		}

		public bool IsJustPressed(KeyCode rKey)
		{
			return false;
		}

		public bool IsJustPressed(int rKey)
		{
			return false;
		}

		public bool IsJustPressed(string rAction)
		{
			return false;
		}

		public bool IsJustReleased(KeyCode rKey)
		{
			return false;
		}

		public bool IsJustReleased(int rKey)
		{
			return false;
		}

		public bool IsJustReleased(string rAction)
		{
			return false;
		}

		public bool IsPressed(KeyCode rKey)
		{
			return false;
		}

		public bool IsPressed(int rKey)
		{
			return false;
		}

		public bool IsPressed(string rAction)
		{
			return false;
		}

		public bool IsReleased(KeyCode rKey)
		{
			return false;
		}

		public bool IsReleased(int rKey)
		{
			return false;
		}

		public bool IsReleased(string rAction)
		{
			return false;
		}

		private void Awake()
		{
			settingsSave = ServiceLocator.Get<SaveEntities>().SettingsSave;
			IsEnabled = true;
		}
	}
}
