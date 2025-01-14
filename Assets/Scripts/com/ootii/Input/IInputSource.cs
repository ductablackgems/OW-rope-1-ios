using UnityEngine;

namespace com.ootii.Input
{
	public interface IInputSource
	{
		bool IsEnabled
		{
			get;
			set;
		}

		float InputFromCameraAngle
		{
			get;
			set;
		}

		float InputFromAvatarAngle
		{
			get;
			set;
		}

		float MovementX
		{
			get;
		}

		float MovementY
		{
			get;
		}

		float MovementSqr
		{
			get;
		}

		float ViewX
		{
			get;
		}

		float ViewY
		{
			get;
		}

		bool IsViewingActivated
		{
			get;
		}

		bool IsJustPressed(KeyCode rKey);

		bool IsJustPressed(int rKey);

		bool IsJustPressed(string rAction);

		bool IsPressed(KeyCode rKey);

		bool IsPressed(int rKey);

		bool IsPressed(string rAction);

		bool IsJustReleased(KeyCode rKey);

		bool IsJustReleased(int rKey);

		bool IsJustReleased(string rAction);

		bool IsReleased(KeyCode rKey);

		bool IsReleased(int rKey);

		bool IsReleased(string rAction);

		float GetValue(int rKey);

		float GetValue(string rAction);
	}
}
