using com.ootii.Helpers;
using com.ootii.Input;
using System;
using UnityEngine;

namespace com.ootii.Game
{
	public class GameCore : MonoBehaviour
	{
		public static GameCore Core;

		public GameObject _InputSourceOwner;

		[NonSerialized]
		public IInputSource _InputSource;

		public bool _AutoFindInputSource = true;

		public bool _IsCursorVisible;

		public string _ShowCursorAlias = "Cursor";

		public string _EditorPauseAlias = "";

		public GameObject InputSourceOwner
		{
			get
			{
				return _InputSourceOwner;
			}
			set
			{
				_InputSourceOwner = value;
			}
		}

		public IInputSource InputSource
		{
			get
			{
				return _InputSource;
			}
			set
			{
				_InputSource = value;
			}
		}

		public bool AutoFindInputSource
		{
			get
			{
				return _AutoFindInputSource;
			}
			set
			{
				_AutoFindInputSource = value;
			}
		}

		public bool IsCursorVisible
		{
			get
			{
				return _IsCursorVisible;
			}
			set
			{
				_IsCursorVisible = value;
				Cursor.lockState = ((!_IsCursorVisible) ? CursorLockMode.Locked : CursorLockMode.None);
				Cursor.visible = _IsCursorVisible;
			}
		}

		public string ShowCursorAlias
		{
			get
			{
				return _ShowCursorAlias;
			}
			set
			{
				_ShowCursorAlias = value;
			}
		}

		public string EditorPauseAlias
		{
			get
			{
				return _EditorPauseAlias;
			}
			set
			{
				_EditorPauseAlias = value;
			}
		}

		protected void Awake()
		{
			if (Core != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (Core == null)
			{
				Core = this;
			}
			if (_InputSourceOwner != null)
			{
				_InputSource = InterfaceHelper.GetComponent<IInputSource>(_InputSourceOwner);
			}
			if (_InputSource == null)
			{
				_InputSource = InterfaceHelper.GetComponent<IInputSource>(base.gameObject);
			}
			if (_AutoFindInputSource && _InputSource == null)
			{
				IInputSource[] components = InterfaceHelper.GetComponents<IInputSource>();
				for (int i = 0; i < components.Length; i++)
				{
					GameObject gameObject = ((MonoBehaviour)components[i]).gameObject;
					if (gameObject.activeSelf && components[i].IsEnabled)
					{
						_InputSource = components[i];
						_InputSourceOwner = gameObject;
					}
				}
			}
			IsCursorVisible = _IsCursorVisible;
		}

		protected void Update()
		{
			if (_InputSource != null && _ShowCursorAlias.Length > 0 && _InputSource.IsJustPressed(_ShowCursorAlias))
			{
				IsCursorVisible = !IsCursorVisible;
			}
		}
	}
}
