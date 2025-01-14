using System;
using UnityEngine;

namespace App.Util
{
	public class SceneBorders : MonoBehaviour
	{
		[Serializable]
		public class Borders
		{
			public Bounds Bounds;

			public Color Color = Color.clear;
		}

		[SerializeField]
		private Borders borders;

		public Bounds BorderBounds => borders.Bounds;

		private void OnDrawGizmosSelected()
		{
			if (borders != null)
			{
				Color color = Gizmos.color;
				Gizmos.color = borders.Color;
				Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.lossyScale);
				Gizmos.DrawCube(borders.Bounds.center, borders.Bounds.size);
				Gizmos.color = color;
			}
		}
	}
}
