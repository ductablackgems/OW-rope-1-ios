using UnityEngine;

namespace FluffyUnderware.Curvy.Examples
{
	public class FollowSplineExampleController : MonoBehaviour
	{
		public FollowSpline[] Controllers;

		private int selection;

		private void Start()
		{
			SetRelative();
		}

		private void OnGUI()
		{
			if (GUILayout.Button("Reset"))
			{
				FollowSpline[] controllers = Controllers;
				for (int i = 0; i < controllers.Length; i++)
				{
					controllers[i].Initialize();
				}
			}
			GUILayout.BeginHorizontal();
			GUILayout.Label("Movement Mode: ");
			int num = GUILayout.SelectionGrid(selection, new string[2]
			{
				"Relative",
				"Absolute"
			}, 2);
			GUILayout.EndHorizontal();
			if (num != selection)
			{
				selection = num;
				switch (selection)
				{
				case 0:
					SetRelative();
					break;
				case 1:
					SetAbsolute();
					break;
				}
			}
		}

		private void SetRelative()
		{
			FollowSpline[] controllers = Controllers;
			foreach (FollowSpline obj in controllers)
			{
				obj.Mode = FollowSpline.FollowMode.Relative;
				obj.Speed = 0.2f;
			}
		}

		private void SetAbsolute()
		{
			FollowSpline[] controllers = Controllers;
			foreach (FollowSpline obj in controllers)
			{
				obj.Mode = FollowSpline.FollowMode.AbsoluteExtrapolate;
				obj.Speed = 4f;
			}
		}
	}
}
