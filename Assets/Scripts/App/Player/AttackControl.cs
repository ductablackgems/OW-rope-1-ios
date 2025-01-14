using System;
using UnityEngine;

namespace App.Player
{
	[Serializable]
	public class AttackControl
	{
		public ControlType controlType;

		public KeyCode keyCode;

		public MouseButtonType mouseButtonType;

		public string alternativeButtonName = string.Empty;
	}
}
