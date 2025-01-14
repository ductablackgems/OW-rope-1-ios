using System.Collections.Generic;
using UnityEngine;

namespace FluffyUnderware.Curvy.Examples
{
	public class Result
	{
		public string Name;

		public List<double> Values = new List<double>();

		public void OnGUI()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(Name, GUILayout.Width(400f));
			foreach (double value in Values)
			{
				GUILayout.Label($"{value:0.0000}", GUILayout.Width(120f));
			}
			GUILayout.EndHorizontal();
		}
	}
}
