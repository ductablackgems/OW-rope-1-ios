using System.Collections.Generic;
using UnityEngine;

namespace com.ootii.Utilities.Debug
{
	public class SceneFlags : MonoBehaviour
	{
		public struct SceneFlag
		{
			public Vector3 Position;

			public Quaternion Rotation;

			public float Height;

			public Color Color;

			public string Text;

			public Vector3 Vector;
		}

		public static bool IsActive = true;

		public static List<SceneFlag> Flags = new List<SceneFlag>();

		public bool _IsEnabled = true;

		public bool IsEnabled
		{
			get
			{
				return _IsEnabled;
			}
			set
			{
				_IsEnabled = value;
				IsActive = _IsEnabled;
			}
		}

		public static void AddFlag(Vector3 rPosition, Quaternion rRotation)
		{
			AddFlag(rPosition, rRotation, 1f, Color.black, "", Vector3.zero);
		}

		public static void AddFlag(Vector3 rPosition, Quaternion rRotation, Color rColor)
		{
			AddFlag(rPosition, rRotation, 1f, rColor, "", Vector3.zero);
		}

		public static void AddFlag(Vector3 rPosition, Quaternion rRotation, float rHeight, Color rColor)
		{
			AddFlag(rPosition, rRotation, rHeight, rColor, "", Vector3.zero);
		}

		public static void AddFlag(Vector3 rPosition, Quaternion rRotation, Color rColor, string rText)
		{
			AddFlag(rPosition, rRotation, 1f, rColor, rText, Vector3.zero);
		}

		public static void AddFlag(Vector3 rPosition, Quaternion rRotation, float rHeight, Color rColor, string rText)
		{
			AddFlag(rPosition, rRotation, rHeight, rColor, rText, Vector3.zero);
		}

		public static void AddFlag(Vector3 rPosition, Quaternion rRotation, Color rColor, Vector3 rVector)
		{
			AddFlag(rPosition, rRotation, 1f, rColor, "", rVector);
		}

		public static void AddFlag(Vector3 rPosition, Quaternion rRotation, float rHeight, Color rColor, string rText, Vector3 rVector)
		{
			if (IsActive)
			{
				SceneFlag item = default(SceneFlag);
				item.Position = rPosition;
				item.Rotation = rRotation;
				item.Height = rHeight;
				item.Color = rColor;
				item.Text = rText;
				item.Vector = rVector;
				Flags.Add(item);
			}
		}

		private void Awake()
		{
			IsActive = _IsEnabled;
			Flags.Clear();
		}

		private void OnDrawGizmos()
		{
		}
	}
}
