using System;

namespace FluffyUnderware.Curvy.Utils
{
	[Serializable]
	internal class Edge
	{
		public int[] vertexIndex = new int[2];

		public int[] faceIndex = new int[2];

		public override string ToString()
		{
			return vertexIndex[0].ToString() + "-" + vertexIndex[1].ToString();
		}
	}
}
