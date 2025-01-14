using UnityEngine;

namespace com.ootii.Input
{
	[AddComponentMenu("ootii/Input Sources/Unity Input Source")]
	public class UnityInputSource : MonoBehaviour, IInputSource, IViewActivator
	{
		public bool _IsEnabled = true;

		public bool _IsXboxControllerEnabled;

		public int _ViewActivator = 2;

		public virtual bool IsEnabled
		{
			get
			{
				return _IsEnabled;
			}
			set
			{
				_IsEnabled = value;
			}
		}

		public virtual bool IsXboxControllerEnabled => _IsXboxControllerEnabled;

		public virtual float InputFromCameraAngle
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		public virtual float InputFromAvatarAngle
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		public virtual float MovementX
		{
			get
			{
				if (!_IsEnabled)
				{
					return 0f;
				}
				float num = 0f;
				if (UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetKey(KeyCode.RightArrow))
				{
					num += 1f;
				}
				if (UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetKey(KeyCode.LeftArrow))
				{
					num -= 1f;
				}
				if (_IsXboxControllerEnabled && num == 0f)
				{
					try
					{
						num = UnityEngine.Input.GetAxis("WXLeftStickX");
						return num;
					}
					catch
					{
						return num;
					}
				}
				return num;
			}
		}

		public virtual float MovementY
		{
			get
			{
				if (!_IsEnabled)
				{
					return 0f;
				}
				float num = 0f;
				if (UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetKey(KeyCode.UpArrow))
				{
					num += 1f;
				}
				if (UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetKey(KeyCode.DownArrow))
				{
					num -= 1f;
				}
				if (_IsXboxControllerEnabled && num == 0f)
				{
					try
					{
						num = UnityEngine.Input.GetAxis("WXLeftStickY");
						return num;
					}
					catch
					{
						return num;
					}
				}
				return num;
			}
		}

		public virtual float MovementSqr
		{
			get
			{
				if (!_IsEnabled)
				{
					return 0f;
				}
				float movementX = MovementX;
				float movementY = MovementY;
				return movementX * movementX + movementY * movementY;
			}
		}

		public virtual float ViewX
		{
			get
			{
				if (!_IsEnabled)
				{
					return 0f;
				}
				float num = UnityEngine.Input.GetAxis("Mouse X");
				if (_IsXboxControllerEnabled && num == 0f)
				{
					num = UnityEngine.Input.GetAxis("WXRightStickX") * (Time.deltaTime / 0.01666f);
				}
				return num;
			}
		}

		public virtual float ViewY
		{
			get
			{
				if (!_IsEnabled)
				{
					return 0f;
				}
				float num = UnityEngine.Input.GetAxis("Mouse Y");
				if (_IsXboxControllerEnabled && num == 0f)
				{
					num = UnityEngine.Input.GetAxis("WXRightStickY") * (Time.deltaTime / 0.01666f);
				}
				return num;
			}
		}

		public virtual bool IsViewingActivated
		{
			get
			{
				if (!_IsEnabled)
				{
					return false;
				}
				bool flag = false;
				if (_IsXboxControllerEnabled)
				{
					flag = (UnityEngine.Input.GetAxis("WXRightStickX") != 0f);
					if (!flag)
					{
						flag = (UnityEngine.Input.GetAxis("WXRightStickY") != 0f);
					}
				}
				if (!flag)
				{
					if (_ViewActivator == 0)
					{
						flag = true;
					}
					else if (_ViewActivator == 1)
					{
						flag = UnityEngine.Input.GetMouseButton(0);
					}
					else if (_ViewActivator == 2)
					{
						flag = UnityEngine.Input.GetMouseButton(1);
					}
					else if (_ViewActivator == 3)
					{
						flag = UnityEngine.Input.GetMouseButton(0);
						if (!flag)
						{
							flag = UnityEngine.Input.GetMouseButton(1);
						}
					}
					else if (_ViewActivator == 4)
					{
						flag = UnityEngine.Input.GetMouseButton(2);
					}
				}
				return flag;
			}
		}

		public int ViewActivator
		{
			get
			{
				return _ViewActivator;
			}
			set
			{
				_ViewActivator = value;
			}
		}

		public virtual bool IsJustPressed(KeyCode rKey)
		{
			if (!_IsEnabled)
			{
				return false;
			}
			return UnityEngine.Input.GetKeyDown(rKey);
		}

		public virtual bool IsJustPressed(int rKey)
		{
			if (!_IsEnabled)
			{
				return false;
			}
			return UnityEngine.Input.GetKeyDown((KeyCode)rKey);
		}

		public virtual bool IsJustPressed(string rAction)
		{
			if (!_IsEnabled)
			{
				return false;
			}
			try
			{
				return UnityEngine.Input.GetButtonDown(rAction);
			}
			catch
			{
				return false;
			}
		}

		public virtual bool IsPressed(KeyCode rKey)
		{
			if (!_IsEnabled)
			{
				return false;
			}
			return UnityEngine.Input.GetKey(rKey);
		}

		public virtual bool IsPressed(int rKey)
		{
			if (!_IsEnabled)
			{
				return false;
			}
			return UnityEngine.Input.GetKey((KeyCode)rKey);
		}

		public virtual bool IsPressed(string rAction)
		{
			if (!_IsEnabled)
			{
				return false;
			}
			try
			{
				bool flag = UnityEngine.Input.GetButton(rAction);
				if (!flag)
				{
					flag = (UnityEngine.Input.GetAxis(rAction) != 0f);
				}
				return flag;
			}
			catch
			{
				return false;
			}
		}

		public virtual bool IsJustReleased(KeyCode rKey)
		{
			if (!_IsEnabled)
			{
				return false;
			}
			return UnityEngine.Input.GetKeyUp(rKey);
		}

		public virtual bool IsJustReleased(int rKey)
		{
			if (!_IsEnabled)
			{
				return false;
			}
			return UnityEngine.Input.GetKeyUp((KeyCode)rKey);
		}

		public virtual bool IsJustReleased(string rAction)
		{
			if (!_IsEnabled)
			{
				return false;
			}
			try
			{
				return UnityEngine.Input.GetButtonUp(rAction);
			}
			catch
			{
				return false;
			}
		}

		public virtual bool IsReleased(KeyCode rKey)
		{
			if (!_IsEnabled)
			{
				return false;
			}
			return !UnityEngine.Input.GetKey(rKey);
		}

		public virtual bool IsReleased(int rKey)
		{
			if (!_IsEnabled)
			{
				return false;
			}
			return !UnityEngine.Input.GetKey((KeyCode)rKey);
		}

		public virtual bool IsReleased(string rAction)
		{
			if (!_IsEnabled)
			{
				return false;
			}
			try
			{
				bool flag = UnityEngine.Input.GetButton(rAction);
				if (!flag)
				{
					flag = (UnityEngine.Input.GetAxis(rAction) != 0f);
				}
				return !flag;
			}
			catch
			{
				return false;
			}
		}

		public virtual float GetValue(int rKey)
		{
			return 0f;
		}

		public virtual float GetValue(string rAction)
		{
			try
			{
				return UnityEngine.Input.GetAxis(rAction);
			}
			catch
			{
				return 0f;
			}
		}
	}
}
